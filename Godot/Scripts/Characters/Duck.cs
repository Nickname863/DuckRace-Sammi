using DuckRace.Scripts.DataModels;
using Godot;
using System;
using System.Drawing;
using Color = Godot.Color;

namespace DuckRace.Scripts.Characters;

public partial class Duck : Node2D
{
    private AnimationPlayer _animPlayer;
    private Sprite2D _faceSprite;
    private Sprite2D _partyHatSprite;
    private CharacterBody2D _duckCharacter;
    private Label _nameLabel;
    public DuckRaceInfo DuckInfo { get; set; }
    private bool _nameChanged;
    private bool _isMoving;
    private double _elapsedTimeSinceSpeedChange;
    private double _totalElapsedRaceTime;
    private int _speed;

    public int? DuckIndex = null;
    public int? TotalDuckCount = null;
    public override void _Ready()
    {
        Visible = false;
        _animPlayer = GetNode<AnimationPlayer>("./AnimationPlayer");
        _faceSprite = GetNode<Sprite2D>("./Duck/Face");
        _partyHatSprite = GetNode<Sprite2D>("./Duck/Face/PartyHat");
        _duckCharacter = GetNode<CharacterBody2D>("./Duck");
        _nameLabel = GetNode<Label>("./Duck/NameLabel");
        _nameLabel.Text = DuckInfo.Name;
        _nameChanged = true;
        ApplyColoring();
        PositionForRace();

        if (DuckInfo.HasPartyHat)
        {
            _partyHatSprite.Visible = true;
        }

    }
    public void PlayPulseAnimation()
    {
        _animPlayer.SetSpeedScale(0.5f);
        _animPlayer.Play("pulse");
    }

    public void HideNameLabel()
    {
        _nameLabel.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (_nameChanged)
        {
            var width = _nameLabel.GetRect().Size.X;
            var posY = _nameLabel.GetPosition().Y;
            _nameLabel.SetPosition(new Vector2(width/-2,posY));
            _nameChanged = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isMoving)
        {
            _elapsedTimeSinceSpeedChange += delta;
            _totalElapsedRaceTime += delta;
            // Calculate a new speed every 2.5 seconds. If this number is too small things will look bad.
            if (_elapsedTimeSinceSpeedChange > 2.5)
            {
                SetMovementVelocity();
                _elapsedTimeSinceSpeedChange = 0;
            }

            _duckCharacter.MoveAndSlide();
        }
    }

    public void StartRacing()
    {
        _animPlayer.Play("walking");
        SetMovementVelocity();
        _isMoving = true;
    }

    private void SetMovementVelocity()
    {
        var stragglersBonus = 0;
        // To artificially boost the chances of Ducks that have fallen behind i will add a bonus speed relative to the ducks progress
        // This is kinda dumb, but it sometimes causes a duck to recover.
        if (Random.Shared.Next(0,100) < 30)
        {
            var relativeProgress = GetViewport().GetVisibleRect().Size.X / _duckCharacter.GetGlobalPosition().X;
            stragglersBonus = (int) (relativeProgress  * _totalElapsedRaceTime * 8.0);
        }
        _speed = Random.Shared.Next(100 ,  300 + stragglersBonus);
        _animPlayer.SetSpeedScale(0.5f * (_speed / 100.0f));
        _duckCharacter.Velocity = Vector2.Right * _speed;
    }

    public void Freeze()
    {
        _duckCharacter.Velocity = Vector2.Zero;
        (_faceSprite.Material as ShaderMaterial)?.SetShaderParameter("is_grey", true);
        _animPlayer.Stop();
        _isMoving = false;
    }

    public void ApplyColoring()
    {
        _faceSprite.Material = (Material)_faceSprite.Material.Duplicate(true);
        var faceSpriteShaderMat = _faceSprite.Material as ShaderMaterial;
        var newColor = DuckInfo.Color;
        faceSpriteShaderMat.SetShaderParameter("new_color", newColor);
    }

    public void PositionForRace()
    {

        // If the Duck has no index or there is no set Number of Race Ducks, this method will do nothing.
        if (!TotalDuckCount.HasValue || !DuckIndex.HasValue)
        {
            return;
        }
        // We get the size of the next best viewport so we can position our Ducks
        var viewPortSize = GetViewport().GetVisibleRect().Size;
        float containerHeight = viewPortSize.Y;
        float containerWidth = viewPortSize.X;

        var spriteHeight = _faceSprite.Texture.GetHeight();
        var spriteWidth = _faceSprite.Texture.GetWidth();

        var distance = ((containerHeight - spriteHeight) / TotalDuckCount.Value);

        // This is supposed to evenly space out or ducks on the Y axis
        _duckCharacter.SetPosition(new Vector2((containerWidth/-2) + spriteWidth / 2, containerHeight - spriteHeight / 2 - ((distance * DuckIndex.Value)) - distance / 2));
        Visible = true;

    }

}