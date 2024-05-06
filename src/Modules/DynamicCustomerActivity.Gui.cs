using QMarketPlugin.Utils;
using TMPro;
using UnityEngine;

namespace QMarketPlugin.Modules;

internal static partial class DynamicCustomerActivity {

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
}
