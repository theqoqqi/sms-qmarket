using System.Collections.Generic;
using HarmonyLib;

namespace QMarketPlugin.Modules;

public static partial class CustomerStrategies {
    public static class Patches {
        
        [HarmonyPatch(typeof(CustomerStrategiesManager), "CurrentStrategy", MethodType.Getter)]
        [HarmonyPostfix]
        public static void CurrentStrategy(List<CustomerStrategySO> ___m_CustomerStrategies) {
            Setup(___m_CustomerStrategies);
        }
    }
}
