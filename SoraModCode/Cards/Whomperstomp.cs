using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

// This is actually called Whomperstomp in game.
public class Whomperstomp() : SoraModCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // Claw-ish. Maybe I should buff every Meow Wounce card on attack. Meow wow bombs...
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Link];
    
    private const string _increaseKey = "Increase";
    private Decimal _extraDamageFromWouncePlays;
    
    
    private Decimal ExtraDamageFromWouncePlays
    {
        get => this._extraDamageFromWouncePlays;
        set
        {
            this.AssertMutable();
            this._extraDamageFromWouncePlays = value;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5M, ValueProp.Move), new DynamicVar("Increase", 2M)];
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull((object) play.Target, "cardPlay.Target");
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue).FromCard((CardModel) this, play).Targeting(play.Target).WithHitVfxNode((Func<Creature, Node2D>) (t => (Node2D) NScratchVfx.Create(t, true))).Execute(choiceContext);
        Decimal baseValue = this.DynamicVars["Increase"].BaseValue;
        this.BuffFromWouncePlay(baseValue);
    }

    protected override void OnUpgrade()
    {

    }
    
    private void BuffFromWouncePlay(Decimal extraDamage)
    {
        DamageVar damage = this.DynamicVars.Damage;
        damage.BaseValue = damage.BaseValue + extraDamage;
        this.ExtraDamageFromWouncePlays += extraDamage;
    }
    
    // After playing, return to hand.
    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        location.player = Owner;
        return location;
    }
    
    public override bool HasTurnEndInHandEffect => true;
    // Maybe give it Ethereal to prevent spam?
    protected override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat((CardModel) this.CombatState.CreateCard<MeowWounce>(this.Owner), PileType.Draw, this.Owner));
    }
}