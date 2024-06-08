namespace mutemaanpa_cs_test;

using Mutemaanpa;

[TestClass]
public class CharacterTest
{
    CharacterData character = new(
        Ability: new(1, 1, 1, 1, 1, 1),
        Stat: new(
            "test",
            1.0f,
            1,
            Origin.SPY
        ),
        Uuid: Guid.NewGuid(),
        Position: Godot.Vector3.Up,
        Player: null
    );

    [TestMethod]
    public void TestDB()
    {
        if (File.Exists("save.db"))
        {
            File.Delete("save.db");
        }
        var db = new Database("Data Source=save.db");
        db.CommitCharacter(character);
        var characters = db.QueryCharacter();
        Assert.AreEqual(character, characters.First());
        // clean up if successful
        File.Delete("save.db");
    }
}
