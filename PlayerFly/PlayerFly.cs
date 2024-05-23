using Godot;
using System;

public partial class PlayerFly : CharacterBody3D
{
	[Export]
	private Camera camera;

	public const float Speed = 15.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (Input.IsActionPressed("jump"))
			velocity.Y = Speed;
		else if (Input.IsActionPressed("crouch"))
			velocity.Y = -Speed;
		else
			velocity.Y = 0;

		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (camera.Basis * new Vector3(inputDir.X, 0, inputDir.Y));
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
