namespace Mutemaanpa;

using System;
using Godot;

public partial class CreationNavigator : PanelContainer
{
    [Export]
    private SetInfo? SetInfo;

    [Export]
    private SetAbility? SetAbility;
    internal Character? character;

    public void SetFinishCallback(Action callback)
    {
        SetAbility!.FinishButton!.Pressed += callback;
    }

    public override void _EnterTree()
    {
        SetInfo!.character = character;
        SetAbility!.character = character;
    }
}
