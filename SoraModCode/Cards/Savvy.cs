using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Savvy() : SoraModCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // Snap
    
    private const string _cardsToChooseKey = "CardsToChoose";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CardsToChoose", 2)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Retain), HoverTipFactory.FromCard<TreasureMagnet>(this.IsUpgraded), HoverTipFactory.FromCard<LuckyStrike>(this.IsUpgraded)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // Select cards for Sly
        var library = CardPile.GetCards(this.Owner, PileType.Draw, PileType.Discard, PileType.Hand).ToList(); // Library
        if (library.Count == 0) return;
        
        // Card Select Prompt
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, (int) DynamicVars["CardsToChoose"].BaseValue)
        {
            PretendCardsCanBePlayed = true
        };
        
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            library,
            this.Owner,
            new CardSelectorPrefs(
                new LocString("cards", "SORAMOD-SAVVY.selectionScreenPrompt"), // Added this is cards.json
                0,
                (int) DynamicVars["CardsToChoose"].BaseValue
            )
        );

        if (selectedCards == null || selectedCards.Count() == 0)
        {
            return;
        }
        
        foreach (CardModel card in selectedCards)
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Sly);
        }
        
        // Create next card
        CardModel createdCard = CombatState.CreateCard<TreasureMagnet>(Owner);
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