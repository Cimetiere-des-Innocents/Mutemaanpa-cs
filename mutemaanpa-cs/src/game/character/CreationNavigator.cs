namespace Mutemaanpa;

using System;
using Godot;

public partial class CreationNavigator : PanelContainer
{

    [Export]
    private SetInfo? SetInfo;

    [Export]
    private SetAbility? SetAbility;

    public override void _Ready()
    {
        base._Ready();
        SetAbility!.FinishButton!.Pressed += () =>
        {
            var saveDatabase = Provider.Of<SaveDatabase>(this);
            var saveUuid = saveDatabase.NewSave();

            var metadata = Provider.Of<MetadataManager>(this);
            metadata.CurrentSave = saveUuid;

            var characterDb = new CharacterDatabase($"Data Source=m8a_save_{saveUuid}.db");
            characterDb.InitDatabase();
            Provider.Add(this, characterDb);

            var characterManager = new CharacterManager(characterDb);
            Provider.Add(this, characterManager);

            characterManager.RegisterCharacter(
                SetInfo!.GetCharacterStat(),
                SetAbility!.GetAbility(),
                null,
                Guid.NewGuid()
            );
            characterManager.Store();

            var gameMain = GameMain.CreateGameMain(characterManager);

            Router.Of(this).Overwrite(gameMain);
        };
    }

}
