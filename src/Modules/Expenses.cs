namespace QMarketPlugin.Modules;

public static partial class Expenses {

    private const float RentMultiplier = 5;

    private static bool initialized;

    private static void SetupRent(ExpensesSO expenses) {
        if (initialized) {
            return;
        }

        MultiplyRent(expenses);

        initialized = true;
    }

    private static void MultiplyRent(ExpensesSO expenses) {
        expenses.DefaultDailyRent *= RentMultiplier;

        foreach (var section in IDManager.Instance.Sections) {
            section.DailyRentAddition *= RentMultiplier;
        }
    }
}
