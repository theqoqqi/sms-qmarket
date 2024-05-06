using HarmonyLib;

namespace QMarketPlugin.Modules;

public static partial class DailyEvents {
    public static class Patches {

        [HarmonyPatch(typeof(SaveManager), "Load")]
        [HarmonyPostfix]
        public static void Load(string ___m_CurrentSaveFilePath) {
            if (!string.IsNullOrEmpty(___m_CurrentSaveFilePath)) {
                Setup(___m_CurrentSaveFilePath);
            }
        }

        [HarmonyPatch(typeof(DayCycleManager), "StartNextDay")]
        [HarmonyPostfix]
        public static void StartNextDay() {
            OnStartNextDay();
        }

        [HarmonyPatch(typeof(DayCycleManager), "UpdateGameTime")]
        [HarmonyPostfix]
        public static void UpdateGameTime() {
            Update();
        }
    }
}
