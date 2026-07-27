using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;


public partial class Body : Node2D
{
	[Export] public Sprite2D Sprite;
	[Export] public Vector2 Velocity;
	[Export] public Vector2 Acceleration;
	[Export] public double Mass;
	[Export] public double Restitution = 0.2;

    public double Radius;
	public Vector2 NewVelocity;
	public float softening = 0.0f;
    public List<Body> OtherBodies;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Radius = Sprite.GetRect().Size.X / 2;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	
	public void ComputePhysics()
	{
		Vector2 NewAcceleration = Vector2.Zero;
        //GD.Print($"{Name}'s OtherBodies: {OtherBodies.Count}");
        foreach (Body oB in OtherBodies)
		{



            // Check if collision will occur between this and the other body
            //|r2 - r1|^2 <= (r1+r2)^2 then overlap
            if (Position.DistanceSquaredTo(oB.Position) <= Math.Pow(Radius + oB.Radius, 2))
			{
				// v_2 = v_1 + v_2/1
				Vector2 relativeVelocity = oB.Velocity - Velocity;
				Vector2 normalDir = (oB.Position - Position).Normalized();

				// Project velocity along normal 
				Vector2 normalVelocity = relativeVelocity.Dot(normalDir) * normalDir;

				//if (relativeVelocity.Dot(normalDir) > 0)
				//	continue;
				// along the normal: 
				// v_f = -e*v_i
				// J = v_f - v_i
				// J = -e*v_i - v_i
				// -> J = -(1+e)*v_i

				Vector2 Impulse = (float) - (1 + Restitution) * normalVelocity;
				Double MassRatio = (oB.Mass) / (Mass + oB.Mass);
				NewVelocity -= Impulse * (float)MassRatio;

				continue;
			}
            
			// Add acceleration from force exerted by other body
            // r'' = -G*m2*(r1 - r2)/|r1-r2|^3 - G*m3*(r1-r3)/|r1-r3|^3
            NewAcceleration -= (float)(Simulator.G * oB.Mass) * (Position - oB.Position)
                / (float)Math.Pow((Position - oB.Position).Length() + softening, 3.0);

        }

		Acceleration = NewAcceleration;
		//GD.Print($"{Name}'s Acceleration: {Acceleration}");

	}

	public void Update()
	{
		Velocity = NewVelocity;
		Velocity += Acceleration;
		Position += Velocity;
	}

}
