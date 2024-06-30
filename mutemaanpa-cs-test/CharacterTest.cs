namespace mutemaanpa_cs_test;

using Mutemaanpa;

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
        var manager = new CharacterMemory(db);
        var uuid = manager.RegisterCharacter(
            stat,
            ability,
            null,
            Guid.NewGuid()
        );
        var character = manager.GetCharacterState(uuid);
        manager.Store();
        var manager2 = new CharacterMemory(db);
        manager2.Load();
        var characterRead = manager2.GetPlayer();
        Assert.AreEqual(character, characterRead);
    }
}
