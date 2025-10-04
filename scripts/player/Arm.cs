using Godot;

public partial class Arm : Node2D
{
    public override void _Process(double delta)
    {
        LookAt(GetGlobalMousePosition());
    }
}
