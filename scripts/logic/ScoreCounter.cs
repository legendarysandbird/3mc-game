using Godot;

public partial class ScoreCounter : Label
{
	[Export] private EnemyDirector _enemyDirector;

	private int _score;

	public override void _Ready()
	{
		_enemyDirector.EnemyDeath += OnEnemyDeath;

		Text = _score.ToString();
	}

	private void OnEnemyDeath()
	{
		_score++;
		Text = _score.ToString();
	}
}
