namespace Mutemaanpa;

public partial class Lena : Interaction
{
    public override void _Ready()
    {
        base._Ready();
        interactiveText = new SerialBanter("Did you expect a needle", "You thought riverllon was flat");
    }
}
