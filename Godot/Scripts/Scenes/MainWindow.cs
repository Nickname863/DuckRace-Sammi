using DuckRace.Scripts.Characters;
using DuckRace.Scripts.DataModels;
using DuckRace.Scripts.Global;
using GDExtension.Wrappers;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HttpClient = Godot.HttpClient;
using Timer = Godot.Timer;

public partial class MainWindow : Node2D
{
    public static MainWindow Instance;

    private readonly List<Duck> _instantiatedDucks = new List<Duck>();
    private Node2D _raceNode;
    private SubViewport _subViewport;
    private SubViewportContainer _subViewportContainer;
    private Celebration _celebrationNode;
    private Timer _goalLineCrossedTimer;
    private bool _raceInProgress;
    private const float SecondsToWaitBeforeClearingCelebration = 6.0f;

    private DuckRaceInfo _winner;
    private readonly Spout _spout2 = Spout.Instantiate();

    private AnimationPlayer _startTextAnimationPlayer;
    private HttpRequest _httpRequest;
    private RichTextLabel _errorOutPut;

    public override void _Ready()
    {
        GD.Print("MainWindow");

        _spout2.SetSenderName("Duck Race");

        // Make this class a singleton, technically we don't need to but it was simpler/quicker to do it like this
        Instance = this;

        // Get all Nodes
        _subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        _subViewportContainer = GetNode<SubViewportContainer>("SubViewportContainer");
        _startTextAnimationPlayer = GetNode<AnimationPlayer>("StartTextAnimator");
        _errorOutPut = GetNode<RichTextLabel>("ErrorOutput");
        _httpRequest = GetNode<HttpRequest>("HTTPRequest");
        _goalLineCrossedTimer = GetNode<Timer>("GoalLineCrossedTimer");

        // Register some events
        _startTextAnimationPlayer.AnimationFinished += RaceStartOnAnimationFinished;
        _httpRequest.RequestCompleted += InstanceOnDuckRaceWinnerSendEventHandler;
        _goalLineCrossedTimer.Timeout += RemoveRaceFromViewport;

        // Time to wait after Line has been crossed
        _goalLineCrossedTimer.WaitTime = 2;


        // This causes the Viewport to always render, even if the App is minimized
        DisplayServer.RegisterAdditionalOutput(_subViewportContainer);


    }

    public override void _Process(double delta)
    {
        // If there are Ducks in queue for race add them to the race
        if (GlobalDataManager.Instance.RaceParticipants.Count > 0 && !_raceInProgress)
        {
            var duckRaceParticipants = new List<DuckRaceInfo>();
            while (GlobalDataManager.Instance.RaceParticipants.TryTake(out var duck))
            {
                duckRaceParticipants.Add(duck);
            }

            PrepareRaceScene();
            PrepareDuckRace(duckRaceParticipants);
            _startTextAnimationPlayer.Play("readySetGo");
            _raceInProgress = true;
            Engine.MaxFps = 60;
        }


        // This sends the texture to OBS
        SendSubViewportToSpout();

        if (!_raceInProgress)
        {
            // To prevent visual lag i will ramp down the max fps over multiple frames
            // There is probably smarter ways to do this.
            if (Engine.MaxFps > GlobalDataManager.Instance.DestinationFPS)
            {
                Engine.MaxFps -= 1;
            }
        }

    }

    // We use the Godot HTTP Request Handler, because starting a new .NET Thread mid execution causes an ugly visual stutter
    private void SendWebRequestToSammi(DuckRaceInfo duckInfo)
    {
        var webhookContent = new WebhookMessage() { customObjectData = duckInfo };
        _httpRequest.Request("http://localhost:9450/webhook", Array.Empty<string>(), HttpClient.Method.Post, JsonSerializer.Serialize(webhookContent));
    }

    private async void InstanceOnDuckRaceWinnerSendEventHandler(long result, long responsecode, string[] headers, byte[] body)
    {
        if (responsecode == 0)
        {
            _errorOutPut.Text += "Could Not access Sammi, is it running? And did you activate the local API server?\n";
            return;
        }
        // Alternate way to wait for a set amount of seconds
        await ToSignal(GetTree().CreateTimer(SecondsToWaitBeforeClearingCelebration), SceneTreeTimer.SignalName.Timeout);
        RemoveCelebrationFromViewport();
    }

    private void RaceStartOnAnimationFinished(StringName animname)
    {
        _instantiatedDucks.ForEach(x => x.StartRacing());
    }


    private void PrepareRaceScene()
    {
        var raceScenePacked = GD.Load<PackedScene>("res://Scenes/raceViewport.tscn");
        var race = raceScenePacked.Instantiate<Node2D>();
        var intendedHoizontal = _subViewport.GetVisibleRect().Size.X / 2;
        race.SetPosition(new Vector2(intendedHoizontal, race.Position.Y));
        _raceNode = race;
        _subViewport.AddChild(race);
    }

    private void PrepareDuckRace(List<DuckRaceInfo> ducks)
    {
        var duckCount = ducks.Count;

        for (int i = duckCount - 1; i >= 0; i--)
        {

            var packedDuck = GD.Load<PackedScene>("res://Characters/duck.tscn");
            var duck = packedDuck.Instantiate<Duck>();
            duck.DuckInfo = ducks[i];
            duck.DuckIndex = i;
            duck.TotalDuckCount = duckCount;
            _instantiatedDucks.Add(duck);
            _raceNode.AddChild(duck);
        }
    }

    private void SendSubViewportToSpout()
    {
        var screenTexture = _subViewport.GetTexture();
        var viewportTextureRid = RenderingServer.ViewportGetTexture(_subViewport.GetViewportRid());
        var textureHandle = RenderingServer.TextureGetNativeHandle(viewportTextureRid);
        _spout2.SendTexture((long)textureHandle, 0x0DE1, screenTexture.GetWidth(), screenTexture.GetHeight(), false, 0);
    }

    private void RemoveRaceFromViewport()
    {
        _goalLineCrossedTimer.Stop();
        _instantiatedDucks.Clear();
        _raceNode.QueueFree();
        var packedDuck = GD.Load<PackedScene>("res://Characters/duck.tscn");
        var duck = packedDuck.Instantiate<Duck>();
        var packedCelebration = GD.Load<PackedScene>("res://Scenes/celebration.tscn");
        _celebrationNode = packedCelebration.Instantiate<Celebration>();
        duck.Name = "DuckNode";
        duck.DuckInfo = _winner;
        _celebrationNode.AddChild(duck);
        _subViewport.AddChild(_celebrationNode);
        SendWebRequestToSammi(duck.DuckInfo);
    }

    public void RemoveCelebrationFromViewport()
    {
        _subViewport.CallDeferred(MethodName.RemoveChild, _celebrationNode);
        _celebrationNode = null;
        _raceInProgress = false;

    }

    public void DuckWin(Duck winner)
    {
        _instantiatedDucks.Where(x => x != winner).ToList().ForEach(x => x.Freeze());
        _instantiatedDucks.Clear();
        _winner = winner.DuckInfo;
        _goalLineCrossedTimer.Start();
    }
}
