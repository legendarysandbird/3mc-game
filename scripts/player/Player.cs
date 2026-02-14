using Godot;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    private readonly float _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    [Export] private float _moveSpeed;
    [Export] private float _jumpVelocity;
    [Export] private int _rotationSpeed;
    [Export] private float _projectileSpeed;

    private Node2D? _armNode;
    private Node2D? _projectileSpawnNode;
    private Area2D? _hitbox;
    private Health? _healthPool;
    private AmmoPool? _ammoPool;
    private Timer? _jumpTimer;
    private Timer? _gunTimer;
    private Vector2 _mousePosition;
    private Vector2 _stickDirection;

    public Vector2 ProjectileDirection { get; private set; }


    public override void _Ready()
    {
        _armNode = GetNode<Node2D>("AnimatedSprite2D/Arm").NotNull(nameof(_projectileSpawnNode));
        _projectileSpawnNode = GetNode<Node2D>("AnimatedSprite2D/Arm/ProjectileSpawnPoint").NotNull(nameof(_projectileSpawnNode));
        _hitbox = GetNode<Area2D>("Hitbox").NotNull(nameof(_hitbox));
        _healthPool = GetNode<Health>("Health").NotNull(nameof(_healthPool));
        _ammoPool = GetNode<AmmoPool>("AmmoPool").NotNull(nameof(_ammoPool));
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
            y -= _jumpVelocity;
            _jumpTimer.Start();
        }

        x = Input.GetAxis("player_left", "player_right") * _moveSpeed;

        Velocity = new Vector2(x, y);
        MoveAndSlide();
        RotatePlayer(delta);
    }

    private void HandleShooting()
    {
        _gunTimer.NotNull(nameof(_gunTimer));
        _ammoPool.NotNull(nameof(_ammoPool));
        _armNode.NotNull(nameof(_armNode));
        _projectileSpawnNode.NotNull(nameof(_projectileSpawnNode));

        Vector2 sourcePosition = _armNode.GlobalPosition;
        Vector2 projectileDirection = GetProjectileDirection(sourcePosition);
        if (projectileDirection != Vector2.Zero)
        {
            ProjectileDirection = projectileDirection;
        }

        if (!Input.IsActionPressed("fire") || _gunTimer.TimeLeft > 0 || _ammoPool.AmmoPoolValue < 1)
        {
            return;
        }

        _gunTimer.Start();

        Projectile projectile = Projectile.Create(_projectileSpawnNode.GlobalPosition, ProjectileDirection * _projectileSpeed);

        GetTree().Root.AddChild(projectile);
        _ammoPool.ChangeAmmoPoolValue(-1);
    }

    private Vector2 GetProjectileDirection(Vector2 sourcePosition)
    {
        var projectileDirection = Vector2.Zero;

        var mousePosition = GetGlobalMousePosition();
        if (_mousePosition != mousePosition)
        {
            projectileDirection = mousePosition - sourcePosition;
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
