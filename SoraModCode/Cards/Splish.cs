using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Splish() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // Infernal Blade
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Splash>(this.IsUpgraded), HoverTipFactory.FromCard<SeaSpectacle>(this.IsUpgraded), HoverTipFactory.FromCard<Water>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // Add Water
        CardModel createdWaterCard = CombatState.CreateCard<Water>(Owner);
        if (IsUpgraded)
            createdWaterCard.EnergyCost.SetThisCombat(0);
        else
            createdWaterCard.EnergyCost.SetUntilPlayed(0);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdWaterCard, PileType.Draw, this.Owner, CardPilePosition.Random));
        
        // Create next Link card
        CardModel createdLinkCard = CombatState.CreateCard<Splash>(Owner);
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