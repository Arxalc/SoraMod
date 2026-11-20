using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class Ventus() : SoraModCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        PureLightPower? pureLightPower = await PowerCmd.Apply<PureLightPower>(choiceContext, this.Owner.Creature, 1, this.Owner.Creature, (SoraModCard) this);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}