namespace Mutemaanpa;

using System;
using Godot;
using Kernel;

public partial class CreationNavigator : PanelContainer
{

    [Export]
    private SetInfo? SetInfo;

    [Export]
    private SetAbility? SetAbility;

    public void SetFinishCallback(Action<CharacterStat, Name> action)
    {
        SetAbility!.FinishButton!.Pressed += () => action(SetAbility!.GetAbility(),
                                                          SetInfo!.GetCharacterName());
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
