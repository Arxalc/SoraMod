using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Curaga() : SoraModCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // Not Yet Copy
    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(10M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Magic, CardKeyword.Exhaust]; 

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(this.Owner.Creature, "Cast", this.Owner.Character.CastAnimDelay);
        await CreatureCmd.Heal(this.Owner.Creature, this.DynamicVars.Heal.BaseValue);
    }

    protected override void OnUpgrade() => this.DynamicVars.Heal.UpgradeValueBy(3M);
}