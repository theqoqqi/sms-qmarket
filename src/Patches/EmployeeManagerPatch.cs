using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MyBox;

namespace QMarketPlugin.Patches;

public static class EmployeeManagerPatch {

    public static PlayerPaymentData DailyWage => GetDailyWage(cashiersData, restockersData);

    private static List<int> cashiersData = new List<int>();

    private static List<int> restockersData = new List<int>();

    [HarmonyPatch(typeof(EmployeeManager), "LoadData")]
    [HarmonyPostfix]
    private static void LoadData(List<int> ___m_CashiersData, List<int> ___m_RestockersData) {
        SetEmployeesData(___m_CashiersData, ___m_RestockersData);
    }

    [HarmonyPatch(typeof(EmployeeManager), "DailyWage", MethodType.Getter)]
    [HarmonyPrefix]
    private static void DailyWage_Fix(
            out PlayerPaymentData __result,
            out bool __runOriginal,
            List<int> ___m_CashiersData,
            List<int> ___m_RestockersData
    ) {
        SetEmployeesData(___m_CashiersData, ___m_RestockersData);

        __result = DailyWage;
        __runOriginal = false;

        if (__result != null) {
            Withdraw(__result.Amount);
        }
    }

    private static void Withdraw(float amount) {
        Singleton<MoneyManager>.Instance.MoneyTransition(
                -(float) Math.Round(amount, 2),
                MoneyManager.TransitionType.STAFF
        );
    }

    private static void SetEmployeesData(List<int> ___m_CashiersData, List<int> ___m_RestockersData) {
        cashiersData = ___m_CashiersData;
        restockersData = ___m_RestockersData;
    }

    private static PlayerPaymentData GetDailyWage(
            IReadOnlyCollection<int> cashiers,
            IReadOnlyCollection<int> restockers
    ) {
        var idManager = Singleton<IDManager>.Instance;

        if (!cashiers.Any() && !restockers.Any()) {
            return null;
        }

        var cashiersWage = cashiers
                .Select(id => idManager.CashierSO(id))
                .Select(cashierSo => cashierSo.DailyWage)
                .Sum();

        var restockersWage = restockers
                .Select(id => idManager.RestockerSO(id))
                .Select(restockerSo => restockerSo.DailyWage)
                .Sum();

        var wage = cashiersWage + restockersWage;

        return new PlayerPaymentData {
                Amount = wage,
                PaymentType = PlayerPaymentType.STAFF
        };
    }
}
