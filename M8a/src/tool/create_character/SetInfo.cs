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

    public CharacterStat GetCharacterStat() => new()
    {
        Name = CharacterName!.Text,
        Hp = 0.0f,
        Mp = 0,
        Origin = (Origin)Origin!.Selected
    };

    public override void _Ready()
    {
        base._Ready();
        BackButton!.Pressed += () =>
        {
            Router.Of(this).Pop();
        };
    }
}

