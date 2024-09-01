// namespace Mutemaanpa;

// using Godot;
// using Kernel;

// /// <summary>
// /// CharacterInformation shows the character stat in a in-game popup window.
// /// </summary>
// public partial class CharacterInformation : CenterContainer
// {
//     [Export]
//     Label? CharacterName;

//     [Export]
//     LabelValue? HitPointDisplay;

//     [Export]
//     LabelValue? ManaPointDisplay;

//     [Export]
//     LabelValue? OriginDisplay;

//     [Export]
//     LabelValue? StrengthDisplay;

//     [Export]
//     LabelValue? StaminaDisplay;

//     [Export]
//     LabelValue? DexterityDisplay;

//     [Export]
//     LabelValue? ConstitutionDisplay;

//     [Export]
//     LabelValue? IntelligenceDisplay;

//     [Export]
//     LabelValue? WisdomDisplay;

//     [Export]
//     Button? QuitButton;

//     Session? Session;

//     public static CharacterInformation From(Session session)
//     {
//         var info = ResourceLoader.Load<PackedScene>("res://scene/game/hud/character_information.tscn")
//             .Instantiate<CharacterInformation>();
//         info.Session = session;
//         return info;
//     }
 
//     public override void _Ready()
//     {
//         QuitButton!.Pressed += QueueFree;
//     }

//     public override void _PhysicsProcess(double delta)
//     {
//         var hp = 
//         HitPointDisplay!.SetLabelValue("HP", $"{state.Data.Stat.Hp} / {state.Runtime.MaxHitPoint}");
//         ManaPointDisplay!.SetLabelValue("MP", $"{state.Data.Stat.Mp} / {state.Runtime.MaxManaPoint}");
//         OriginDisplay!.SetLabelValue("Origin", state.Data.Stat.Origin.ToString());
//         StrengthDisplay!.SetLabelValue("Strength", state.Data.Ability.Strength.ToString());
//         StaminaDisplay!.SetLabelValue("Stamina", state.Data.Ability.Stamina.ToString());
//         DexterityDisplay!.SetLabelValue("Dexterity", state.Data.Ability.Stamina.ToString());
//         ConstitutionDisplay!.SetLabelValue("Constitution", state.Data.Ability.Stamina.ToString());
//         IntelligenceDisplay!.SetLabelValue("Intelligence", state.Data.Ability.Stamina.ToString());
//         WisdomDisplay!.SetLabelValue("Wisdom", state.Data.Ability.Stamina.ToString());
//         info.CharacterName!.Text = state.Data.Stat.Name;
//     }
// }
