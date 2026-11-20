using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class Aeroga() : SoraModCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AerogaPower>(1M)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SoraModKeywords.Magic]; 
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AerogaPower>()];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        AerogaPower aerogaPower = await PowerCmd.Apply<AerogaPower>(choiceContext, this.Owner.Creature, this.DynamicVars["AerogaPower"].BaseValue, this.Owner.Creature, (CardModel) this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["AerogaPower"].UpgradeValueBy(1M);
    }
}