using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class JackSparrow() : SoraModCard(1, CardType.Power, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Savvy>(this.IsUpgraded), HoverTipFactory.FromCard<TreasureMagnet>(this.IsUpgraded), HoverTipFactory.FromCard<LuckyStrike>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel createdCard = CombatState.CreateCard<Savvy>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {

    }
}