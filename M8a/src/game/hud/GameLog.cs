namespace Mutemaanpa;

using Godot;

public partial class GameLog : Control
{
    [Export]
    RichTextLabel? LogBox;

    public override void _Ready()
    {
        EventBus.Subscribe<HitEvent>(HitHandler);
        EventBus.Subscribe<DeadEvent>(DeadHandler);
    }

    private void HitHandler(HitEvent hit)
    {
        LogBox?.AppendText($"{hit.Victim} was hit by damage {hit.Damage}.\n");
    }

    private void DeadHandler(DeadEvent dead)
    {
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            GameOverHandler();
        }
    }

    private void GameOverHandler()
    {
        EventBus.Unsubscribe<HitEvent>(HitHandler);
        EventBus.Unsubscribe<DeadEvent>(DeadHandler);
    }
}
