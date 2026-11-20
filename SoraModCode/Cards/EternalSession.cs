using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

public class EternalSession() : SoraModCard(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
{
    //private const string _dexLossKey = "DexLoss";
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30M, ValueProp.Move), new PowerVar<DexterityPower>(-2M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard((CardModel) this, play).TargetingAllOpponents(this.CombatState).WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3").Execute(choiceContext);
        foreach (Creature hittableEnemy in (IEnumerable<Creature>) this.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<DexterityPower>(choiceContext, hittableEnemy, this.DynamicVars.Dexterity.BaseValue, this.Owner.Creature, (CardModel) this);
        }
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Dexterity.UpgradeValueBy(-1M);
    }
}