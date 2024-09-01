namespace Mutemaanpa;

using Godot;
using Kernel;

public partial class CharacterCreation : Node3D
{

    [Export]
    CreationNavigator? creationNavigator;

    public static CharacterCreation CreateCharacterCreation(Session session)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/create_character/character_creation.tscn")
            .Instantiate<CharacterCreation>();
        void action(CharacterStat stat, Name name)
        {
            CommandModule.createUser(session, stat, name);
            Router.Of(node).Pop();
        }
        node.creationNavigator!.SetFinishCallback(action);
        return node;
    }
}
