using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;

public class HeroicSwingPower() : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => (AbstractModel) ModelDb.Card<HeroicSwing>();
}