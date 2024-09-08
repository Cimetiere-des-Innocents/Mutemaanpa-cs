namespace Mutemaanpa;

using Godot;

/// <summary>
/// Character creation is a scene that helps to create new character. It writes new character to
/// a disk file called "character_created.json". That file path can be override.
/// </summary>
public partial class CharacterCreation : Node3D
{

	[Export]
	CreationNavigator? creationNavigator;

	Character? character;

	string OutputPath = "character_created.json";

	public static CharacterCreation NewCreator(string output = "character_created.json")
	{
		var node = ResourceLoader
					.Load<PackedScene>("res://scene/tool/create_character/character_creation.tscn")
					.Instantiate<CharacterCreation>();
		node.OutputPath = output;
		return node;
	}

	public override void _EnterTree()
	{
		var state = new CreatingCharacter();
		AddChild(state);
		character = state;
		creationNavigator!.character = state;
		creationNavigator.SetFinishCallback(() =>
		{
			var serialized = EntityExt.SerializeAll(character!);
			var dir = Catalog.Pwd(this)!;
			using var file = FileAccess.Open($"{dir.GetCurrentDir()}/created_character.json", FileAccess.ModeFlags.Write);
			file.StoreString(Json.Stringify(serialized));
		});
	}
}
