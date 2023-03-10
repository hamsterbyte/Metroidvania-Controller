using Godot;
using System;

public partial class CameraController : Camera2D{
	[Export] public CharacterBody2D character;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
		Position = character.GlobalPosition;
	}
	
}
