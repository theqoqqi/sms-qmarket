using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using QMarketPlugin.Patches;
using QMarketPlugin.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace QMarketPlugin.Modules;

public static class DailyStatistics {

    private static TMP_Text billsText;

    private static TMP_Text dailyWageText;

    private static TMP_Text balanceText;

    private static float DailyWageCosts => EmployeeManagerPatch.DailyWage?.Amount ?? 0;

    private static DailyStatisticsScreen screen;

    private static void SetupGui(
            DailyStatisticsScreen screen,
            DailyStatisticsScreenAnimation animation,
            TMP_Text billsText,
            TMP_Text balanceText
    ) {
        DailyStatistics.screen = screen;
        DailyStatistics.billsText = billsText;
        dailyWageText = AddDailyWageLine(billsText);
        DailyStatistics.balanceText = balanceText;

        AnimateNewGui(animation);
    }

    private static void AnimateNewGui(DailyStatisticsScreenAnimation animation) {
        var animatedElements = GetAnimatedElements(animation);

        animatedElements.InsertAfter(GetCanvasGroup(dailyWageText), GetCanvasGroup(billsText));

        SetAnimatedElements(animation, animatedElements);
    }

    private static List<CanvasGroup> GetAnimatedElements(DailyStatisticsScreenAnimation animation) {
        return ReflectionUtils.GetNonPublicFieldValue<CanvasGroup[]>(animation, "m_UIElements").ToList();
    }

    private static void SetAnimatedElements(DailyStatisticsScreenAnimation animation, List<CanvasGroup> value) {
        ReflectionUtils.SetNonPublicFieldValue(animation, "m_UIElements", value.ToArray());
    }

    private static CanvasGroup GetCanvasGroup(TMP_Text textComponent) {
        return textComponent.transform.parent.GetComponent<CanvasGroup>();
    }

    private static void UpdateGui() {
        SetMoneyText(dailyWageText, -DailyWageCosts);
        SetMoneyText(balanceText, MoneyManager.Instance.Money - DailyWageCosts);
    }

    private static void RemoveGui() {
        Object.Destroy(dailyWageText);
    }

    private static float GetDailyProfit(DailyStatisticsManager manager) {
        var statistics = manager.DailyStatisticsData;

        return statistics.CheckoutIncome
               + statistics.SupplyCosts
               + statistics.UpgradeCosts
               + statistics.BillCosts
               + statistics.RentCosts
               - DailyWageCosts;
    }

    private static void SetMoneyText(TMP_Text textComponent, float amount) {
        textComponent.text = GetPreCode(amount) + amount.ToMoneyText(textComponent.fontSize);
    }

    private static string GetPreCode(float forAmount) {
        var method = ReflectionUtils.GetNonPublicMethod(screen, "StatisticPreCode", typeof(float));

        return (string) method?.Invoke(screen, new object[] { forAmount });
    }

    private static TMP_Text AddDailyWageLine(TMP_Text billsText) {

        var billsLine = billsText.transform.parent.gameObject;
        var container = billsLine.transform.parent.gameObject;
        var dailyWageLine = Object.Instantiate(billsLine, container.transform, true);

        billsLine.transform.position += Vector3.up * 5;
        dailyWageLine.transform.position += Vector3.down * 20;

        var dailyWageTitle = dailyWageLine.GetComponent<TMP_Text>();
        var dailyWageText = dailyWageLine.transform.Find("Upgrades Text").GetComponent<TMP_Text>();

        dailyWageTitle.text = "Персонал:";
        dailyWageText.text = "0";

        Object.Destroy(dailyWageLine.GetComponent<LocalizeStringEvent>());

        return dailyWageText;
    }

    public static class Patches {

        [HarmonyPatch(typeof(DailyStatisticsManager), "DailyProfit", MethodType.Getter)]
        [HarmonyPrefix]
        private static void DailyProfit_Fix(
                DailyStatisticsManager __instance,
                out float __result,
                out bool __runOriginal
        ) {
            __result = GetDailyProfit(__instance);
            __runOriginal = false;
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "Start")]
        [HarmonyPostfix]
        private static void Start(
                DailyStatisticsScreen __instance,
                DailyStatisticsScreenAnimation ___m_ScreenAnimation,
                TMP_Text ___m_BillsText,
                TMP_Text ___m_BalanceText
        ) {
            SetupGui(__instance, ___m_ScreenAnimation, ___m_BillsText, ___m_BalanceText);
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "UpdateDailyStatistics")]
        [HarmonyPostfix]
        private static void UpdateDailyStatistics() {
            UpdateGui();
        }

        [HarmonyPatch(typeof(DailyStatisticsScreen), "OnDayCycleDisabled")]
        [HarmonyPostfix]
        private static void OnDayCycleDisabled() {
            RemoveGui();
        }
    }
}
