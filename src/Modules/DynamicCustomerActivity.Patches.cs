using HarmonyLib;

namespace QMarketPlugin.Modules;

internal static partial class DynamicCustomerActivity {
    public static class Patches {

        [HarmonyPatch(typeof(DayCycleManager), "Start")]
        [HarmonyPostfix]
        public static void Start() {
            UpdateCurrentActivity();
            UpdateGui();
        }

        [HarmonyPatch(typeof(CustomerSpawnSettingManager), "GetCustomerSpawningTime")]
        [HarmonyPostfix]
        public static void GetCustomerSpawningTime(ref float __result) {
            __result /= UpdateCurrentActivity();

            UpdateGui();
        }
    
        [HarmonyPatch(typeof(DayCycleManager), "StartNextDay")]
        [HarmonyPostfix]
        public static void StartNextDay() {
            UpdateCurrentActivity();
            UpdateGui();
        }
    
        [HarmonyPatch(typeof(CustomerManager), "CreateShoppingList")]
        [HarmonyPostfix]
        public static void CreateShoppingList(ref ItemQuantity __result) {
            ModifyShoppingList(ref __result);
        }
    }
}
