using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Level : Node
{
    public List<Player> Players { get; private set; } = new();

    [Export] private Array<Node2D>? _playerSpawnPoints;
    [Export] private Node? _playersAnchor;
    [Export] private float _moveSpeed;
    [Export] private float _jumpVelocity;
    [Export] private int _rotationSpeed;
    [Export] private float _projectileSpeed;

    public override void _Ready()
    {
        _moveSpeed.IsInitialized(nameof(_moveSpeed));
        _jumpVelocity.IsInitialized(nameof(_jumpVelocity));
        _rotationSpeed.IsInitialized(nameof(_rotationSpeed));
        _projectileSpeed.IsInitialized(nameof(_projectileSpeed));
        _playerSpawnPoints.NotNull(nameof(_playerSpawnPoints));
        _playersAnchor.NotNull(nameof(_playersAnchor));
    }

    public void Init(int playerCount)
    {
        _playerSpawnPoints.NotNull(nameof(_playerSpawnPoints));
        _playersAnchor.NotNull(nameof(_playersAnchor));

        int spawnPointCount = _playerSpawnPoints.Count;
        if (spawnPointCount < playerCount)
        {
            throw new ArgumentOutOfRangeException(nameof(playerCount), $"Trying to spawn {playerCount} players, but only {spawnPointCount} spawn points are defined!");
        }

        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            Vector2 startingPosition = _playerSpawnPoints[playerIndex].GlobalPosition;
            var player = Player.Create(playerIndex + 1, startingPosition, _moveSpeed, _jumpVelocity, _rotationSpeed, _projectileSpeed);
            _playersAnchor.AddChild(player);
            Players.Add(player);
        }
    }
}
