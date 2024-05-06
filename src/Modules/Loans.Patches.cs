using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine.UI;

namespace QMarketPlugin.Modules;

public static partial class Loans {
    public static class Patches {

        [HarmonyPatch(typeof(LocalizationManager), "Awake")]
        [HarmonyPrefix]
        private static void Awake() {
            Loans.Setup();
        }

        [HarmonyPatch(typeof(LoansTab), "Start")]
        [HarmonyPostfix]
        private static void Setup(LoansTab __instance) {
            SetupLoansTab(__instance);
        }

        [HarmonyPatch(typeof(LoanItem), "Setup")]
        [HarmonyPostfix]
        private static void Setup(LoanItem __instance) {
            SetupLoanItem(__instance);
        }
        
        [HarmonyPatch(typeof(LoanItem), "UpdateAvailableLayoutUI")]
        [HarmonyPostfix]
        private static void UpdateAvailableLayoutUI(
                BankCreditSO ___m_Loan,
                TMP_Text ___m_DailyInterestText,
                Button ___m_LoanButton
        ) {
            UpdateLoanAvailableLayout(___m_Loan, ___m_DailyInterestText, ___m_LoanButton);
        }

        [HarmonyPatch(typeof(LocalizationManager), "UpdateLocalization")]
        [HarmonyPostfix]
        private static void UpdateLocalization(Dictionary<int, string> ___m_LocalizedLoanNames) {
            AddLocalizationKeys(___m_LocalizedLoanNames);
        }

        [HarmonyPatch(typeof(IDManager), "BankCreditSO")]
        [HarmonyPrefix]
        private static void BankCreditSo(List<BankCreditSO> ___m_BankCredits) {
            AddMissingLoans(___m_BankCredits);
        }

        [HarmonyPatch(typeof(BankManager), "Start")]
        [HarmonyPrefix]
        private static void Setup(List<BankCreditSO> ___m_Loans) {
            AddMissingLoans(___m_Loans);
            AddLoanListeners();
        }

        [HarmonyPatch(typeof(BankManager), "LoadLoanDatas")]
        [HarmonyPostfix]
        private static void LoadLoanDatas(List<LoanData> ___m_LoanDatas, List<BankCreditSO> ___m_Loans) {
            AddMissingLoanDatas(___m_LoanDatas, ___m_Loans);
        }
    }
}
