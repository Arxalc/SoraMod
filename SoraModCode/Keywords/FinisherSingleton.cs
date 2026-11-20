using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Keywords;

public class FinisherSingleton() : CustomSingletonModel(HookType.Combat)
{
  
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Card.Keywords.Contains(SoraModKeywords.Finisher))
            return;
        
        CardModel card = (await CardSelectCmd.FromHandForDiscard(choiceContext, cardPlay.Player, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), (Func<CardModel, bool>) null, (AbstractModel) this)).FirstOrDefault<CardModel>();
        if (card == null)
            return;
        await CardCmd.Discard(choiceContext, card);
    }
}