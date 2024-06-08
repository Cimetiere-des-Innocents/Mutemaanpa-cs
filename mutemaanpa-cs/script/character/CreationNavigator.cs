namespace Mutemaanpa;

using System;
using Godot;

public partial class CreationNavigator : PanelContainer
{
	private CharacterManager? CharacterManager;

	[Export]
	private SetInfo? SetInfo;

	[Export]
	private SetAbility? SetAbility;

	public override void _Ready()
	{
		base._Ready();
		CharacterManager = Provider.Of<CharacterManager>(this);
		SetAbility!.FinishButton!.Pressed += () =>
		{
			CharacterManager.RegisterCharacter(
				SetInfo!.GetCharacterStat(),
				SetAbility!.GetAbility(),
				null,
				Guid.NewGuid()
			);
			CharacterManager.Store();
			Router.Of(this).Overwrite("/intermission/opening");
		};
	}

}
