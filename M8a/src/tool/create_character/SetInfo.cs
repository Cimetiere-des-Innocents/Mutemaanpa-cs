namespace Mutemaanpa;

using Godot;

public partial class SetInfo : MarginContainer
{
    [Export]
    private LineEdit? CharacterName;

    [Export]
    private Button? BackButton;

    public Kernel.Name GetCharacterName() => new()
    {
        Name = CharacterName!.Text,
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

