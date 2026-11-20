using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class KingdomKey() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<UltimaWeapon>();
    
    // What does this happen in terms of tools of the trade?
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || this.Owner.PlayerCombatState.TurnNumber > 1)
            return;

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
    }
    
    /*
    //Really have to make sure this is instance-based.
    // Gauge of 10.
    public int valorGauge = 0;
    public int wisdomGauge = 0;
    
    // Man I really have to double check my card code and see if cardPlay.Player or cardPlay.Owner was appropriate...
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner)
            return;
        if ((cardPlay.Card.Type != CardType.Attack && cardPlay.Card.Type != CardType.Skill) || cardPlay.Card.Keywords.Contains(SoraModKeywords.Link)) // ...double check this logic.
            return;
        
        
        if (cardPlay.Card.Keywords.Contains(SoraModKeywords.Finisher))
        {
            valorGauge++;
        }
        if (cardPlay.Card.Keywords.Contains(SoraModKeywords.Magic))
        {
            wisdomGauge++;
        }
        if (cardPlay.Card.Type == CardType.Attack)
        {
            valorGauge++;
        }
        if (cardPlay.Card.Type == CardType.Skill)
        {
            wisdomGauge++;
        }
        
        // UI increases guage.
        
    }
    
    private void increaseGauge()
    {
        //  
        if (valorGauge >= 10) // block wisdom
        {
            
        }
    }
    */
}