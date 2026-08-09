using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;


public partial class Body : Node2D
{
	[Export] public Sprite2D Sprite;
	[Export] public Vector2 Velocity;
	[Export] public Vector2 Acceleration;
	[Export] public float Mass;
	[Export] public float Restitution = 0.02f;
	[Export] public float Radius = 100;

	public const float MIN_SPEED = 0.0f;
	
	public Vector2 NewVelocity;
	public float softening = 0.1f;
	public List<Body> OtherBodies;

	private bool _isColliding;

	// Called when the node enters the scene tree for the first time.

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, Radius, new Color(255,255,255));
	}

	public override void _Ready()
	{
		Sprite.Visible = false;
		//Radius = 100;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public Vector2 TestAcceleration(Vector2 testPos)
	{
		Vector2 acc = Vector2.Zero;

		foreach (Body oB in OtherBodies)
		{
			Vector2 toOther = oB.Position - testPos;
            float dist = toOther.Length();
            float minDist = Radius + oB.Radius;

            // Add acceleration from force exerted by other body
            // r'' = -G*m2*(r1 - r2)/|r1-r2|^3 - G*m3*(r1-r3)/|r1-r3|^3
            acc += (float)(Simulator.G * oB.Mass) * (toOther)
			/ (float)Math.Pow(dist + softening, 3.0);
        }

		return acc;
	}

	public void HandleCollisions()
	{
		foreach (Body oB in OtherBodies)
		{
            Vector2 toOther = oB.Position - Position;
            float dist = toOther.Length();
            float minDist = Radius + oB.Radius;

            // Check if collision will occur between this and the other body
            //|r2 - r1|^2 <= (r1+r2)^2 then overlap
            if (dist <= minDist && dist > 0)
            {
                _isColliding = true;
                //GD.Print($"{Name} is Colliding!");
                // v_2 = v_1 + v_2/1
                Vector2 relativeVelocity = oB.Velocity - Velocity;
                Vector2 PrevVelocity = Velocity;
                Vector2 normalDir = toOther.Normalized();


                if (relativeVelocity.Dot(normalDir) < 0)
                {
                    // Project velocity along normal 
                    Vector2 normalVelocity = relativeVelocity.Dot(normalDir) * normalDir;

                    // along the normal: 
                    // v_f = -e*v_i
                    // J = v_f - v_i
                    // J = -e*v_i - v_i
                    // -> J = -(1+e)*v_i
                    Vector2 Impulse = -(1 + Restitution) * normalVelocity;
                    float MassRatio = (oB.Mass) / (Mass + oB.Mass);
                    Velocity = -Impulse * MassRatio;

                    float overlap = minDist - dist;
                    float massRatio = oB.Mass / (Mass + oB.Mass);

                    Position -= normalDir * (overlap * massRatio);
                }

            }
        }


	}
	public void ComputePhysics()
	{
		Acceleration = TestAcceleration(Position);
		HandleCollisions();

	}

	public void Update(double delta)
	{
		GD.Print($"{Name} has mass {Mass}");
		GD.Print($"{Name} new Velocity is {NewVelocity.Length()}, {NewVelocity.Angle() / float.Pi * 180.0f} degs");
		GD.Print($"{Name} new Acceleration is {Acceleration.Length()}, {Acceleration.Angle() / float.Pi * 180.0f} degs");

		// Euler Integraiton
		Velocity += Acceleration * (float)delta * Simulator.TimeScale;
		Position += Velocity * (float)delta * Simulator.TimeScale;

	}
}