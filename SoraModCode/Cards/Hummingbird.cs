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

public class Hummingbird() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(3M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust]; // How to optionally add Regen?
    // TODO: Make the base and upgraded card descriptions AND optional regen hovertip...
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Crafty>(this.IsUpgraded), HoverTipFactory.FromCard<PixieDust>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);

        CardModel createdCard = CombatState.CreateCard<TinkerTalent>(Owner);
        if (play.Card.IsUpgraded)
        {
            RegenPower regenPower = await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, DynamicVars.Heal.BaseValue, this.Owner.Creature, (CardModel) this);
            CardCmd.Upgrade(createdCard);
        }
        else
        {
            await CreatureCmd.Heal(this.Owner.Creature, DynamicVars.Heal.BaseValue);
        }
        
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner));
    }

    protected override void OnUpgrade()
    {

    }
}