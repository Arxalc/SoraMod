using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Ancients;

public class MasterOfMasters : CustomAncientModel
{
    
    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 3;
    }
    
    protected override OptionPools MakeOptionPools => new OptionPools(
        [
            AncientOption<Nunchaku>(),
            AncientOption<Lantern>(),
            AncientOption<ArtOfWar>()
            //more relic options
        ]
    );
    
    private IEnumerable<EventOption> OptionPool1
    {
        get
        {
            return (IEnumerable<EventOption>) new EventOption[2]
            {
                RelicOption<MeldingRecipe>(),
                RelicOption<BookOfProphecies>()
            };
        }
    }

    private IEnumerable<EventOption> OptionPool2
    {
        get
        {
            return (IEnumerable<EventOption>) new EventOption[2]
            {
                RelicOption<GazingEye>(),
                RelicOption<GoldenEyes>()
            };
        }
    }
}
