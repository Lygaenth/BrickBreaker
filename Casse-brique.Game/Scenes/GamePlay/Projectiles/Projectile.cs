using Godot;
using System;

public partial class Projectile : RigidBody2D
{
	private int _speed = 150;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y + (float)delta * _speed);
	}
}
