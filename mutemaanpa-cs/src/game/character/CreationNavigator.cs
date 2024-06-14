namespace Mutemaanpa;

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
            var metadata = Provider.Of<MetadataManager>(this);
            var gameMain = GameMain.CreateGameMain(saveDatabase,
                                                   metadata,
                                                   SetInfo!.GetCharacterStat(),
                                                   SetAbility!.GetAbility());
            Router.Of(this).Overwrite(gameMain);
        };
    }

}
