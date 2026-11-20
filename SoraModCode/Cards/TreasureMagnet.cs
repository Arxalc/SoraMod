using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class TreasureMagnet() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // Royalties
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(20)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<LuckyStrike>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        RoyaltiesPower royaltiesPower = await PowerCmd.Apply<RoyaltiesPower>(choiceContext, this.Owner.Creature, this.DynamicVars.Gold.BaseValue, this.Owner.Creature, (CardModel) this);
        
        CardModel createdCard = CombatState.CreateCard<LuckyStrike>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Gold.UpgradeValueBy(10M);
    }
}