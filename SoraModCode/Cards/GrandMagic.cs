using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class GrandMagic() : SoraModCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        GrandMagicPower? grandMagicPower = await PowerCmd.Apply<GrandMagicPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (SoraModCard) this);
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}