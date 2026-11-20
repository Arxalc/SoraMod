using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class KingdomKey() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    // At the end of your turn, retain 1 card? -> What would I even evolve this relic into...
    
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