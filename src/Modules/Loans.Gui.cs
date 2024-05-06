using System;
using System.Collections.Generic;
using QMarketPlugin.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace QMarketPlugin.Modules;

public static partial class Loans {
    private static void AddLocalizationKeys(IDictionary<int, string> localizedLoanNames) {
        foreach (var entry in LoanInfos) {
            localizedLoanNames[entry.Key] = entry.Value.Title;
        }
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

    private static void UpdateLoanAvailableLayout(
            BankCreditSO loan,
            TMP_Text dailyInterestText,
            Button activateButton
    ) {
        dailyInterestText.text = $"{loan.DailyInterestPercent * 100}%";

        SetLoanButtonActive(activateButton, CanTakeLoan(loan));
    }

    private static void SetLoanButtonActive(Button button, bool isActive) {
        var activeColor = new Color(0.063f, 0.708f, 0.3f);
        var lockedColor = new Color(0.708f, 0.063f, 0.3f);
        var color = isActive ? activeColor : lockedColor;

        button.interactable = isActive;
        button.GetComponent<Image>().color = color;
    }

    private static void UpdateLoanUis() {
        var loanItems = Object.FindObjectsOfType<LoanItem>();

        foreach (var loanItem in loanItems) {
            var method = ReflectionUtils.GetNonPublicMethod(loanItem, "UpdateAvailableLayoutUI");

            method.Invoke(loanItem, Array.Empty<object>());
        }
    }
}
