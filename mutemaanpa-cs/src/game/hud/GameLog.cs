namespace Mutemaanpa;

using Godot;

public partial class GameLog : Control
{
    [Export]
    RichTextLabel? LogBox;

    public override void _Ready()
    {
        EventBus.Subscribe((HitEvent hit) =>
        {
            LogBox!.AppendText($"{hit.Victim} was hit by damage {hit.Damage}.");
        });
        EventBus.Subscribe((DeadEvent dead) =>
        {
            LogBox!.AppendText($"{dead.Victim} is dead!");
        });
    }
}
