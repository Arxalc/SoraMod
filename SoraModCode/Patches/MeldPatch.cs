using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SoraMod.SoraModCode.Enchantments;

public class MeldSingleton : CustomSingletonModel
{
    public MeldSingleton() : base(HookType.Combat) { }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        
        // Check if this card has the Meld enchantment
        if (card.Enchantment is Meld meldEnchantment && meldEnchantment.MeldTarget != null)
        {
            await CardCmd.AutoPlay(choiceContext, meldEnchantment.MeldTarget, null);
        }
    }
    
}