using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class SeaSpectacle() : SoraModCard(3, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        // in your library (excludes Exhaust)
        List<CardModel> list = GetWaters(this.Owner).ToList<CardModel>();
        foreach (CardModel card in list)
        {
            await CardCmd.AutoPlay(choiceContext, card, (Creature) null);
        }
    }
    
    private static IEnumerable<CardModel> GetWaters(Player owner)
    {
        return owner.PlayerCombatState.AllCards.Where<CardModel>((Func<CardModel, bool>) (c => (c is Water || c is Waterga) && c.Pile.Type != PileType.Exhaust));
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
            
    }
}