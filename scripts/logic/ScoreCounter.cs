using Godot;

// Temporary hack method of counting enemies killed.
// Score counting logic needs to be refactored into something like a "PlayerStats" node.
// Until then, expect hacks like line 18. 

public partial class ScoreCounter : Label
{
    [Export] private PlayerHud? _playerHud;

    private EnemyDirector? _enemyDirector;

    private int _score;

    public override void _Ready()
    {

        _enemyDirector = _playerHud.NotNull(nameof(_playerHud)).GetParent().GetNode("EnemyDirector") as EnemyDirector;

        _enemyDirector.NotNull(nameof(_enemyDirector));

        _enemyDirector.EnemyDeath += OnEnemyDeath;

        Text = _score.ToString();
    }

    private void OnEnemyDeath()
    {
        _score++;
        Text = _score.ToString();
    }
}
