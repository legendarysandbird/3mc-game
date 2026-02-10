using Godot;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    private const float MoveSpeed = 200.0f;
    private const float JumpVelocity = 700.0f;

    private readonly float _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    [Export] private int _rotationSpeed = 30;
    [Export] private float _projectileSpeed = 300.0f;

    private Node2D? _projectileSpawnNode;
    private Area2D? _hitbox;
    private Health? _healthPool;
    private Timer? _jumpTimer;
    private Timer? _gunTimer;
    private Vector2 _mousePosition;
    private Vector2 _stickDirection;

    public Vector2 ProjectileDirection { get; private set; }


    public override void _Ready()
    {
        _projectileSpawnNode = GetNode<Node2D>("AnimatedSprite2D/Arm/ProjectileSpawnPoint").NotNull(nameof(_projectileSpawnNode));
        _hitbox = GetNode<Area2D>("Hitbox").NotNull(nameof(_hitbox));
        _healthPool = GetNode<Health>("Health").NotNull(nameof(_healthPool));
        _jumpTimer = GetNode<Timer>("JumpTimer").NotNull(nameof(_jumpTimer));
        _gunTimer = GetNode<Timer>("GunTimer").NotNull(nameof(_gunTimer));
        _mousePosition = GetGlobalMousePosition();
        ProjectileDirection = Vector2.Right;

        _hitbox.BodyEntered += OnHitboxBodyEntered;
        _healthPool.HealthEmpty += OnHealthPoolEmpty;
    }

    public override void _PhysicsProcess(double delta)
    {
        MovePlayer(delta);
        HandleShooting();
    }

    private void RotatePlayer(double delta)
    {
        float targetRotation = IsOnFloor() ? GetFloorNormal().X : 0;
        Rotation = Mathf.Lerp(Rotation, targetRotation, (float)delta * _rotationSpeed);
    }

    private bool IsJumpEligible()
    {
        _jumpTimer.NotNull(nameof(_jumpTimer));

        return IsOnFloor() && _jumpTimer.TimeLeft == 0;
    }

    private void MovePlayer(double delta)
    {
        _jumpTimer.NotNull(nameof(_jumpTimer));

        float x = 0.0f;
        float y = Velocity.Y;

        if (!IsOnFloor())
        {
            y += _gravity * (float)delta;
        }

        if (Input.IsActionPressed("player_jump") && IsJumpEligible())
        {
            y -= JumpVelocity;
            _jumpTimer.Start();
        }

        x = Input.GetAxis("player_left", "player_right") * MoveSpeed;

        Velocity = new Vector2(x, y);
        MoveAndSlide();
        RotatePlayer(delta);
    }

    private void HandleShooting()
    {
        _gunTimer.NotNull(nameof(_gunTimer));
        _projectileSpawnNode.NotNull(nameof(_projectileSpawnNode));

        Vector2 spawnPosition = _projectileSpawnNode.GlobalPosition;
        Vector2 projectileDirection = GetProjectileDirection(spawnPosition);
        if (projectileDirection != Vector2.Zero)
        {
            ProjectileDirection = projectileDirection;
        }

        if (!Input.IsActionPressed("fire") || _gunTimer.TimeLeft > 0)
        {
            return;
        }

        _gunTimer.Start();

        Projectile projectile = Projectile.Create(_projectileSpawnNode.GlobalPosition, ProjectileDirection * _projectileSpeed);

        GetTree().Root.AddChild(projectile);
    }

    private Vector2 GetProjectileDirection(Vector2 spawnPosition)
    {
        var projectileDirection = Vector2.Zero;

        var mousePosition = GetGlobalMousePosition();
        if (_mousePosition != mousePosition)
        {
            projectileDirection = mousePosition - spawnPosition;
            _mousePosition = mousePosition;
        }

        var stickDirection = Input.GetVector("player_aim_left", "player_aim_right", "player_aim_up", "player_aim_down");
        if (_stickDirection != stickDirection)
        {
            projectileDirection = stickDirection;
            _stickDirection = stickDirection;
        }

        return projectileDirection.Normalized();
    }

    private void OnHitboxBodyEntered(Node2D body)
    {
        _healthPool.NotNull(nameof(_healthPool));

        if (body is Enemy enemy)
        {
            _healthPool.ChangeHealth(-1);
        }
    }

    private void OnHealthPoolEmpty()
    {
        QueueFree();
    }
}
