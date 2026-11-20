using MegaCrit.Sts2.Core.Models;

namespace SoraMod.SoraModCode.Encounters;

// All logic lives in TeemingDarknessPower?
public sealed class Enshroud : AfflictionModel
{

    public string moddedOverlayPath = "";
    
    
    
    public string OverlayPath
    {
        get
        {
            return SceneHelper.GetScenePath("cards/overlays/afflictions/" + this.Id.Entry.ToLowerInvariant());
        }
    }

    public Control CreateOverlay()
    {
        return PreloadManager.Cache.GetScene(this.OverlayPath).Instantiate<Control>();
    }

    public bool HasOverlay => ResourceLoader.Exists(this.OverlayPath);
    
}