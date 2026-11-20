using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Encounters;

public class TheSeeker : CustomMonsterModel
{
    private const string _enshroudingDarknessMove = "ENSHROUDING_DARKNESS_MOVE";
    private const string _consumingDarknessMove = "CONSUMING_DARKNESS_MOVE";

    private MoveState _deadState;
    private int _respawns;
    
    public override int MinInitialHp => this.FirstFormHp;
    public override int MaxInitialHp => this.MinInitialHp;
    
    public int FirstFormHp
    {
        get => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 160, 140);
    }

    public int SecondFormHp
    {
        get => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 80, 70);
    }
    
    /*
    public override string DeathSfx
    {
        get
        {
            if (!this.IsMutable)
                return base.DeathSfx;
            return !this.Creature.HasPower<AdaptablePower>() ? base.DeathSfx : "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_knock_out";
        }
    }
    */
    
    private MoveState DeadState
    {
        get => this._deadState;
        set
        {
            this.AssertMutable();
            this._deadState = value;
        }
    }

    private int Respawns
    {
        get => this._respawns;
        set
        {
            this.AssertMutable();
            this._respawns = value;
        }
    }
    
    public override bool ShouldDisappearFromDoom => this.Respawns >= 1; // not sure what this does...
    
    /*
    public async Task TriggerDeadState()
    {
        NRunMusicController.Instance?.UpdateMusicParameter("test_subject_progress", 1f);
        ++this.CombatState.RunState.ExtraFields.TestSubjectKills;
        await CreatureCmd.TriggerAnim(this.Creature, "DeadTrigger", 0.0f);
        this.SetMoveImmediate(this.DeadState, true);
    }
    */
    
    public override void BeforeRemovedFromRoom()
    {
        this.Creature.PowerApplied -= new Action<PowerModel>(this.AfterPowerApplied);
        this.Creature.PowerRemoved -= new Action<PowerModel>(this.AfterPowerRemoved);
    }
    
    private void AfterPowerApplied(PowerModel power)
    {
        if (!(power is IntangiblePower))
            return;
        this.SetColor(StsColors.halfTransparentWhite);
    }

    private void AfterPowerRemoved(PowerModel power)
    {
        if (!(power is IntangiblePower))
            return;
        this.SetColor(Colors.White);
    }
    
    private void SetColor(Color color)
    {
        NCombatRoom.Instance?.GetCreatureNode(this.Creature)?.GetSpecialNode<CanvasGroup>("%CanvasGroup")?.SetSelfModulate(color);
    }
    
    
    
    
    
    
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> states = new List<MonsterState>();
        this.DeadState = new MoveState("RESPAWN_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.RespawnMove), new AbstractIntent[2]
        {
            // Desperation Move
            (AbstractIntent) new HealIntent(),
            (AbstractIntent) new BuffIntent()
        })
        {
            MustPerformOnceBeforeTransitioning = true
        };
        
        // testing something
        // Find The Seeker 
        foreach (Creature creature in CombatState.Enemies)
        {
            if (creature.IsEnemy) // Again assuming Ansem is the only enemy for now
            {
                if (creature.CurrentHp <= FirstFormHp / 3)
                {
                    // Do desperation move.
                }
            }
        }
        
        
        MoveState initialState = new MoveState("ENSHROUDING_DARKNESS_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.EnshroudingDarknessMove), new AbstractIntent[1]
        {
            (AbstractIntent) new CardDebuffIntent()
        });
        
        MoveState secondInitialState = new MoveState("CONSUMING_DARKNESS_MOVE", new Func<IReadOnlyList<Creature>, Task>(this.ConsumingDarknessMove), new AbstractIntent[1]
        {
            (AbstractIntent) new BuffIntent()
        });
        
        ConditionalBranchState desperationMoveBranchState = new ConditionalBranchState("DESPERATION_MOVE_BRANCH");
        
        desperationMoveBranchState.AddState((MonsterState) this.BurnBrightForMeState, (Func<bool>) (() => !this.HasAmalgamDied));
        
        
        throw new NotImplementedException();
    }
    
    private async Task RespawnMove(IReadOnlyList<Creature> targets)
    {
        this.Respawns++;
        NRunMusicController.Instance?.UpdateMusicParameter("test_subject_progress", 2f);
        SfxCmd.Play(this.Respawns == 1 ? "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_two_heads" : "event:/sfx/enemy/enemy_attacks/test_subject/test_subject_revive_three_heads");
        await CreatureCmd.TriggerAnim(this.Creature, "RespawnTrigger", 0.0f);
        await Cmd.Wait(0.8f);
        this.Creature.GetCreatureNode()?.SetDefaultScaleTo((float) (1.0 + (double) this.Respawns * 0.10000000149011612), 0.1f);
        await Cmd.Wait(1.15f);
        this.Creature.GetPower<AdaptablePower>()?.DoRevive();
        switch (this.Respawns)
        {
            case 1:
                await this.Revive(this.SecondFormHp);
                PainfulStabsPower painfulStabsPower = await PowerCmd.Apply<PainfulStabsPower>((PlayerChoiceContext) new ThrowingPlayerChoiceContext(), this.Creature, 1M, this.Creature, (CardModel) null);
                break;
        }
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
    
    private async Task Revive(int baseRespawnHp)
    {
        this.AssertMutable();
        Decimal scaledHp = Creature.ScaleHpForMultiplayer((Decimal) baseRespawnHp, this.CombatState.Encounter, this.CombatState.Players.Count, this.CombatState.RunState.CurrentActIndex);
        Decimal num = await CreatureCmd.SetMaxHp(this.Creature, scaledHp);
        await CreatureCmd.Heal(this.Creature, scaledHp);
    }
    
}
