using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class GoldenEyes() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => HoverTipFactory.FromEnchantment<Corrupted>();
    }
    
    public override async Task AfterObtained()
    {
        var curseList = Owner.Deck.Cards.Where(c => c.Rarity == CardRarity.Curse).ToList();

        if (curseList.Count != 0)
        {
            Log.Warn("Curse detected\n.");
            int removeIndex = Owner.RunState.Rng.Niche.NextInt(0, curseList.Count); // test this
            CardModel removeCurse = curseList[removeIndex];
            await CardPileCmd.RemoveFromDeck(removeCurse);
        }
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, this.DynamicVars.Cards.IntValue);
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(this.Owner, (EnchantmentModel) ModelDb.Enchantment<Corrupted>(), 1, prefs))
        {
            CardCmd.Enchant<Corrupted>(card, 1M);
            NCardEnchantVfx child = NCardEnchantVfx.Create(card);
            if (child != null)
            {
                NRun instance = NRun.Instance;
                if (instance != null)
                    instance.GlobalUi.CardPreviewContainer.AddChildSafely((Node) child);
            }
        }
        
        
    }
}