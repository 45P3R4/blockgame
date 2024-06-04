using System.Text;
using Godot;

public partial class Interaction : RayCast3D
{
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("use"))
			if (IsColliding())
			{
				Node3D collider = GetCollider() as Node3D;
				Chunk ch = collider.GetParent().GetParent() as Chunk;
				Vector3 pos = GetCollisionPoint();
				Vector3 normal = GetCollisionNormal();

				pos -= ch.Offset;
				ch.SpawnBlock((Vector3I)(pos + normal/2));
			}

		if (Input.IsActionJustPressed("fire"))
			if (IsColliding())
			{
				Node3D collider = GetCollider() as Node3D;
				Chunk ch = collider.GetParent().GetParent() as Chunk;
				Vector3 pos = GetCollisionPoint();
				Vector3 normal = GetCollisionNormal();

				pos -= ch.Offset;
				ch.BreakBlock((Vector3I)(pos - normal/2));
			}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().ChangeSceneToFile("res://UI/Menu.tscn");
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
	}
}
