using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class ProudRoar() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    
    private const string _proudRoarDamageKey = "ProudRoarDamage";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("ProudRoarDamage", 20M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ProudRoarPower>(), HoverTipFactory.FromCard<Wildcat>(this.IsUpgraded), HoverTipFactory.FromCard<KingsFlare>(this.IsUpgraded)];
    

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        (await PowerCmd.Apply<ProudRoarPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (CardModel) this)).SetDamage(this.DynamicVars["ProudRoarDamage"].BaseValue);

        CardModel createdCard = CombatState.CreateCard<Wildcat>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["ProudRoarDamage"].UpgradeValueBy(10M);
    }
}