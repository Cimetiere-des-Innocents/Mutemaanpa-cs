namespace Mutemaanpa;

using Godot;

public partial class CharacterCreation : Node3D
{

    [Export]
    CreationNavigator? creationNavigator;

    Character? character;

    public override void _EnterTree()
    {
        var state = new Character();
        character = state;
        creationNavigator!.character = state;
        creationNavigator.SetFinishCallback(() =>
        {
            var dict = new SaveDict();
            (character as Entity<CharacterBody3D>).Save(dict);
            // EntityExt.SaveExt(character!, dict);
            GD.Print(dict);
        });
    }
}
