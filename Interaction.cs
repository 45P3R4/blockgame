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
				Vector3I pos = (Vector3I)GetCollisionPoint();

				GD.Print(GetCollisionPoint());

				pos -= ch.Offset;
				ch.SpawnBlock(pos);
			}

		if (Input.IsActionJustPressed("fire"))
			if (IsColliding())
			{
				Node3D collider = GetCollider() as Node3D;
				Chunk ch = collider.GetParent().GetParent() as Chunk;
				Vector3 pos = GetCollisionPoint();
				Vector3 normal = GetCollisionNormal();

				GD.Print(GetCollisionPoint());

				pos -= ch.Offset;
				ch.BreakBlock((Vector3I)(pos));
			}
	}
}
