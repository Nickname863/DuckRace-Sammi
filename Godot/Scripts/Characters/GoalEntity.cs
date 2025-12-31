using Godot;
using System;
using System.Reflection;

namespace DuckRace.Scripts.Characters;

public partial class GoalEntity : Area2D
{
    [Export()]
    private CollisionShape2D _collider;
    
    public override void _Ready()
    {
        var viewPortSize = GetViewport().GetVisibleRect().Size;
        float containerHeight = viewPortSize.Y;
        float containerWidth = viewPortSize.X;
        var y = GetPosition().Y;
        SetPosition(new Vector2(0 - 200, y));

        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.GetParent() is Duck duck)
        {

            BodyEntered -= OnBodyEntered;
            MainWindow.Instance.DuckWin(duck);
        }

    }
}