using Core.Common.DI;
using Core.MasterFile.DI;
using Core.MasterFile.Manager;
using Core.MasterFile.Manager.Extensions;
using UnityEngine;

public class MasterFileTest : MonoBehaviour
{
    private readonly string[] _masterFilePaths = new[]
    {
        "E:\\SteamLibrary\\steamapps\\common\\Skyrim\\Data\\Skyrim.esm",
        "E:\\SteamLibrary\\steamapps\\common\\Skyrim\\Data\\Update.esm",
        "E:\\SteamLibrary\\steamapps\\common\\Skyrim\\Data\\Dawnguard.esm",
        "E:\\SteamLibrary\\steamapps\\common\\Skyrim\\Data\\HearthFires.esm",
        "E:\\SteamLibrary\\steamapps\\common\\Skyrim\\Data\\Dragonborn.esm",
    };

    private void Start()
    {
        var container = new DependencyContainer();
        container.AddModule(MasterFileModule.Create(_masterFilePaths));
        var masterFileManager = container.Resolve<MasterFileManager>();
        var loadingScreen = masterFileManager.GetRandomLoadingScreen();
        Debug.Log(loadingScreen.EditorID);
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        var cell = masterFileManager.FindCellByEditorId("DLC1ArkngthamzExterior01");
        var cellData = masterFileManager.GetCellData(cell.FormId);
        Debug.Log(cellData.CellRecord.EditorID);
        Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
        stopwatch.Reset();
        stopwatch.Start();
        var cell2 = masterFileManager.FindCellByEditorId("DLC1ArkngthamzExterior02");
        var cellData2 = masterFileManager.GetCellData(cell2.FormId);
        Debug.Log(cellData2.CellRecord.EditorID);
        Debug.Log(stopwatch.ElapsedMilliseconds);
        stopwatch.Stop();
        stopwatch.Reset();
    }
}