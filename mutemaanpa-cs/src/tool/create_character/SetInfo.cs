namespace Mutemaanpa;

using Godot;

public partial class SetInfo : MarginContainer
{
    [Export]
    private LineEdit? CharacterName;

    [Export]
    private OptionButton? Origin;

    [Export]
    private Button? BackButton;

    public CharacterStat GetCharacterStat() => new(
        CharacterName!.Text,
        0.0f,
        0,
        (Origin)Origin!.Selected
    );

    public override void _Ready()
    {
        base._Ready();
        BackButton!.Pressed += () =>
        {
            Router.Of(this).Pop();
        };
    }
}

