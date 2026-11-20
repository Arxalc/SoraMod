using BaseLib.Abstracts;
using SoraMod.SoraModCode.Extensions;
using Godot;

namespace SoraMod.SoraModCode.Character;

public class SoraModCardPool : CustomCardPoolModel
{
	public override string Title => SoraMod.CharacterId; //This is not a display name.

	public override string BigEnergyIconPath => "charui/another_heart_icon.png".ImagePath(); // ... why didn't this show
	public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

	// {NameOfVar:diff()}
	// {Damage:diff()}
	// {IfUpgraded:show:upgraded text|not upgraded text}

	/* These HSV values will determine the color of your card back.
	They are applied as a shader onto an already colored image,
	so it may take some experimentation to find a color you like.
	Generally they should be values between 0 and 1. */
	public override Color ShaderColor => new Color("#00eaff");
	public override float H => ShaderColor.H; //Hue; changes the color.
	public override float S => ShaderColor.S; //Saturation
	public override float V => ShaderColor.V; //Brightness

	//Alternatively, leave these values at 1 and provide a custom frame image.
	/*public override Texture2D CustomFrame(CustomCardModel card)
	{
		//This will attempt to load SoraMod/images/cards/frame.png
		return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
	}*/

	//Color of small card icons
	public override Color DeckEntryCardColor => new("00eaff");

	public override bool IsColorless => false;
}
