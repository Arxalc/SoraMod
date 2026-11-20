using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Encounters;

public class Ansem : CustomMonsterModel
{
    private const string _enshroudingDarknessMove = "ENSHROUDING_DARKNESS_MOVE";
    private const string _consumingDarknessMove = "CONSUMING_DARKNESS_MOVE";

    public override int MinInitialHp
    {
        get => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 240, 210);
    }

    public override int MaxInitialHp => this.MinInitialHp;
    
    private void SetColor(Color color)
    {
        NCombatRoom.Instance?.GetCreatureNode(this.Creature)?.GetSpecialNode<CanvasGroup>("%CanvasGroup")?.SetSelfModulate(color);
    }
 
    private int DesperationAmount
    {
        get => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 80 /*0xA0*/, 70);
    }
    
    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        AnsemDMPower dmPower = await PowerCmd.Apply<AnsemDMPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), this.Creature, (Decimal) this.DesperationAmount, this.Creature, (CardModel) null);
    }
    
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> states = new List<MonsterState>();
  
        MoveState initialState = new MoveState("ENSHROUDING_DARKNESS_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.EnshroudingDarknessMove), new AbstractIntent[1]
        {
            (AbstractIntent) new CardDebuffIntent()
        });
        
        MoveState secondInitialState = new MoveState("CONSUMING_DARKNESS_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.ConsumingDarknessMove), new AbstractIntent[1]
        {
            (AbstractIntent) new BuffIntent()
        });

        MoveState moveState1 = new MoveState("WEAK_SINGLE_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.WeakSingleMove), new AbstractIntent[1]);
        
        
        ConditionalBranchState desperationMoveBranchState = new ConditionalBranchState("DESPERATION_MOVE_BRANCH");
        
        //desperationMoveBranchState.AddState((MonsterState) this.BurnBrightForMeState, (Func<bool>) (() => !this.HasAmalgamDied));
        
        
        throw new NotImplementedException();
    }
    
    
    private async Task EnshroudingDarknessMove(IReadOnlyList<Creature> targets)
    {
        //SfxCmd.Play("event:/sfx/enemy/enemy_attacks/queen/queen_cast");
       // await CreatureCmd.TriggerAnim(this.Creature, "Cast", 0.5f);
       IReadOnlyList<EnshroudingDarknessPower> enshroudingDarknessPower = await PowerCmd.Apply<EnshroudingDarknessPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), targets, 1M, this.Creature, (CardModel) null);
    }
    
    private async Task ConsumingDarknessMove(IReadOnlyList<Creature> targets)
    {
        //SfxCmd.Play("event:/sfx/enemy/enemy_attacks/queen/queen_cast");
        // await CreatureCmd.TriggerAnim(this.Creature, "Cast", 0.5f);
        ConsumingDarknessPower consumingDarknessPower = await PowerCmd.Apply<ConsumingDarknessPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), this.Creature, 1M, this.Creature, (CardModel) null);
    }

    private int WeakSingleMoveDamage
    {
        get => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 15);
    }
    
    private async Task WeakSingleMove(IReadOnlyList<Creature> targets)
    {
        AttackCommand attackCommand = await DamageCmd.Attack((Decimal) this.WeakSingleMoveDamage).FromMonster((MonsterModel) this).Execute((PlayerChoiceContext) null);
    }
    
    // Triggered when Ansem gets down to 1/3 hp by AnsemDMPower
    public async Task SetDesperation()
    {
        /*
        SfxCmd.Play("event:/sfx/enemy/enemy_attacks/ceremonial_beast/ceremonial_beast_stun");
        await CreatureCmd.TriggerAnim(this.Creature, "Stun", 0.6f);
        */
    }
    
}
