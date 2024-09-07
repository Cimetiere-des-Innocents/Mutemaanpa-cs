namespace Mutemaanpa;

using Godot;

public partial class CharacterCreation : Node3D
{

    [Export]
    CreationNavigator? creationNavigator;

    Character? character;

    public override void _EnterTree()
    {
        var state = new CreatingCharacter();
        AddChild(state);
        character = state;
        creationNavigator!.character = state;
        creationNavigator.SetFinishCallback(() =>
        {
            var serialized = EntityExt.SerializeAll(character!);
        });
    }
}
