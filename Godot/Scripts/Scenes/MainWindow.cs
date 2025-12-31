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
    private Node2D raceNode;
    private SubViewport subViewport;
    private SubViewportContainer subViewportContainer;
    private Celebration celebrationNode;
    private Timer _goalLineCrossedTimer;
    private bool raceInProgress = false;

    private DuckRaceInfo _winner;
    private Spout spout2 = Spout.Instantiate();

    private AnimationPlayer _startTextAnimationPlayer;
    private HttpRequest httpRequest;
    private RichTextLabel _errorOutPut;

    public override void _Ready()
    {
        GD.Print("MainWindow");

        spout2.SetSenderName("Duck Race");

        Instance = this;
        subViewport = GetNode<SubViewport>("SubViewportContainer/SubViewport");
        subViewportContainer = GetNode<SubViewportContainer>("SubViewportContainer");
        _startTextAnimationPlayer = GetNode<AnimationPlayer>("StartTextAnimator");
        _errorOutPut = GetNode<RichTextLabel>("ErrorOutput");
        _startTextAnimationPlayer.AnimationFinished += RaceStartOnAnimationFinished;

        httpRequest = GetNode<HttpRequest>("HTTPRequest");
        httpRequest.RequestCompleted += InstanceOnDuckRaceWinnerSendEventHandler;



        DisplayServer.RegisterAdditionalOutput(subViewportContainer);
        _goalLineCrossedTimer = GetNode<Timer>("GoalLineCrossedTimer");
        _goalLineCrossedTimer.WaitTime = 2;
        _goalLineCrossedTimer.Timeout += RemoveRaceFromViewport;


    }

    // We use the Godot HTTP Request Handler, because starting a new .NET Thread mid execution causes an ugly visual stutter
    private void SendWebRequesttoSammi(DuckRaceInfo duckInfo)
    {
        var webhookContent = new WebhookMessage() { customObjectData = duckInfo };
        httpRequest.Request("http://localhost:9450/webhook", Array.Empty<string>(), HttpClient.Method.Post, JsonSerializer.Serialize(webhookContent));
    }

    private async void InstanceOnDuckRaceWinnerSendEventHandler(long result, long responsecode, string[] headers, byte[] body)
    {
        if (responsecode == 0)
        {
            _errorOutPut.Text += "Could Not access Sammi, is it running? And did you activate the local API server?\n";
            return;
        }
        await ToSignal(GetTree().CreateTimer(6.0f), SceneTreeTimer.SignalName.Timeout);
        RemoveCelebrationViewport();
    }


    private void RaceStartOnAnimationFinished(StringName animname)
    {
        _instantiatedDucks.ForEach(x => x.StartRacing());
    }


    public override void _Process(double delta)
    {

        if (GlobalDataManager.Instance.RaceParticipants.Count > 0 && !raceInProgress)
        {
            var duckRaceParticipants = new List<DuckRaceInfo>();
            while (GlobalDataManager.Instance.RaceParticipants.TryTake(out var duck))
            {
                duckRaceParticipants.Add(duck);
            }

            PrepareRaceScene();
            PrepareDuckRace(duckRaceParticipants);
            _startTextAnimationPlayer.Play("readySetGo");
            raceInProgress = true;
            Engine.MaxFps = 60;
        }

        SendSubViewportToSpout();
        if (!raceInProgress)
        {
            // To prevent visual lag i will ramp down the max fps over multiple frames
            if (Engine.MaxFps > GlobalDataManager.Instance.DestinationFPS)
            {
                Engine.MaxFps -= 1;
            }
        }

    }

    private void PrepareRaceScene()
    {
        var raceScenePacked = GD.Load<PackedScene>("res://Scenes/raceViewport.tscn");
        var race = raceScenePacked.Instantiate<Node2D>();
        var intendedHoizontal = subViewport.GetVisibleRect().Size.X / 2;
        race.SetPosition(new Vector2(intendedHoizontal, race.Position.Y));
        raceNode = race;
        subViewport.AddChild(race);
    }

    private void SendSubViewportToSpout()
    {
        var screenTexture = subViewport.GetTexture();

        var viewportTextureRid = RenderingServer.ViewportGetTexture(subViewport.GetViewportRid());
        var textureHandle = RenderingServer.TextureGetNativeHandle(viewportTextureRid);
        spout2.SendTexture((long)textureHandle, 0x0DE1, screenTexture.GetWidth(), screenTexture.GetHeight(), false, 0);
    }

    public void PrepareDuckRace(List<DuckRaceInfo> ducks)
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
            raceNode.AddChild(duck);
        }
    }

    public void RemoveRaceFromViewport()
    {
        _goalLineCrossedTimer.Stop();
        _instantiatedDucks.Clear();
        raceNode.QueueFree();
        var packedDuck = GD.Load<PackedScene>("res://Characters/duck.tscn");
        var duck = packedDuck.Instantiate<Duck>();
        var packedCelebration = GD.Load<PackedScene>("res://Scenes/celebration.tscn");
        celebrationNode = packedCelebration.Instantiate<Celebration>();
        duck.Name = "DuckNode";
        duck.DuckInfo = _winner;
        celebrationNode.AddChild(duck);
        subViewport.AddChild(celebrationNode);
        SendWebRequesttoSammi(duck.DuckInfo);
    }

    public void RemoveCelebrationViewport()
    {
        subViewport.CallDeferred(MethodName.RemoveChild, celebrationNode);
        celebrationNode = null;
        raceInProgress = false;

    }

    public void DuckWin(Duck winner)
    {
        _instantiatedDucks.Where(x => x != winner).ToList().ForEach(x => x.Freeze());
        _instantiatedDucks.Clear();
        _winner = winner.DuckInfo;
        _goalLineCrossedTimer.Start();
    }
}
