using Core.Common.DI;
using Core.MasterFile.Converter.Cell;
using Core.MasterFile.DI;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using UnityEngine;

public class MasterFileTest : MonoBehaviour
{
    private readonly string[] _masterFilePaths = new[]
    {
        "/Users/suslanium/RiderProjects/SkyrimFiles/Skyrim/Data/Skyrim.esm",
        "/Users/suslanium/RiderProjects/SkyrimFiles/Skyrim/Data/Update.esm",
        "/Users/suslanium/RiderProjects/SkyrimFiles/Skyrim/Data/Dawnguard.esm",
        "/Users/suslanium/RiderProjects/SkyrimFiles/Skyrim/Data/HearthFires.esm",
        "/Users/suslanium/RiderProjects/SkyrimFiles/Skyrim/Data/Dragonborn.esm",
    };

    private void Start()
    {
        var container = new DependencyContainer();
        container.AddModule(MasterFileModule.Create(_masterFilePaths));
        var masterFileManager = container.Resolve<MasterFileManager>();
        var cellConverter = container.Resolve<CellConverter>();
        var loadingScreen = masterFileManager.GetRandomLoadingScreen();
        Debug.Log(loadingScreen.EditorID);
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var cell = masterFileManager.FindCellByEditorId("DLC1ArkngthamzExterior01");
        var cellData = masterFileManager.GetCellData(cell.FormId);
        var convertedCellData = cellConverter.ConvertRawCellData(cellData);
        Debug.Log(cellData.CellRecord.EditorID);
        Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
        stopwatch.Reset();
        stopwatch.Start();
        var cell2 = masterFileManager.FindCellByEditorId("DLC1ArkngthamzExterior02");
        var cellData2 = masterFileManager.GetCellData(cell2.FormId);
        var convertedCellData2 = cellConverter.ConvertRawCellData(cellData2);
        Debug.Log(cellData2.CellRecord.EditorID);
        Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
        stopwatch.Reset();
    }
}