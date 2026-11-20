using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using SoraMod.SoraModCode.Character;
using SoraMod.SoraModCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace SoraMod.SoraModCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(SoraModCardPool))]
public abstract class SoraModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    
    // Card Notes:
    /*
     * Pooh's Chapter's End must be updated for every new Link card.
     * Each Link attack must have an .Unpowered() call to not be affected by player modifiers.
     * Each Link block must do ValueProp.Unpowered to get around things like Frail
     * Unfortunately, there's not a good way to do this via custom keyword singleton so card-specific it is...
     */
    
    // TODO: Problem - Powers like Envenom, Vigor, etc. will trigger still...
    // The only way to get around this would be to either use reflection or make a "Harmony patch"? custom hook ignore powers?
}