using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace QMarketPlugin.Modules;

public static class Loans {

    private static readonly IDictionary<int, LoanInfo> LoanInfos = new Dictionary<int, LoanInfo>();

    static Loans() {
        AddLoan(100, 0, 0, "Занять у родственников");
        AddLoan(500, 0, 10, "Занять у друга");
        AddLoan(1500, 0, 25, "Занять у близкого друга");
        AddLoan(2500, 0, 40, "Занять у знакомого бизнесмена");

        AddLoan(300, 0.1f, 0, "Занять у местной группировки");
        AddLoan(2000, 0.15f, 15, "Средний займ у группировки");
        AddLoan(4000, 0.20f, 30, "Большой займ у группировки");
        AddLoan(8000, 0.25f, 45, "Крупный займ у группировки");

        AddLoan(1000, 0.01f, 5, "Малый кредит");
        AddLoan(2500, 0.02f, 20, "Средний кредит");
        AddLoan(5000, 0.03f, 35, "Большой кредит");
        AddLoan(10000, 0.04f, 50, "Профессиональный кредит");
    }

    private static void AddLoan(int sum, float dailyPercent, int requiredLevel, string title) {
        var loanId = LoanInfos.Count + 1;
        var loanInfo = new LoanInfo(title, sum, dailyPercent, requiredLevel);

        LoanInfos.Add(loanId, loanInfo);
    }

    private static void SetupLoans(IEnumerable<BankCreditSO> existingLoans) {
        var loans = new List<BankCreditSO>(existingLoans);

        CreateMissingLoans(loans);
        SetupLoanInstances(loans);
    }

    private static void CreateMissingLoans(List<BankCreditSO> loans) {
        foreach (var loanId in LoanInfos.Keys) {
            if (!LoanExists(loans, loanId)) {
                loans.Add(CreateExtraLoan(loanId));
            }
        }
    }

    private static BankCreditSO CreateExtraLoan(int loanId) {
        var loan = ScriptableObject.CreateInstance<BankCreditSO>();

        loan.ID = loanId;

        return loan;
    }

    private static void SetupLoanInstances(List<BankCreditSO> loans) {
        foreach (var loan in loans) {
            LoanInfos[loan.ID].AssignInstance(loan);
        }
    }

    private static void AddMissingLoans(List<BankCreditSO> loans) {
        foreach (var loanId in LoanInfos.Keys) {
            EnsureLoanExists(loans, loanId);
        }
    }

    private static void EnsureLoanExists(List<BankCreditSO> loans, int loanId) {
        if (!LoanExists(loans, loanId) && CanAddExtraLoan(loanId)) {
            AddExtraLoan(loans, loanId);
        }
    }

    private static void AddMissingLoanDatas(ICollection<LoanData> loanDatas, IReadOnlyList<BankCreditSO> loans) {
        while (loanDatas.Count < loans.Count) {
            var loan = loans[loanDatas.Count];

            loanDatas.Add(new LoanData {
                    LoanID = loan.ID
            });
        }
    }

    private static void AddLocalizationKeys(IDictionary<int, string> localizedLoanNames) {
        foreach (var entry in LoanInfos) {
            localizedLoanNames[entry.Key] = entry.Value.Title;
        }
    }

    private static bool LoanExists(List<BankCreditSO> loans, int id) {
        return loans.Find(loan => loan.ID == id);
    }

    private static bool CanAddExtraLoan(int loanId) {
        return LoanInfos.ContainsKey(loanId);
    }

    private static void AddExtraLoan(ICollection<BankCreditSO> loans, int loanId) {
        loans.Add(LoanInfos[loanId].Instance);
    }

    private static void SetupLoansTab(LoansTab tab) {
        var loansGameObject = tab.transform.Find("Loans").gameObject;
        var gridLayout = loansGameObject.GetComponent<GridLayoutGroup>();

        gridLayout.padding.left = 6;
        gridLayout.padding.top = -10;
        gridLayout.spacing -= new Vector2(60f, 40f);
    }

    private static void SetupLoanItem(LoanItem loanItem) {
        var rectTransform = loanItem.GetComponent<RectTransform>();

        rectTransform.localScale = new Vector3(0.75f, 0.75f, 1f);
    }
    
    private class LoanInfo {

        public BankCreditSO Instance { get; private set; }

        public readonly string Title;

        private readonly Action<BankCreditSO> Filler;

        public LoanInfo(string title, int sum, float dailyPercent, int requiredLevel) {
            Title = title;
            Filler = loan => {
                loan.Amount = sum;
                loan.DailyInterestPercent = dailyPercent;
                loan.RequiredPlayerLevel = requiredLevel;
            };
        }

        public void AssignInstance(BankCreditSO instance) {
            Instance = instance;
            Filler(instance);
        }
    }

    public static class Patches {

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

        [HarmonyPatch(typeof(LocalizationManager), "Awake")]
        [HarmonyPrefix]
        private static void Awake() {
            SetupLoans(IDManager.Instance.Loans);
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
        }

        [HarmonyPatch(typeof(BankManager), "LoadLoanDatas")]
        [HarmonyPostfix]
        private static void LoadLoanDatas(List<LoanData> ___m_LoanDatas, List<BankCreditSO> ___m_Loans) {
            AddMissingLoanDatas(___m_LoanDatas, ___m_Loans);
        }
    }
}
