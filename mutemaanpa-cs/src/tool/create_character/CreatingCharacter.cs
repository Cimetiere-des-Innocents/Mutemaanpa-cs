namespace Mutemaanpa;

[Entity]
public partial class CreatingCharacter : Character
{
    public static readonly EntityType<CreatingCharacter> TYPE = EntityTypeUtil.Create("creating_character", () => new CreatingCharacter());
}
