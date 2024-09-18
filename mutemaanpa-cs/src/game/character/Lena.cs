namespace Mutemaanpa;

[Entity]
public partial class Lena : Character
{
    public static readonly EntityType<Lena> TYPE = EntityTypeUtil.FromScene<Lena>("lena", "res://scene/game/character/Lena.tscn");

}
