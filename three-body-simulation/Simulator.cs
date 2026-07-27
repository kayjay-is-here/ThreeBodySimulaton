using Godot;
using System;
using System.Linq;

public partial class Simulator : Node2D
{
	
	[Export] Body[] Bodies = new Body[3];
    public static double G = 6.6743 * (10^-11);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        // O(n*m)
        foreach (Body b in Bodies)
        {
            b.OtherBodies = Bodies.Except([b]).ToList();
        }
               

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        foreach (Body b in Bodies)
        {
            b.ComputeAcceleration();
        }

        foreach (Body b in Bodies)
        {
            b.Update();
        }
    }
}
