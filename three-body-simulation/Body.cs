using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;


public partial class Body : Node2D
{
	[Export] public Sprite2D Sprite;
	[Export] public Vector2 Velocity;
	[Export] public Vector2 Acceleration;
	[Export] public double Mass; 
	public List<Body> OtherBodies;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void ComputeAcceleration()
	{
		Vector2 NewAcceleration = Vector2.Zero;
        GD.Print($"{Name}'s OtherBodies: {OtherBodies.Count}");
        foreach (Body oB in OtherBodies)
		{
			if (oB == this) continue;

			// Newton's law of gravitation
			// r'' = -G*m2*(r1 - r2)/|r1-r2|^3 - G*m3*(r1-r3)/|r1-r3|^3
			NewAcceleration -= (float)(Simulator.G*oB.Mass) * (Position - oB.Position)
                / (float)Math.Pow((Position - oB.Position).Length(),3.0);
		}

		Acceleration = NewAcceleration;
		GD.Print($"{Name}'s Acceleration: {Acceleration}");

	}

	public void Update()
	{
		Velocity += Acceleration;
		Position += Velocity;
	}

}
