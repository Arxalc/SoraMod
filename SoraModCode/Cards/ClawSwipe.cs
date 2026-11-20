using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class ClawSwipe() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    private const string _cardsToChooseKey = "CardsToChoose";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CardsToChoose", 2)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<StalwartFang>(this.IsUpgraded), HoverTipFactory.FromCard<HowlingMoon>(this.IsUpgraded), HoverTipFactory.FromCard<Claw>()];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // TODO: Exclude Rage form too!!

        // Card Select Prompt
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, (int) DynamicVars["CardsToChoose"].BaseValue)
        {
            PretendCardsCanBePlayed = true
        };
        // this.SelectionScreenPrompt
        
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            this.Owner,
            prefs,
            e => e is not AntiForm,
            this
        );
        

        if (selectedCards == null || selectedCards.Count() == 0)
        {
            return;
        }
        
        foreach (CardModel card in selectedCards)
        {
            // Look at Guards for this
            CardModel clawCard = (CardModel) this.CombatState.CreateCard<Claw>(this.Owner);
            if (card.IsUpgraded)
                CardCmd.Upgrade(clawCard);
            CardPileAddResult? nullable = await CardCmd.Transform(card, clawCard);
        }
        
        // Create next card
        CardModel createdCard = CombatState.CreateCard<StalwartFang>(Owner);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(createdCard);
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(createdCard, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CardsToChoose"].UpgradeValueBy(1);
    }
}