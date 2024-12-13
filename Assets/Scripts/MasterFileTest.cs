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
    }
}