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
        };
        Origin!.ItemSelected += (long selected) =>
        {
            Character.ORIGIN[character!] = (Origin)selected;
        };
        BackButton!.Pressed += () =>
        {
            Router.Of(this).Pop();
        };
    }
}
