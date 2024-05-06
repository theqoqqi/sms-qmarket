using HarmonyLib;
using TMPro;

namespace QMarketPlugin.Modules;

public static partial class DailyStatistics {
    public static class Patches {

        [HarmonyPatch(typeof(DailyStatisticsManager), "DailyProfit", MethodType.Getter)]
        [HarmonyPrefix]
        private static void DailyProfit_Fix(
                DailyStatisticsManager __instance,
                out float __result,
                out bool __runOriginal
        ) {
            __result = GetDailyProfit(__instance);
            __runOriginal = false;
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "Start")]
        [HarmonyPostfix]
        private static void Start(
                DailyStatisticsScreen __instance,
                DailyStatisticsScreenAnimation ___m_ScreenAnimation,
                TMP_Text ___m_BillsText,
                TMP_Text ___m_BalanceText
        ) {
            SetupGui(__instance, ___m_ScreenAnimation, ___m_BillsText, ___m_BalanceText);
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "UpdateDailyStatistics")]
        [HarmonyPostfix]
        private static void UpdateDailyStatistics() {
            UpdateGui();
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "OnDayCycleDisabled")]
        [HarmonyPostfix]
        private static void OnDayCycleDisabled() {
            RemoveGui();
        }
    }
}
