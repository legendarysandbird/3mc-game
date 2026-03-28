using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PlayerSpawner : Node
{
    [Export] private Array<Node2D>? _spawnPoints;

    public override void _Ready()
    {
        _spawnPoints.NotNull(nameof(_spawnPoints));
    }
}
