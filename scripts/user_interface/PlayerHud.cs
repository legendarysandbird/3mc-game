using System;
using Godot;

public partial class PlayerHud : Control
{

    [Export] public Player? Player { get; private set; }

    public override void _Ready()
    {
        Player.NotNull(nameof(Player));
    }

}
