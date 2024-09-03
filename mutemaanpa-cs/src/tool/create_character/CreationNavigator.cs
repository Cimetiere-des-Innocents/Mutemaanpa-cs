
using System;
using Godot;

namespace Mutemaanpa;
public partial class CreationNavigator : PanelContainer
{

    [Export]
    private SetInfo? SetInfo;

    [Export]
    private SetAbility? SetAbility;

    public void SetFinishCallback(Action<CharacterStat, CharacterAbility> action)
    {
        SetAbility!.FinishButton!.Pressed += () => action(SetInfo!.GetCharacterStat(),
                                                          SetAbility!.GetAbility());
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
