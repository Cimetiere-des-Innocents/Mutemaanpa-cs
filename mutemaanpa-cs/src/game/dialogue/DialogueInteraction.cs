namespace Mutemaanpa;

public abstract partial class DialogueInteraction : Interaction
{
    public abstract string GetYarnProject();

    public abstract string GetYarnNode();

    protected override void DoInteraction()
    {
        DialogueBox.CreateDialogue(this, GetYarnProject(), GetYarnNode());
    }
}
