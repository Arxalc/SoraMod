using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class OneForAll() : SoraModCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [(DynamicVar) new PowerVar<StrengthPower>(1M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        // Exhaust any number of summons from your deck. Gain 1 strength for each exhausted card.
        
        CardPile.GetCards(this.Owner, PileType.Draw, PileType.Discard, PileType.Hand).Where(c => c.Keywords.Contains(SoraModKeywords.Link));
        
        // I'm... not sure if this will affect the hand instantly or not
        var linkCards = CardPile.GetCards(this.Owner, PileType.Draw, PileType.Discard, PileType.Hand)
            .Where(c => c.Keywords.Contains(SoraModKeywords.Link))
            .ToList();

        if (linkCards.Count == 0)
        {
            return;
        }
        
        // Card Select Prompt
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 1)
        {
            PretendCardsCanBePlayed = true
        };
        
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            linkCards,
            this.Owner,
            new CardSelectorPrefs(
                new LocString("cards", "SORAMOD-ONE_FOR_ALL.selectionScreenPrompt"), // Added this is cards.json
                0,
                linkCards.Count
            )
        );

        if (selectedCards == null || selectedCards.Count() == 0)
        {
            return;
        }
        
        foreach (CardModel card in selectedCards)
        {
            CardPileAddResult? nullable = await CardCmd.Exhaust(choiceContext, card);
        }
        StrengthPower strengthPower1 = await PowerCmd.Apply<StrengthPower>(choiceContext, this.Owner.Creature, this.DynamicVars["StrengthPower"].BaseValue * selectedCards.Count(), this.Owner.Creature, (CardModel) this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Strength.UpgradeValueBy(1M);
    }
}