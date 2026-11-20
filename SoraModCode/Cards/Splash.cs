using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Splash() : SoraModCard(2, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<SeaSpectacle>(this.IsUpgraded), HoverTipFactory.FromCard<Waterga>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // Add Waterga
        CardModel createdWaterCard = CombatState.CreateCard<Waterga>(Owner);
        if (IsUpgraded)
            createdWaterCard.EnergyCost.SetThisCombat(0);
        else
            createdWaterCard.EnergyCost.SetUntilPlayed(0);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdWaterCard, PileType.Draw, this.Owner, CardPilePosition.Random));
        
        // Create next Link card
        CardModel createdLinkCard = CombatState.CreateCard<SeaSpectacle>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdLinkCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdLinkCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {

    }
}