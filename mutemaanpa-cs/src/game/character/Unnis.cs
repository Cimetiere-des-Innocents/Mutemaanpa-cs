using Godot;

namespace Mutemaanpa;

[Entity]
public partial class Unnis : Character
{
    public static readonly EntityType<Unnis> TYPE = EntityTypeUtil.FromScene<Unnis>("unnis", "res://scene/game/character/Unnis.tscn");

}
