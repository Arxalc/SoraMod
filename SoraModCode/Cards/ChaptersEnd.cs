using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class ChaptersEnd() : SoraModCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // pain.
        
        // Choose card to exhaust.

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
                new LocString("cards", "SORAMOD-CHAPTERS_END.selectionScreenPrompt"), // Added this is cards.json
                0,
                1
            )
        );

        if (selectedCards == null || selectedCards.Count() == 0)
        {
            return;
        }

        CardModel chosenCard = selectedCards.First();
        CardModel lastLinkCard = FindLastLink(chosenCard).ToMutable();

        // ...What happens if one is upgraded and the other isn't...? 
        // TODO: Test.
        if (chosenCard.Id == lastLinkCard.Id)
        {
            // play itself instead of exhausting.
            await CardCmd.AutoPlay(choiceContext, chosenCard, (Creature) null);
        }
        else
        {
            // exhaust, then play the last link card...
            // TODO: Check for upgrades...
            // This feels like a lot to do to just create and play this card...
            CardPileAddResult? nullable = await CardCmd.Exhaust(choiceContext, chosenCard);
            CombatState.AddCard(lastLinkCard, Owner);
            await CardPileCmd.AddGeneratedCardToCombat(lastLinkCard, PileType.Exhaust, this.Owner);
            await CardCmd.AutoPlay(choiceContext, lastLinkCard, (Creature) null);
        }

        
            
    }

    /*
     * Given a card, return the last link card in that link series.
     * Will return itself if it is the last link card.
     * ...Maybe do an interface for link cards one day.
     */
    private CardModel? FindLastLink(CardModel card)
    {
        // Already did a null check before calling this method.
        if (!card.Keywords.Contains(SoraModKeywords.Link))
        {
            MainFile.Logger.Warn("No link finale card found. How did you even manage this?"); 
            return null;
        }
        
        return card switch
        {
            Donald or DonaldFire or DuckFlare or Comet or Trinity => ModelDb.Card<Trinity>(),
            Goofy or GoofyTornado or KnockSmash or Bombardier or Trinity => ModelDb.Card<Trinity>(),
            PeterPan or Hummingbird or Crafty or PixieDust => ModelDb.Card<PixieDust>(),
            MeowWow or Whomperstomp or BalloonBurst => ModelDb.Card<BalloonBurst>(),
            PoohBear or StoutnessExercise or BothersomeBees or ChaptersEnd => ModelDb.Card<ChaptersEnd>(),
            Beast or ClawSwipe or StalwartFang or HowlingMoon => ModelDb.Card<HowlingMoon>(),
            JackSparrow or Savvy or TreasureMagnet or LuckyStrike => ModelDb.Card<LuckyStrike>(),
            Hercules or AuraGuard or HeroicSwing or HeartOfAHero => ModelDb.Card<HeartOfAHero>(),
            Kairi or LuckyCharm or LightBarrier or SevenWishes or OneHeart => ModelDb.Card<OneHeart>(),
            Riku or DarkFiraga or DarkBarrier or NightmaresEnd or EternalSession => ModelDb.Card<EternalSession>(),
            Simba or ProudRoar or Wildcat or KingsFlare => ModelDb.Card<KingsFlare>(),
            Ariel or Splish or Splash or SeaSpectacle => ModelDb.Card<SeaSpectacle>(),
            Stitch or Ohana or PlasmaSurge or PlasmaEncounter => ModelDb.Card<PlasmaEncounter>(),
            Mickey or HealingLight or KingsGuidance or EnduringFaith => ModelDb.Card<EnduringFaith>(),
            Roxas => ModelDb.Card<Roxas>(),
            Xion => ModelDb.Card<Xion>(),
            Ventus => ModelDb.Card<Ventus>(),
            _ => null
        };
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}