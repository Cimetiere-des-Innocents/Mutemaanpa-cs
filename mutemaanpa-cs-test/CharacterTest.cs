namespace mutemaanpa_cs_test;

using Dapper;
using Mutemaanpa;

[TestClass]
public class CharacterTest
{
    CharacterData character = new (
        Ability: new (1, 1, 1, 1, 1, 1),
        Stat: new (),
        Uuid: Guid.NewGuid(),
        Position: Godot.Vector3.Up,
        Player: null
    );

    [TestMethod]
    public void TestDB()
    {
        var db = new Database("Data Source=save.db");
        db.CommitCharacter(character);
    }
}
