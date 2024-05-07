using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QMarketPlugin.Modules;

internal static partial class DynamicCustomerActivity {

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
    
    public static float CurrentActivity { get; private set; }

    public static void ModifyShoppingList(ref ItemQuantity itemQuantity) {
        if (IsLargePurchase()) {
            HandleLargePurchase(itemQuantity);
        }
    }

    private static bool IsLargePurchase() {
        return Random.value < Weekday.Of(CurrentDay).LargePurchaseChance;
    }

    private static void HandleLargePurchase(ItemQuantity itemQuantity) {
        itemQuantity.Products = itemQuantity.Products.ToDictionary(
                pair => pair.Key,
                pair => pair.Value + GetRandomExtraProductCount()
        );
    }

    private static int GetRandomExtraProductCount() {
        var count = 1;

        while (Random.value < 0.5f) {
            count++;
        }

        return count;
    }

    private static float UpdateCurrentActivity() {
        var randomFactor = 1 + (Random.value - Random.value) * 0.05f;

        CurrentActivity = GetHourlyRate(CurrentHour)
                          * Weekday.Of(CurrentDay).ActivityRate
                          * GetLicenseFactor()
                          * randomFactor;

        return CurrentActivity;
    }

    private static float GetHourlyRate(int hour) {
        return GetHourWeight(hour) / AverageHourlyWeight;
    }

    private static float GetHourWeight(int hour) {
        return HourlyWeights.TryGetValue(hour, out var weight)
                ? weight : AverageHourlyWeight;
    }
}
