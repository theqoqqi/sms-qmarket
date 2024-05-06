using HarmonyLib;

namespace QMarketPlugin.Modules;

public static partial class Expenses {
    public static class Patches {

        [HarmonyPatch(typeof(ExpensesManager), "Start")]
        [HarmonyPrefix]
        private static void Start(ExpensesSO ___m_ExpensesSettings) {
            SetupRent(___m_ExpensesSettings);
        }
    }
}
