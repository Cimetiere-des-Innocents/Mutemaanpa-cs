
using Godot;

namespace Mutemaanpa;
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

