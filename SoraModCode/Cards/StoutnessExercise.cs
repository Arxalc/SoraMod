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

public class StoutnessExercise() : SoraModCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>(4M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VigorPower>(), HoverTipFactory.FromCard<BothersomeBees>(this.IsUpgraded), HoverTipFactory.FromCard<ChaptersEnd>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        VigorPower vigorPower = await PowerCmd.Apply<VigorPower>(choiceContext, this.Owner.Creature, (Decimal) this.DynamicVars["VigorPower"].IntValue, this.Owner.Creature, (CardModel) this);
        
        CardModel createdCard = CombatState.CreateCard<BothersomeBees>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["VigorPower"].UpgradeValueBy(1M);
    }
}