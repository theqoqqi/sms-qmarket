using HarmonyLib;

namespace QMarketPlugin.Modules;

public static class DailyStatistics {

    private static float GetDailyProfit(DailyStatisticsManager manager) {
        var statistics = manager.DailyStatisticsData;

        return statistics.CheckoutIncome
               + statistics.SupplyCosts
               + statistics.UpgradeCosts
               + statistics.BillCosts
               + statistics.RentCosts;
    }

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
    }
}
