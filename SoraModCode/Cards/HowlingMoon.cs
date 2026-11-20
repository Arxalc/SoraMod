using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class HowlingMoon() : SoraModCard(3, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Claw>(this.IsUpgraded), HoverTipFactory.FromCard<Maul>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var howlingCards = CardPile.GetCards(this.Owner, PileType.Draw, PileType.Discard, PileType.Hand)
            .Where(e => e is Claw || e is Maul)
            .ToList();
        
        // Upgrade first, play, then exhaust separately so it doesn't look like visual vomit.
        // ...It still looks pretty jumbled... 
        foreach (var card in howlingCards)
        {
            if (IsUpgraded)
            {
                if (!card.IsUpgraded)
                    CardCmd.Upgrade(card);
            }
        }
        
        foreach (var card in howlingCards)
        {
            await CardCmd.AutoPlay(choiceContext, card, this.CurrentTarget);
        }

        foreach (var card in howlingCards)
        {
            CardPileAddResult? nullable = await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {

    }
}