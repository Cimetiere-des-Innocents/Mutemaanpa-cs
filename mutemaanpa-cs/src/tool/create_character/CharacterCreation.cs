
using System;
using Godot;

namespace Mutemaanpa;
public partial class CharacterCreation : Node3D
{

    [Export]
    CreationNavigator? creationNavigator;

    public static CharacterCreation CreateCharacterCreation(Action<CharacterCreation, CharacterStat, CharacterAbility> callback)
    {
        var node = ResourceLoader.Load<PackedScene>("res://scene/tool/create_character/character_creation.tscn")
            .Instantiate<CharacterCreation>();
        node.creationNavigator!.SetFinishCallback((stat, ability) =>
        {
            callback(node, stat, ability);
        });
        return node;
    }
}
