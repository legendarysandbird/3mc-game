using Godot;

[GlobalClass]
public partial class CharacterController3D : CharacterBody3D
{
    private readonly float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    [Export] private float _moveSpeed = 200.0f;
    [Export] private int _rotationSpeed = 30;
    [Export] private float _projectileSpeed = 300.0f;

    private Node3D? _projectileSpawnNode;
    private Area3D? _hitbox;
    private Health? _healthPool;
    private Timer? _gunTimer;

    public override void _Ready()
    {
        _projectileSpawnNode = GetNode<Node3D>("Body Animation/Arm Animation/Projectile Anchor").NotNull(nameof(_projectileSpawnNode));
        _hitbox = GetNode<Area3D>("Hitbox").NotNull(nameof(_hitbox));
        _healthPool = GetNode<Health>("Health").NotNull(nameof(_healthPool));
        _gunTimer = GetNode<Timer>("GunTimer").NotNull(nameof(_gunTimer));

        _hitbox.BodyEntered += OnHitboxBodyEntered;
        _healthPool.HealthEmpty += OnHealthPoolEmpty;
    }

    public override void _PhysicsProcess(double delta)
    {
        MovePlayer(delta);
        HandleShooting();
    }

    private void MovePlayer(double delta)
    {
        float x = 0.0f;
        float z = 0.0f;
        float y = Velocity.Y;

        if (!IsOnFloor())
        {
            y -= _gravity * (float)delta;
        }

        if (Input.IsActionPressed("player_left"))
        {
            x -= _moveSpeed;
        }

        if (Input.IsActionPressed("player_right"))
        {
            x += _moveSpeed;
        }

        if (Input.IsActionPressed("player_up"))
        {
            z -= _moveSpeed;
        }

        if (Input.IsActionPressed("player_down"))
        {
            z += _moveSpeed;
        }

        Velocity = new Vector3(x, y, z);
        MoveAndSlide();
    }

    private void HandleShooting()
    {
        _gunTimer.NotNull(nameof(_gunTimer));
        _projectileSpawnNode.NotNull(nameof(_projectileSpawnNode));

        if (!Input.IsActionPressed("fire") || _gunTimer.TimeLeft > 0)
        {
            return;
        }

        _gunTimer.Start();

        Vector3 spawnPosition = _projectileSpawnNode.GlobalPosition;
        Vector2 mousePosition = GetViewport().GetMousePosition();
        Vector3 projectileDirection = new Vector3(mousePosition.X, 0.0f, mousePosition.Y) - Position;
        Projectile3D projectile = Projectile3D.Create(_projectileSpawnNode.GlobalPosition, projectileDirection * _projectileSpeed);

        GetTree().Root.AddChild(projectile);
    }

    private void OnHitboxBodyEntered(Node3D body)
    {
        _healthPool.NotNull(nameof(_healthPool));

        // TODO Change this back to Enemy3D
        if (body is Node enemy)
        {
            _healthPool.ChangeHealth(-1);
        }
    }

    private void OnHealthPoolEmpty()
    {
        QueueFree();
    }
}
