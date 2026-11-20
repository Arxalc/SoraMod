using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Keywords;

namespace SoraMod.SoraModCode.Cards;

[Pool(typeof(SoraModCardPool))]
public class BalloonBurst : ConstructedCardModel
{
        
    public BalloonBurst() : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
        WithDamage(5, 2);  // Default, will be overwritten
        WithKeyword(SoraModKeywords.Link);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .TargetingAllOpponents(this.CombatState)
            .WithHitVfxNode((Func<Creature, Node2D>)(t => (Node2D)NScratchVfx.Create(t, true)))
            .Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        this.DynamicVars.Damage.UpgradeValueBy(2M);
    }
    
    
}