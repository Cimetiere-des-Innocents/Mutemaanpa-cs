using System;
using Godot;

namespace Mutemaanpa;

public enum Origin
{
    SOLDIER,
    CLERIC,
    ROGUE,
    HUNTER,
    BUREAUCRAT,
    SPY,
    NAMELESS_ONE
}

public partial class Character : EntityCharacterBody3D
{
    [Export]
    private int strength = 0;
    [Export]
    private int stamina = 0;
    [Export]
    private int dexterity = 0;
    [Export]
    private int constitution = 0;
    [Export]
    private int intelligence = 0;
    [Export]
    private int wisdom = 0;
    [Export]
    private string name = "";
    [Export]
    private Origin origin = Origin.SOLDIER;
    [Export]
    private double maxHp = 0;

    public static EntityDataKey<int> STRENGTH = new("strength", EntityDataSerializers.INT);
    public static EntityDataKey<int> STAMINA = new("stamina", EntityDataSerializers.INT);
    public static EntityDataKey<int> DEXTERITY = new("dexterity", EntityDataSerializers.INT);
    public static EntityDataKey<int> CONSTITUTION = new("constitution", EntityDataSerializers.INT);
    public static EntityDataKey<int> INTELLIGENCE = new("intelligence", EntityDataSerializers.INT);
    public static EntityDataKey<int> WISDOM = new("wisdom", EntityDataSerializers.INT);
    public static EntityDataKey<string> NAME = new("name", EntityDataSerializers.STRING);
    public static EntityDataKey<Origin> ORIGIN = new("origin", EntityDataSerializers.ENUM<Origin>());
    public static EntityDataKey<Guid> UUID = new("uuid", EntityDataSerializers.UUID);

    internal Vector3 GetVelocity(Vector3 input)
    {
        return DEXTERITY[this] * input;
    }

    internal void Hit(double v)
    {
        Hp.HP[this] -= v;
    }

    public override void DefineData(EntityDataBuilder builder)
    {
        base.DefineData(builder);
        builder.define(STRENGTH, strength);
        builder.define(STAMINA, stamina);
        builder.define(DEXTERITY, dexterity);
        builder.define(CONSTITUTION, constitution);
        builder.define(INTELLIGENCE, intelligence);
        builder.define(WISDOM, wisdom);
        builder.define(ORIGIN, origin);
        builder.define(UUID, Guid.NewGuid());
        Hp.DefineHp(builder, maxHp, maxHp);
    }
}