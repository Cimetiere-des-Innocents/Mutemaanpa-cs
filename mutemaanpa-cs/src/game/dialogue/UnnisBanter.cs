namespace Mutemaanpa;

public partial class UnnisBanter : BanterInteraction
{
    public static readonly IBanter banter = new SingleBanter("And you thought revellon was flat.");
    protected override IBanter GetBanters()
    {
        return banter;
    }
}
