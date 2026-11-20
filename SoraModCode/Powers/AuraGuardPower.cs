using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Cards;


public class AuraGuardPower() : TemporaryStrengthPower
{
    // Do I have to do something to make sure it's associated with my mod?
    
    public override AbstractModel OriginModel => (AbstractModel) ModelDb.Card<AuraGuard>();
    
}