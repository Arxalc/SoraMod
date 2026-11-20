using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class LuckyCharm() : SoraModCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // Expertise
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<LightBarrier>(this.IsUpgraded), HoverTipFactory.FromCard<SevenWishes>(this.IsUpgraded), HoverTipFactory.FromCard<OneHeart>(this.IsUpgraded)];

    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (CardModel card in await CardPileCmd.Draw(choiceContext, (Decimal) this.DynamicVars.Cards.IntValue, this.Owner))
            CardCmd.ApplySingleTurnRetain(card);
        
        CardModel createdCard = CombatState.CreateCard<LightBarrier>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Cards.UpgradeValueBy(2M);
    }
}