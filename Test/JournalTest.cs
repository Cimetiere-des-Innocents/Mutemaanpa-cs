namespace mutemaanpa_cs_test;

[TestClass]
public class JournalTest
{
    [TestInitialize]
    public void Init()
    {
        if (File.Exists("save.db"))
        {
            File.Delete("save.db");
        }
    }

    [TestMethod]
    public void ReadWriteTest()
    {
        var journal = new Journal("save.db");
        var uuid = Guid.NewGuid();
        journal.Set(uuid, "level", "one");
        journal.Store();
        var journal2 = new Journal("save.db");
        Assert.AreEqual(journal2.Get(uuid, "level"), "one");
    }
}
