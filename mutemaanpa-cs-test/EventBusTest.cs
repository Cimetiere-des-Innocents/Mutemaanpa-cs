namespace mutemaanpa_cs_test;

[TestClass]
public class EventBusTest
{
    readonly AEvent eventA = new("111");
    readonly BEvent eventB = new("222");
    string s = "";

    public class AEvent(string s) : EventArgs
    {
        public override string ToString()
        {
            return s + " A";
        }
    }

    public class BEvent(string s) : EventArgs
    {
        public override string ToString()
        {
            return s + " B";
        }
    }

    [TestMethod]
    public void TestPubSub()
    {
        EventBus.Subscribe((AEvent a) =>
        {
            s = a.ToString();
        });
        EventBus.Publish(eventA);
        Assert.AreEqual(s, eventA.ToString());
    }

    [TestMethod]
    public void TestTwoEvents()
    {
        EventBus.Subscribe((AEvent a) =>
        {
            s = a.ToString();
        });
        EventBus.Publish(eventA);
        EventBus.Publish(eventB);
        Assert.AreEqual(s, eventA.ToString());
    }
}
