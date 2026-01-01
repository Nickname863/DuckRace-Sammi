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
        var y = GetPosition().Y;
        SetPosition(new Vector2(0 - 200, y));
        // Add Collision to the Goal Line
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        // If The parent of the collision shape that hit the goal is a Duck it wins
        if (body.GetParent() is Duck duck)
        {

            BodyEntered -= OnBodyEntered;
            MainWindow.Instance.DuckWin(duck);
        }

    }
}