using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using SoraMod.SoraModCode.Maps;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Relics;

public class BookOfProphecies() : SoraModRelic
{
    // Golden compass
    private int _bookPathAct = -1;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;
    
    [SavedProperty]
    public int BookPathAct
    {
        get => this._bookPathAct;
        set
        {
            this.AssertMutable();
            this._bookPathAct = value;
        }
    }
    
    public override async Task AfterObtained()
    {
        this.BookPathAct = this.Owner.RunState.CurrentActIndex;
        await RunManager.Instance.GenerateMap();
    }
    
    
    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        ActModel act = runState.Acts[actIndex];
        EncounterModel currentBoss1 = act.BossEncounter; // .Id ???
        //EncounterModel currentBoss2 = act.SecondBossEncounter;
        
        // TODO: Make sure this sets Act 3's second boss if higher ascension
        /*
        if (actIndex == runState.Acts.Count - 1 && RunManager.Instance.AscensionManager.HasLevel(AscensionLevel.DoubleBoss))
        {
            EncounterModel encounter = runState.Rng.UpFront.NextItem<EncounterModel>(act.AllBossEncounters.Where<EncounterModel>((Func<EncounterModel, bool>) (e => e.Id != act.BossEncounter.Id)));
            act.SetSecondBossEncounter(encounter);
        }
        */
        
        // I'm assuming the second boss is set for now...
        
        // Change the act boss to something else... maybe xehanort in the future
        
        // This is not disregarding current boss from random pool...? I think?? tbf I did try rerolling multiple times...
        // This isn't changing the boss icon though.
        EncounterModel newBoss = runState.Rng.UpFront.NextItem<EncounterModel>(act.AllBossEncounters.Where<EncounterModel>((Func<EncounterModel, bool>) (e => e.Id != act.BossEncounter.Id)));
        act.SetBossEncounter(newBoss);
        
        NGlobalUi globalUi = NRun.Instance.GlobalUi;
        globalUi.TopBar.BossIcon.RefreshBossIcon();
        
        return this.BookPathAct != actIndex ? map : new BookPathActMap(runState);
        
    }
        
}