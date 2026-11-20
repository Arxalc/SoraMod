using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class ConsumingDarkness() : SoraModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

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