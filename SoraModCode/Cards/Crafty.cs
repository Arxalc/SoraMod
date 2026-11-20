using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Crafty() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // Armaments without the Block
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<PixieDust>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel createdCard = CombatState.CreateCard<PixieDust>(Owner);
        if (this.IsUpgraded)
        {
            foreach (CardModel card in PileType.Hand.GetPile(this.Owner).Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgradable)))
                CardCmd.Upgrade(card);
            
            CardCmd.Upgrade(createdCard);
        }
        else
        {
            CardModel card = await CardSelectCmd.FromHandForUpgrade(choiceContext, this.Owner, (AbstractModel) this);
            if (card == null)
                return;
            CardCmd.Upgrade(card);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {

    }
}