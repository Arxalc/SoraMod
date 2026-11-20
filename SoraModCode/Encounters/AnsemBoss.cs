using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;

namespace SoraMod.SoraModCode.Encounters;

public class AnsemBoss : CustomEncounterModel
{
    public AnsemBoss() : base(RoomType.Boss)
    {
    }

    public override IReadOnlyList<string> Slots =>
    [
        "queen",
        "amalgam",
    ];

    public override string? CustomScenePath => "res://mod/scenes/HellEncounter.tscn";

    //Visuals and camera position of combat scene; all optional
    public override BackgroundAssets? CustomEncounterBackground(ActModel parentAct, Rng rng)
    {
        return new BackgroundAssets(ModelDb.Encounter<QueenBoss>().Id.Entry.ToLowerInvariant(), Rng);
    }
    
    public override string CustomBgm => "event:/music/act3_boss_queen";
    public override float GetCameraScaling() => 0.85f;
    public override Vector2 GetCameraOffset() => Vector2.Down * 60f;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<TorchHeadAmalgam>().ToMutable(), "amalgam"),
        (ModelDb.Monster<Queen>().ToMutable(), "queen"),
    ];

    public override IEnumerable<MonsterModel> AllPossibleMonsters => 
    [
        ModelDb.Monster<Queen>(),
        ModelDb.Monster<TorchHeadAmalgam>(),
    ];

    //An image must be set if making a boss. Otherwise, it will attempt to load a boss-specific image using the encounter name from basegame resources and fail.
    public override string? CustomRunHistoryIconOutlinePath => ImageHelper.GetImagePath($"ui/run_history/{ModelDb.Encounter<QueenBoss>().Id.Entry.ToLowerInvariant()}.png");
    public override string? CustomRunHistoryIconPath => ImageHelper.GetImagePath($"ui/run_history/{ModelDb.Encounter<QueenBoss>().Id.Entry.ToLowerInvariant()}_outline.png");

    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 1;
    }
}