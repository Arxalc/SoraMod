using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class SunsetMemoriesPower() : SoraModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single; // Counter?

    // Juggling

    // TODO: Need to double check behavior form for like Echo Form multi-plays
    // AfterCardPlayed logic
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == this.Owner && cardPlay.Card.Type == CardType.Skill)
        {
            int numSkillsPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted
                .Count(e => e.Actor == this.Owner && e.CardPlay.Card.Type == CardType.Skill && e.HappenedThisTurn(this.CombatState));
        
            if (numSkillsPlayedThisTurn == 1)
            {
                CardModel cardCopy = cardPlay.Card.CreateClone();
                CardCmd.ApplyKeyword(cardCopy, CardKeyword.Ethereal);
                await CardPileCmd.AddGeneratedCardToCombat(cardCopy, PileType.Hand, this.Owner.Player);
            }
        }
    }
}