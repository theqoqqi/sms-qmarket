using QMarketPlugin.Patches;

namespace QMarketPlugin.Modules;

public static partial class DailyStatistics {

    private static float DailyWageCosts => EmployeeManagerPatch.DailyWage?.Amount ?? 0;

    private static float GetDailyProfit(DailyStatisticsManager manager) {
        var statistics = manager.DailyStatisticsData;

        return statistics.CheckoutIncome
               + statistics.SupplyCosts
               + statistics.UpgradeCosts
               + statistics.BillCosts
               + statistics.RentCosts
               - DailyWageCosts;
    }
}
