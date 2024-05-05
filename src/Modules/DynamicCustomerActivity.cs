using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace QMarketPlugin.Modules;

internal static class DynamicCustomerActivity {

    private static readonly Dictionary<int, float> HourlyWeights = new Dictionary<int, float> {
            { 8, 0.1f },
            { 9, 0.14f },
            { 10, 0.17f },
            { 11, 0.18f },

            { 12, 0.18f },
            { 13, 0.18f },
            { 14, 0.19f },
            { 15, 0.20f },

            { 16, 0.21f },
            { 17, 0.22f },
            { 18, 0.22f },
            { 19, 0.21f },

            { 20, 0.19f },
    };

    private static float AverageHourlyWeight => HourlyWeights.Values.Sum() / HourlyWeights.Count;

    private static DayCycleManager Manager => DayCycleManager.Instance;

    private static int CurrentDay => Manager.CurrentDay;

    private static int CurrentHour => Manager.CurrentHour + (Manager.AM || Manager.CurrentHour == 12 ? 0 : 12);
    
    public static float CurrentActivity => GetHourlyRate(CurrentHour) * Weekday.Of(CurrentDay).ActivityRate;

    private static float GetHourlyRate(int hour) {
        return GetHourWeight(hour) / AverageHourlyWeight;
    }

    private static float GetHourWeight(int hour) {
        return HourlyWeights.TryGetValue(hour, out var weight)
                ? weight : AverageHourlyWeight;
    }

    public class Weekday {
        
        public static readonly Dictionary<int, Weekday> Weekdays = new Dictionary<int, Weekday> {
                {0, new Weekday(0.8f)},
                {1, new Weekday(0.95f)},
                {2, new Weekday(0.85f)},
                {3, new Weekday(0.9f)},
                {4, new Weekday(1f)},
                {5, new Weekday(1.3f)},
                {6, new Weekday(1.2f)},
        };

        public readonly float ActivityRate;

        public Weekday(float activityRate) {
            ActivityRate = activityRate;
        }

        public static Weekday Of(int day) {
            return Weekdays[day % Weekdays.Count];
        }
    }
    
    public static class Patches {

        [HarmonyPatch(typeof(CustomerSpawnSettingManager), "GetCustomerSpawningTime")]
        [HarmonyPostfix]
        public static void GetCustomerSpawningTime(ref float __result) {
            __result /= CurrentActivity;
        }
    }
}
