using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class LightBarrier() : SoraModCard(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{

    public override bool GainsBlock => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("BlockValue", 10M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SevenWishes>(this.IsUpgraded), HoverTipFactory.FromCard<OneHeart>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Decimal num = await CreatureCmd.GainBlock(this.Owner.Creature,this.DynamicVars["BlockValue"].BaseValue, ValueProp.Unpowered | ValueProp.Move, play);
        
        // Remove a random debuff.
        var debuffsList = Owner.Creature.Powers.Where(p => p.Type == PowerType.Debuff).ToList();

        if (debuffsList.Count == 0)
            return;
        
        int removeIndex = Owner.RunState.Rng.Niche.NextInt(0, debuffsList.Count); // test this
        PowerModel removeDebuff = debuffsList[removeIndex];
        await PowerCmd.Remove(removeDebuff);
        
        CardModel createdCard = CombatState.CreateCard<SevenWishes>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BlockValue"].UpgradeValueBy(13);
    }
}