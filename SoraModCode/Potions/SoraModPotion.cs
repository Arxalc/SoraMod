using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Extensions;

namespace SoraMod.SoraModCode.Potions;

[Pool(typeof(SoraModPotionPool))]
public abstract class SoraModPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();

    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}