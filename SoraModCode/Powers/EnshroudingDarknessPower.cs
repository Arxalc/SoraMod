using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Models.Events;
using SoraMod.SoraModCode.Encounters;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class EnshroudingDarknessPower() : SoraModPower
{
    // VineShambler's Tangled Power

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Single;

    // Store a dictionary of <CardModel, Iterator Hovertips> so I can restore them after it's played.
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => HoverTipFactory.FromAffliction<Enshroud>();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => []; // 1/5 -> 1/4 cards are affected? for higher ascensions
    
    
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // Randomly choose cards in your combat deck to afflict.
        var library = CardPile.GetCards(Owner.Player, PileType.Draw, PileType.Discard, PileType.Hand).ToList(); // Library
        if (library.Count == 0) return;

        int numCardsToAfflect = library.Count / 5; // -> 1/4 for higher ascensions?

        // Afflict cards - ideally afflict all at once.
        for (int i = 0; i < numCardsToAfflect; i++)
        {
            CardModel card = this.Owner.Player.RunState.Rng.CombatCardSelection.NextItem<CardModel>((IEnumerable<CardModel>) library); // some sort of lambda where it skips over already chosen cards...
            
            // Try again if it hit an already chosen card.
            if (card.Affliction != null) // if the card is already afflicted
            {
                i--; // retry
            }
            else
            {
                Enshroud enshroud = await CardCmd.Afflict<Enshroud>(card, 1M);
                 
            }
        }
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Affliction is Enshroud)
        {
            CardCmd.ClearAffliction(cardPlay.Card);
            await Cmd.Wait(0.5f); // is this timing caught before card goes into discard...?
        }
    }
    
    public override Task AfterRemoved(Creature oldOwner)
    {
        foreach (CardModel card in oldOwner.Player.PlayerCombatState.AllCards.Where<CardModel>((Func<CardModel, bool>) (c => c.Affliction is Enshroud)))
            CardCmd.ClearAffliction(card);
        return Task.CompletedTask;
    }
}