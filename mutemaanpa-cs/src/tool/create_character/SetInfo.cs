namespace Mutemaanpa;

using Godot;

public partial class SetInfo : MarginContainer
{
    internal Character? character;
    [Export]
    private LineEdit? CharacterName;

    [Export]
    private OptionButton? Origin;

    [Export]
    private Button? BackButton;

    public override void _Ready()
    {
        CharacterName!.TextChanged += (newText) =>
        {
            Character.NAME[character!] = newText;
            Character.ORIGIN[character!] = (Origin)Origin!.Selected;
        };
        BackButton!.Pressed += () =>
        {
            Router.Of(this).Pop();
        };
    }
}
