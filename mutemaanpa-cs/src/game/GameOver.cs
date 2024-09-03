
using System;
using Godot;

namespace Mutemaanpa;
public partial class GameOver : Control
{
    [Export]
    Button? ToTitle;

    [Export]
    Button? LoadGame;

    public static GameOver CreateGameOver(Action ToTitleAction, Action LoadGameAction)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/game/game_over.tscn")
            .Instantiate<GameOver>();
        node.ToTitle!.Pressed += ToTitleAction;
        node.LoadGame!.Pressed += LoadGameAction;
        return node;
    }
}
