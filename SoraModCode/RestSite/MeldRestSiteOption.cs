using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SoraMod.SoraModCode.Enchantments;

namespace SoraMod.SoraModCode.RestSite;

public class MeldRestSiteOption(Player owner) : RestSiteOption(owner)
{
    private const int _cardsToMeld = 2;

    public override string OptionId => "MELD";
    
    // public override string IconPath => "campfire/meld_enchantment.png"; // TODO: Harmony post patch this.
    
    public override LocString Description
    {
        get
        {
            if (!this.IsEnabled)
                return new LocString("rest_site_ui", $"OPTION_{this.OptionId}.descriptionDisabled");
            LocString description = new LocString("rest_site_ui", $"OPTION_{this.OptionId}.description");
            description.Add("Cards", 2M);
            return description;
        }
    }

    public override bool IsEnabled => GetMeldableCardCount(Owner) >= 2;

    private static int GetMeldableCardCount(Player player) // MAKE SURE IF NO MORE THAN 2 CARDS, OPTION IS GREYED OUT
    {
        // test
        return PileType.Deck.GetPile(player).Cards
            .Count<CardModel>((Func<CardModel, bool>)(c => c.Enchantment == null));
    }

    public override async Task<bool> OnSelect()
    {
        var dummyContext = new ThrowingPlayerChoiceContext();

        // Select to remove (second) card to meld 
        var partnerCards = await CardSelectCmd.FromDeckForRemoval(
            Owner,
            new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1)
            {
                RequireManualConfirmation = true
            },
            filter: card => card.Enchantment == null
        );

        CardModel? meldPartner = partnerCards?.FirstOrDefault();
        if (meldPartner == null) return false;

        // Select the card to enchant with Meld
        CardSelectorPrefs prefs = new CardSelectorPrefs(
            new LocString("rest_site_ui", "SORAMOD-CAMPFIRE_SELECT_MELD_TARGET.selectionScreenPrompt"), 
            1);
        
        var enchantTargets = await CardSelectCmd.FromDeckGeneric(
            Owner,
            prefs,
            filter: card => card.Enchantment == null && card != meldPartner);

        CardModel? enchantTarget = enchantTargets?.FirstOrDefault();
        if (enchantTarget == null) return false;
        
        var meldEnchantment = CardCmd.Enchant<Meld>(enchantTarget, 1M);
        if (meldEnchantment == null) return false;
        
        if (enchantTarget.Enchantment is Meld)
        {
            meldEnchantment.MeldTargetId = meldPartner.Id;
            meldEnchantment.MeldTargetWasUpgraded = meldPartner.IsUpgraded;
        }
        
        // Actually remove the meld partner from the deck
        await CardPileCmd.RemoveFromDeck(meldPartner);
        
        var vfx = NCardEnchantVfx.Create(enchantTarget);
        NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(vfx);

        return true;
    }




}