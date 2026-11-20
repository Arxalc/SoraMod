using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class UltimaWeapon() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || this.Owner.PlayerCombatState.TurnNumber > 1)
            return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1, 1);

        // What if you somehow had nothing in your hand...?
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            this.Owner,
            prefs,
            null,
            this
        );
        

        if (selectedCards == null || selectedCards.Count() == 0)
            return;
        
        foreach (CardModel card in selectedCards) // ...which is just one
        {
            CardCmd.ApplyKeyword(card, CardKeyword.Retain);
        }
    }
    
}