using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace SoraMod.SoraModCode.Maps;

using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;

public sealed class BookPathActMap : ActMap
{
    // See NTopBarBossIcon.cs for refreshing top bar boss icon 
    // golden compass map has 3 monsters, 2 elites, 1 shop, 4 campfires, 4 unknowns, 2 treasures
    // this currently has 4 monsters, 2 elites, 1 shop, 4 campfires, 3 unknowns, 2 treasures
    // TODO: Add second boss if asension 10!!
    private readonly MapPointType[] _defaultPointTypes = new MapPointType[16 /*0x10*/]
    {
        MapPointType.Monster,
        MapPointType.Monster,
        MapPointType.Monster,
        MapPointType.Monster,
        MapPointType.RestSite,
        MapPointType.Unknown,
        MapPointType.Elite,
        MapPointType.Treasure,
        MapPointType.RestSite,
        MapPointType.Unknown,
        MapPointType.Elite,
        MapPointType.Treasure,
        MapPointType.RestSite,
        MapPointType.Unknown,        
        MapPointType.Shop,
        MapPointType.RestSite
    };
    private const int _width = 7;
    private const int _middle = 3;

    public override MapPoint BossMapPoint { get; }

    public override MapPoint StartingMapPoint { get; }

    protected override MapPoint?[,] Grid { get; }

    public BookPathActMap(IRunState runState)
    {
        List<MapPointType> list = ((IEnumerable<MapPointType>) this._defaultPointTypes).ToList<MapPointType>();
        if (runState.Players.Count > 1)
            list.RemoveAt(2);
        this.Grid = new MapPoint[7, list.Count + 1];
        this.BossMapPoint = new MapPoint(this.GetColumnCount() / 2, this.GetRowCount())
        {
            PointType = MapPointType.Boss
        };
        this.StartingMapPoint = new MapPoint(this.GetColumnCount() / 2, 0)
        {
            PointType = MapPointType.Ancient
        };
        for (int index = 0; index < list.Count; ++index)
        {
            MapPoint child = new MapPoint(3, index + 1);
            this.Grid[3, index + 1] = child;
            child.PointType = list[index];
            if (index > 0)
                this.Grid[3, index].AddChildPoint(child);
        }
        this.startMapPoints.Add(this.Grid[3, 1]);
        this.Grid[3, this.GetRowCount() - 1].AddChildPoint(this.BossMapPoint);
        this.StartingMapPoint.AddChildPoint(this.Grid[3, 1]);
        
    }
}
