namespace Mutemaanpa;

public class Hp
{
    public static EntityDataKey<double> HP = new("hp", EntityDataSerializers.DOUBLE);
    public static EntityDataKey<double> MAX_HP = new("max_hp", EntityDataSerializers.DOUBLE);
    public static void DefineHp(EntityDataBuilder builder, double hp = 0, double maxHp = 0)
    {
        builder.define(HP, hp);
        builder.define(MAX_HP, maxHp);
    }
}