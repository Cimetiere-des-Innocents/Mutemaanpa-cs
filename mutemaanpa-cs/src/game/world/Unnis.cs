namespace Mutemaanpa;

public partial class Unnis : Interaction
{
    public override void _Ready()
    {
        base._Ready();
        interactiveText = UnnisDialogue();
    }

    private IInteractiveText UnnisDialogue() => new Dialogue(
        text: "Greetings, someone",
        new Transition(
            Text: "Bye unnis.",
            Next: null,
            Effect: EndDialogue
        ),
        new Transition(
            Text: "Hi Unnis.",
            Next: Dialogue.MakeSeqDialogue(ToEat(),
                "alas", "hello"),
            Effect: null
        )
    );

    private Transition ToEat() => new(
        Text: "I want to eat something",
        Next: FoodTheory(),
        Effect: null
    );

    private Dialogue FoodTheory() => new(
        text: "What you want to eat this day?",
        new Transition(
            Text: "Cookie",
            Next: null,
            Effect: () =>
            {
                gameMain!.Journal!.Set(gameMain.Save!.Value, "food", "Cookie");
                EndDialogue();
            }
        ),
        new Transition(
            Text: "Cereal",
            Next: null,
            Effect: () =>
            {
                gameMain!.Journal!.Set(gameMain.Save!.Value, "food", "cookie");
                EndDialogue();
            }
        )
    );
}
