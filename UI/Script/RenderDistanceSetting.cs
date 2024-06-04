using Godot;
using System;

public partial class RenderDistanceSetting : SpinBox
{
    public override void _Ready()
    {
        Value = ChunkRenderer.RenderDistance;
    }

    public override void _ValueChanged(double newValue)
    {
        ChunkRenderer.RenderDistance = (int)newValue;
    }
}
