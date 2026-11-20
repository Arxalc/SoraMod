using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

// Remove this later!
public class TestingCard() : SoraModCard(0, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DexterityPower? testPower = await PowerCmd.Apply<DexterityPower>(choiceContext, this.Owner.Creature, -2, this.Owner.Creature, (SoraModCard) this);
        //EnshroudingDarknessPower? testPower = await PowerCmd.Apply<EnshroudingDarknessPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (SoraModCard) this);
    }

    protected override void OnUpgrade()
    {

    }
}