using MegaCrit.Sts2.Core.Entities.Powers;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class SunsetMemoriesPower() : SoraModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    
}