namespace Mutemaanpa;

using Godot;

public partial class OpeningScene : PanelContainer
{
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ui_accept"))
        {
            Router.Of(this).Overwrite(World.CreateWorld());
        }
    }
}
