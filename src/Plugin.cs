﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using QMarketPlugin.Modules;
using QMarketPlugin.Patches;

namespace QMarketPlugin; 

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Supermarket Simulator.exe")]
public class Plugin : BaseUnityPlugin {
    private Harmony harmony;

    public static Plugin Instance { get; private set; }

    private void Awake() {
        Instance = this;

        harmony = new Harmony("ru.qoqqi.qmarket.patches");
        harmony.PatchAll(typeof(EmployeeManagerPatch));
        harmony.PatchAll(typeof(CustomerManagerPatch));
        harmony.PatchAll(typeof(Loans.Patches));
        harmony.PatchAll(typeof(DynamicCustomerActivity.Patches));
        harmony.PatchAll(typeof(DailyStatistics.Patches));
        harmony.PatchAll(typeof(Expenses.Patches));
        harmony.PatchAll(typeof(DailyEvents.Patches));
        harmony.PatchAll(typeof(CustomerStrategies.Patches));

        gameObject.AddComponent<SkipOnboarding>();

        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
    }

    private void OnDestroy() {
        harmony?.UnpatchSelf();
    }

    public static ManualLogSource GetLogger() {
        return Instance.Logger;
    }
}
