using Godot;

[GlobalClass]
public partial class Player : CharacterBody2D
{
    private const string LEFT_ACTION = "left";
    private const string RIGHT_ACTION = "right";
    private const string FIRE_ACTION = "fire";
    private const string JUMP_ACTION = "jump";
    private const string AIM_LEFT_ACTION = "aim_left";
    private const string AIM_RIGHT_ACTION = "aim_right";
    private const string AIM_UP_ACTION = "aim_up";
    private const string AIM_DOWN_ACTION = "aim_down";

    private readonly float _gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    [Export] public int _playerNumber;
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
    private AnimatedSprite2D? _animator;

    private Vector2 _mousePosition;
    private Vector2 _stickDirection;
    private float _animatorXScaleCache;

    public Vector2 ProjectileDirection { get; private set; }

    public override void _Ready()
    {
        _playerNumber.IsInitialized(nameof(_playerNumber));
        _armNode = GetNode<Node2D>("AnimatedSprite2D/Arm").NotNull(nameof(_projectileSpawnNode));
        _projectileSpawnNode = GetNode<Node2D>("AnimatedSprite2D/Arm/ProjectileSpawnPoint").NotNull(nameof(_projectileSpawnNode));
        _hitbox = GetNode<Area2D>("Hitbox").NotNull(nameof(_hitbox));
        _healthPool = GetNode<Health>("Health").NotNull(nameof(_healthPool));
        _ammoPool = GetNode<AmmoPool>("AmmoPool").NotNull(nameof(_ammoPool));
        _jumpTimer = GetNode<Timer>("JumpTimer").NotNull(nameof(_jumpTimer));
        _gunTimer = GetNode<Timer>("GunTimer").NotNull(nameof(_gunTimer));
        _animator = GetNode<AnimatedSprite2D>("AnimatedSprite2D").NotNull(nameof(_animator));

        _mousePosition = InputManager.Instance.GetGlobalMousePosition(_playerNumber, this);
        ProjectileDirection = Vector2.Right;
        _animatorXScaleCache = _animator.Scale.X;

        _hitbox.BodyEntered += OnHitboxBodyEntered;
        _healthPool.HealthEmpty += OnHealthPoolEmpty;
    }

    public override void _PhysicsProcess(double delta)
    {
        MovePlayer(delta);
        HandleShooting();
        UpdateAnimations(delta);
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

        if (InputManager.Instance.IsActionPressed(_playerNumber, JUMP_ACTION) && IsJumpEligible())
        {
            y -= _jumpVelocity;
            _jumpTimer.Start();
        }

        x = InputManager.Instance.GetAxis(_playerNumber, LEFT_ACTION, RIGHT_ACTION) * _moveSpeed;

        Velocity = new Vector2(x, y);
        MoveAndSlide();
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

        if (!InputManager.Instance.IsActionPressed(_playerNumber, FIRE_ACTION) || _gunTimer.TimeLeft > 0 || _ammoPool.AmmoPoolValue < 1)
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

        var mousePosition = InputManager.Instance.GetGlobalMousePosition(_playerNumber, this);
        if (_mousePosition != mousePosition)
        {
            projectileDirection = mousePosition - sourcePosition;
            _mousePosition = mousePosition;
        }

        var stickDirection = InputManager.Instance.GetVector(_playerNumber, AIM_LEFT_ACTION, AIM_RIGHT_ACTION, AIM_UP_ACTION, AIM_DOWN_ACTION);
        if (_stickDirection != stickDirection)
        {
            projectileDirection = stickDirection;
            _stickDirection = stickDirection;
        }

        return projectileDirection.Normalized();
    }

    private void UpdateAnimations(double delta)
    {
        RotatePlayer(delta);
        UpdateAnimator();
        AnimateArm();
    }

    private void RotatePlayer(double delta)
    {
        float targetRotation = IsOnFloor() ? GetFloorNormal().X : 0;
        Rotation = Mathf.Lerp(Rotation, targetRotation, (float)delta * _rotationSpeed);
    }

    private void UpdateAnimator()
    {
        _animator.NotNull(nameof(_animator));

        bool isLeftPressed = InputManager.Instance.IsActionPressed(_playerNumber, LEFT_ACTION);
        bool isRightPressed = InputManager.Instance.IsActionPressed(_playerNumber, RIGHT_ACTION);
        if (isLeftPressed == isRightPressed)
        {
            _animator.Animation = "idle";
            return;
        }

        _animator.Animation = "walk";

        bool movingLeft = isLeftPressed;
        _animator.Scale = _animator.Scale with { X = _animatorXScaleCache * (movingLeft ? -1 : 1) };
    }

    private void AnimateArm()
    {
        _armNode.NotNull(nameof(_armNode));
        _armNode.LookAt(_armNode.GlobalPosition + ProjectileDirection);
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
