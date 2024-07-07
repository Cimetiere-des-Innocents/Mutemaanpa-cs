namespace mutemaanpa_cs_test;

[TestClass]
public class CharacterTest
{
    static CharacterAbility ability = new(1, 1, 2, 1, 1, 1);
    static CharacterStat stat = new(
            "test",
            1.0f,
            1,
            Origin.SPY
    );

    static CharacterData data = new(
        Ability: ability,
        Stat: stat,
        Uuid: Guid.NewGuid(),
        Position: Godot.Vector3.Up,
        Player: null
    );

    [TestInitialize]
    public void Init()
    {
        if (File.Exists("save.db"))
        {
            File.Delete("save.db");
        }
    }

    [TestMethod]
    public void TestDB()
    {
        var db = new CharacterDatabase("Data Source=save.db");
        db.InitDatabase();
        db.CommitCharacter(data);
        var characters = db.QueryCharacter();
        Assert.AreEqual(data, characters.First());
        // clean up if successful
        File.Delete("save.db");
    }

    [TestMethod]
    public void TestManager()
    {
        var db = new CharacterDatabase("Data Source=save.db");
        db.InitDatabase();
        var manager = new CharacterMemory(db);
        var uuid = manager.RegisterCharacter(
            stat,
            ability,
            Vector3.Zero,
            Guid.NewGuid()
        );
        var character = manager.GetCharacterState(uuid);
        manager.Store();
        var manager2 = new CharacterMemory(db);
        manager2.Load();
        var characterRead = manager2.GetPlayerState();
        Assert.AreEqual(character, characterRead);
    }

    [TestMethod]
    public void TestMove()
    {
        var db = new CharacterDatabase("Data Source=save.db");
        db.InitDatabase();
        var manager = new CharacterMemory(db);
        _ = manager.RegisterCharacter(
            stat,
            ability,
            Vector3.Zero,
            Guid.NewGuid()
        );
        var character = manager.GetPlayer();
        character.Move(Vector3.Up, 1.0f);
        var movedVector = Vector3.Up * 5.0f * 1.0f;
        Assert.AreEqual(character.Dump().Position!.Value, movedVector);
    }

    [TestMethod]
    public void TestHit()
    {
        var db = new CharacterDatabase("Data Source=save.db");
        db.InitDatabase();
        var manager = new CharacterMemory(db);
        _ = manager.RegisterCharacter(
            stat,
            ability,
            Vector3.Zero,
            Guid.NewGuid()
        );
        var character = manager.GetPlayer();
        Assert.IsFalse(character.Dead);
        character.Hit(1.0f);
        Assert.IsTrue(character.Dead);
        character.Hit(100.0f);
        Assert.AreEqual(character.Dump().Stat.Hp, 0.0f, 1e-5);
    }
}
