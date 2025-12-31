using Godot;
using System;
using System.Threading;
using DuckRace.Scripts.Characters;
using DuckRace.Scripts.Global;

public partial class Celebration : Node2D
{
    private AnimationPlayer _animPlayer;
    private Duck _duck;
    private Node2D _wheel;
    private Label _namePlate;
    private Label _headLine;
    private bool _isLabelNameChanged;
    public override void _Ready()
    {

        this.Visible = false;
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _duck = GetNode<Duck>("DuckNode");
        _namePlate = GetNode<Label>("NamePlate");
        _headLine = GetNode<Label>("HeadLine");
        _wheel = GetNode<Node2D>("Wheel");


        _namePlate.Text = _duck.DuckInfo.Name;
        _duck.HideNameLabel();
        _duck.PlayPulseAnimation();
        _animPlayer.SetSpeedScale(0.4f);
        _animPlayer.Play("WinAnimation");
        _isLabelNameChanged = true;
    }

    public override void _Process(double delta)
    {
        // Align everything and set it to visible
        if (_isLabelNameChanged)
        {
            CenterNode(_duck);
            CenterNode(_wheel);
            CenterLabel(_namePlate);
            CenterLabel(_headLine);
            _isLabelNameChanged = false;
            _duck.Visible = true;
            this.Visible = true;
        }

    }
    private void CenterLabel(Label label)
    {
        var viewportWidth = GetViewport().GetVisibleRect().Size.X;
        var width = label.GetRect().Size.X;
        var pos = label.GetPosition();
        label.SetPosition(new Vector2(viewportWidth / 2 - width / 2, pos.Y));
    }
    private void CenterNode(Node2D node)
    {

        var viewportSize = GetViewport().GetVisibleRect().Size;
        node.SetPosition(new Vector2(viewportSize.X / 2 , viewportSize.Y / 2));
    }

}
