using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace SoraMod.SoraModCode.Enchantments;

public class Meld : CustomEnchantmentModel
{
    // See MeldPatch for AfterPlay Logic
    
    public static readonly SavedSpireField<Meld, ModelId> TargetIdField = 
        new(() => default, "SoraMod_Meld_TargetId");

    public static readonly SavedSpireField<Meld, bool> TargetUpgradedField = 
        new(() => false, "SoraMod_Meld_TargetUpgraded");
    
    public ModelId? MeldTargetId
    {
        get => TargetIdField.Get(this);
        set => TargetIdField.Set(this, value);
    }

    public bool MeldTargetWasUpgraded
    {
        get => TargetUpgradedField.Get(this);
        set => TargetUpgradedField.Set(this, value);
    }
    
    
    
    /* Maybe make this expendable for balance...
        if (this.Status != EnchantmentStatus.Normal)
            return;
        this.Status = EnchantmentStatus.Disabled;
*/
}