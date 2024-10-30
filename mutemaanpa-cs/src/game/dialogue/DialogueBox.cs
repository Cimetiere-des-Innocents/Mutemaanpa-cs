namespace Mutemaanpa;

using Godot;
using YarnSpinnerGodot;

public partial class DialogueBox : Control
{
    [Export]
    DialogueView? view;

    DialogueRunner dialogueRunner = new();

    InMemoryVariableStorage variableStorage = new();

    public static void CreateDialogue(
        Node curNode,
        string yarnProjectPath,
        string startNode)
    {
        static DialogueBox findDialogueBox(Node node) => node switch
        {
            World world => world.dialogueBox!,
            null => throw new System.Exception("interaction not having a dialogue box paired."),
            _ => findDialogueBox(node.GetParent())
        };
        var node = findDialogueBox(curNode);
        node.Visible = true;
        var yarnProject = GD.Load<YarnProject>(yarnProjectPath);
        node.dialogueRunner!.SetProject(yarnProject);
        node.dialogueRunner!.StartDialogue(startNode);
    }

    public override void _Ready()
    {
        view!.OnDialogueLineFinished += Hide;
        dialogueRunner.startAutomatically = false;
        dialogueRunner.VariableStorage = variableStorage;
        dialogueRunner.SetDialogueViews([view]);
        variableStorage.debugTextView = GetNode<RichTextLabel>("DebugLabelClass");
        AddChild(variableStorage);
        AddChild(dialogueRunner);
        Hide();
        base._Ready();
    }
}
