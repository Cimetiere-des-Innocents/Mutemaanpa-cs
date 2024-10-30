namespace Mutemaanpa;

public class Hp
{
    public static readonly EntityDataKey<double> HP = new("hp", EntityDataSerializers.DOUBLE);
    public static readonly EntityDataKey<double> MAX_HP = new("max_hp", EntityDataSerializers.DOUBLE);
    public static void DefineHp(EntityDataBuilder builder, double hp = 0, double maxHp = 0)
    {
        builder.define(HP, hp);
        builder.define(MAX_HP, maxHp);
    }
}