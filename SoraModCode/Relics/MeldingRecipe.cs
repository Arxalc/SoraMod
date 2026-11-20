using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Relics;
using SoraMod.SoraModCode.RestSite;

namespace SoraMod.SoraModCode.Relics;

public class MeldingRecipe() : SoraModRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    /*
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            return (IEnumerable<IHoverTip>) new \u003C\u003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.Cook));
        }
    }
    */
    
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != this.Owner)
            return false;
        options.Add((RestSiteOption) new MeldRestSiteOption(player));
        return true;
    }
    
}