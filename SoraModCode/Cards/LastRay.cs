using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;

namespace SoraMod.SoraModCode.Cards;

public class LastRay() : SoraModCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy), ITomeCard
{
    // Need to make sure this shows up.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20M, ValueProp.Move)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);

        // What if you somehow had nothing in your hand...?
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            this.Owner,
            prefs,
            null,
            this
        );
        

        if (selectedCards == null || selectedCards.Count() == 0)
            return;
        
        foreach (CardModel card in selectedCards) // ...which is just one
        {
            CardCmd.ApplySingleTurnRetain(card);
        }
        
        ArgumentNullException.ThrowIfNull((object) play.Target!, "play.Target"); // This shouldn't be possible...? Hmmmmmm what if combat ended? Added "!"
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10M);
    }
}