using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QMarketPlugin.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

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

    private static bool guiInitialized;

    private static TextMeshProUGUI activityText;

    public static void UpdateGui() {
        if (!guiInitialized) {
            AddGui();
            guiInitialized = true;
        }

        SetActivityText(CurrentActivity);
    }

    private static void AddGui() {
        var panel = AddPanel();

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
        panelTransform.anchoredPosition = new Vector2(0f, -15f);
        panelTransform.SetSiblingIndex(0);

        return panelGameObject;
    }

    private static TextMeshProUGUI AddActivityText(Transform parent) {
        var textGameObject = GameObjectUtils.CreateText(parent, "Activity Text", 12f);
        var textTransform = textGameObject.GetComponent<RectTransform>();

        textTransform.anchorMax = new Vector2(1f, 0.3f);

        return textGameObject.GetComponent<TextMeshProUGUI>();
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

            UpdateGui();
        }
    
        [HarmonyPatch(typeof(DayCycleManager), "StartNextDay")]
        [HarmonyPostfix]
        public static void StartNextDay() {
            UpdateGui();
        }
    }
}
