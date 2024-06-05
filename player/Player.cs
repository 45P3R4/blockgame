using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	private Node3D Camera;

	private const float Speed = 5.0f;
	private const float JumpVelocity = 7f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Camera.Basis * new Vector3(inputDir.X, 0, inputDir.Y));
		direction = (direction * new Vector3(1,0,1)).Normalized();
		
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

	    public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().Paused = true;
				Node menuScene = ResourceLoader.Load<PackedScene>("res://UI/GameMenu.tscn").Instantiate();
				GetTree().Root.AddChild(menuScene);
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
	}
}
