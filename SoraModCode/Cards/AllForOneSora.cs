using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class AllForOneSora() : SoraModCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    //TODO: Double check this actually works...
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Put all Friend cards from the draw pile into your hand
        foreach (CardModel card in (IEnumerable<CardModel>)PileType.Draw.GetPile(this.Owner).Cards
                     .Where<CardModel>(new Func<CardModel, bool>(this.Filter)).ToList<CardModel>())
        {
            CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Hand);
        }

        // Make all Friend cards in your hand cost 0 until played
        foreach (CardModel card in (IEnumerable<CardModel>)PileType.Hand.GetPile(this.Owner).Cards)
        {
            if (card.Keywords.Contains(SoraModKeywords.Link))
            {
                card.EnergyCost.SetUntilPlayed(0, false);
            }
        }
    }

    private bool Filter(CardModel card)
    {
       return card.Keywords.Contains(SoraModKeywords.Link);
    }
    
    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}