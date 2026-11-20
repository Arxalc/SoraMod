using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class Zantetsuken() : SoraModCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("executeHP", 30)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Finisher];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (play.Target == null)
            return;

        if (play.Target.CurrentHp <= this.DynamicVars["executeHP"].BaseValue)
        {
            await CreatureCmd.Kill(play.Target, true); // true since it should bypass effects?
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["executeHP"].UpgradeValueBy(10); 
    }
}