namespace Mutemaanpa;

using Godot;

public partial class SetInfo : MarginContainer
{
	[Export]
	private LineEdit? CharacterName;

	[Export]
	private OptionButton? Origin;

	public CharacterStat GetCharacterStat() => new(
		CharacterName!.Text,
		0.0f,
		0,
		(Origin) Origin!.Selected
	);
}

