using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace QMarketPlugin.Modules;

public static partial class DailyEvents {

    private static GameObject gui;

    private static TMP_Text descriptionText;

    private static TMP_Text sumText;

    private static TMP_Text deadlineText;

    // ReSharper disable Unity.PerformanceAnalysis
    private static void SetupGui() {
        var ingameCanvas = GameObject.Find("Ingame Canvas");
        var template = GameObject.Find("---MANAGERS---")
                .transform
                .Find("Mission System")
                .Find("Mission Canvas")
                .Find("Checkout Mission")
                .gameObject;

        CreateGui(template, ingameCanvas);
        PlaceGui();
        SetupTexts();
        HideGui();
    }

    private static void CreateGui(GameObject template, GameObject ingameCanvas) {
        gui = Object.Instantiate(template, ingameCanvas.transform);
        gui.name = "Daily Event UI";
    }

    private static void PlaceGui() {
        var rectTransform = gui.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.anchorMin = new Vector2(0.82f, 0.99f);
        rectTransform.anchorMax = new Vector2(0.82f, 0.99f);
        rectTransform.offsetMin = new Vector2(-300f, -180f);
        rectTransform.offsetMax = new Vector2(0.00f, 0f);
    }

    private static void SetupTexts() {
        SetupText("Title", "Panel Title", 18f, "Непредвиденные расходы");

        descriptionText = SetupText("Objective Text", "Description Text", 16f);
        sumText = SetupText("Counter Text", "Sum Text", 24f);
        deadlineText = CreateText(descriptionText, "Deadline Text", 14f);

        descriptionText.GetComponent<RectTransform>().anchoredPosition += Vector2.up * 10f;
        sumText.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 32f;
        deadlineText.GetComponent<RectTransform>().anchoredPosition += Vector2.down * 96f;
    }

    private static TMP_Text CreateText(TMP_Text template, string name, float fontSize) {
        var text = Object.Instantiate(template, gui.transform);

        SetupText(text.gameObject, name, fontSize);

        return text;
    }

    private static TMP_Text SetupText(string oldName, string newName, float fontSize, string initialText = "") {
        var gameObject = gui.transform.Find(oldName).gameObject;

        Object.Destroy(gameObject.GetComponent<LocalizeStringEvent>());

        return SetupText(gameObject, newName, fontSize, initialText);
    }

    private static TMP_Text SetupText(GameObject gameObject, string newName, float fontSize, string initialText = "") {
        var text = gameObject.GetComponent<TMP_Text>();

        gameObject.name = newName;
        text.text = initialText;
        text.fontSize = fontSize;
        text.enableAutoSizing = false;

        return text;
    }

    private static void ShowGui(DailyEvent dailyEvent) {
        gui.SetActive(true);

        descriptionText.text = dailyEvent.Title;
        sumText.text = $"${dailyEvent.Sum:0}";
        deadlineText.text = $"Деньги надо найти к {dailyEvent.DeadlineHour}:00";
    }

    private static void HideGui() {
        gui.SetActive(false);
    }
}
