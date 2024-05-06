using System.Collections.Generic;
using System.Linq;
using QMarketPlugin.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

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

    private static bool guiInitialized;

    private static TextMeshProUGUI currentDayText;
    
    private static TextMeshProUGUI currentWeekdayText;
    
    private static TextMeshProUGUI activityText;

    public static void UpdateGui() {
        if (!guiInitialized) {
            AddGui();
            guiInitialized = true;
        }

        SetCurrentDayText(CurrentDay);
        SetCurrentWeekdayText(CurrentDay);
        SetActivityText(CurrentActivity);
    }

    public static ItemQuantity ModifyShoppingList(ItemQuantity itemQuantity) {
        var isLargePurchase = Random.value < Weekday.Of(CurrentDay).LargePurchaseChance;
        
        if (isLargePurchase) {
            HandleLargePurchase(itemQuantity);
        }

        return itemQuantity;
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

    private static void AddGui() {
        var panel = AddPanel();

        currentDayText = AddCurrentDayText(panel.transform);
        currentWeekdayText = AddCurrentWeekdayText(panel.transform);
        activityText = AddActivityText(panel.transform);
    }

    private static GameObject AddPanel() {
        var original = GameObject.Find("Time BG");

        var panelGameObject = Object.Instantiate(original, original.transform.parent);
        panelGameObject.name = "Day UI";

        var panelTransform = (RectTransform) panelGameObject.transform;
        panelTransform.position = Vector3.zero;
        panelTransform.rotation = Quaternion.identity;
        panelTransform.localScale = new Vector3(0.9f, 1f, 1f);
        panelTransform.anchoredPosition = new Vector2(0f, -50f);
        panelTransform.SetSiblingIndex(0);

        return panelGameObject;
    }

    private static TextMeshProUGUI AddCurrentDayText(Transform parent) {
        var textGameObject = GameObjectUtils.CreateText(parent, "Current Day Text", 20f);
        var textTransform = textGameObject.GetComponent<RectTransform>();

        textTransform.anchorMax = new Vector2(1f, 1f);

        return textGameObject.GetComponent<TextMeshProUGUI>();
    }

    private static TextMeshProUGUI AddCurrentWeekdayText(Transform parent) {
        var textGameObject = GameObjectUtils.CreateText(parent, "Current Weekday Text", 12f);
        var textTransform = textGameObject.GetComponent<RectTransform>();

        textTransform.anchorMax = new Vector2(1f, 0.6f);

        return textGameObject.GetComponent<TextMeshProUGUI>();
    }

    private static TextMeshProUGUI AddActivityText(Transform parent) {
        var textGameObject = GameObjectUtils.CreateText(parent, "Activity Text", 12f);
        var textTransform = textGameObject.GetComponent<RectTransform>();

        textTransform.anchorMax = new Vector2(1f, 0.3f);

        return textGameObject.GetComponent<TextMeshProUGUI>();
    }

    private static void SetCurrentDayText(int day) {
        currentDayText.text = $"День: {day}";
    }

    private static void SetCurrentWeekdayText(int day) {
        var color = GetWeekdayColor(day);
        
        currentWeekdayText.text = color.AsHtmlTag() + Weekday.Of(day).Title;
    }

    private static Color GetWeekdayColor(int day) {
        return Weekday.Of(day).IsWeekend
                ? new Color(0f, 0.8f, 0f)
                : Color.white;
    }

    private static void SetActivityText(float value) {
        var color = GetActivityColor(value);
        
        activityText.text = $"Активность: {color.AsHtmlTag()}{value * 100:0}";
    }

    private static Color GetActivityColor(float value) {
        var rescaledValue = (value - 0.5f) * 0.5f;
        var clampedValue = Mathf.Clamp(rescaledValue, 0, 0.5f);

        return Color.HSVToRGB(clampedValue, 0.5f + clampedValue, 0.8f);
    }

    private static float UpdateCurrentActivity() {
        var randomFactor = 1 + (Random.value - Random.value) * 0.05f;

        CurrentActivity = GetHourlyRate(CurrentHour)
                          * Weekday.Of(CurrentDay).ActivityRate
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
