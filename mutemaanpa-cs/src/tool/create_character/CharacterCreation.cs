namespace Mutemaanpa;

using System;
using Godot;

public partial class CharacterCreation : Node3D
{

    [Export]
    CreationNavigator? creationNavigator;

    public static CharacterCreation CreateCharacterCreation(SaveDatabase saveDatabase,
                                                            MetadataManager metadata)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/create_character/character_creation.tscn")
            .Instantiate<CharacterCreation>();
        node.creationNavigator!.SetFinishCallback((stat, ability) =>
        {
            var saveUuid = saveDatabase.NewSave();
            metadata.CurrentSave = saveUuid;
            var characterDb = new CharacterDatabase($"Data Source=m8a_save_{saveUuid}.db");
            characterDb.InitDatabase();
            var characterManager = new CharacterManager(characterDb);

            characterManager.RegisterCharacter(
                stat,
                ability,
                null,
                Guid.NewGuid()
            );
            characterManager.Store();
            var gameMain = GameMain.CreateGameMain(characterManager);
            Router.Of(node).Overwrite(gameMain);
        });
        return node;
    }
}
