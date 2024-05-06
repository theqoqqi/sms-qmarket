using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QMarketPlugin.Modules;

public static partial class Loans {

    private static readonly IDictionary<int, LoanInfo> LoanInfos = new Dictionary<int, LoanInfo>();

    static Loans() {
        AddLoan(LoanSource.Private, 100, 0, 0, "Занять у родственников");
        AddLoan(LoanSource.Private, 500, 0, 10, "Занять у друга");
        AddLoan(LoanSource.Private, 1500, 0, 25, "Занять у близкого друга");
        AddLoan(LoanSource.Private, 2500, 0, 40, "Занять у знакомого бизнесмена");

        AddLoan(LoanSource.Criminals, 300, 0.1f, 0, "Занять у местной группировки");
        AddLoan(LoanSource.Criminals, 2000, 0.15f, 15, "Средний займ у группировки");
        AddLoan(LoanSource.Criminals, 4000, 0.20f, 30, "Большой займ у группировки");
        AddLoan(LoanSource.Criminals, 8000, 0.25f, 45, "Крупный займ у группировки");

        AddLoan(LoanSource.Bank, 1000, 0.01f, 5, "Малый кредит");
        AddLoan(LoanSource.Bank, 2500, 0.02f, 20, "Средний кредит");
        AddLoan(LoanSource.Bank, 5000, 0.03f, 35, "Большой кредит");
        AddLoan(LoanSource.Bank, 10000, 0.04f, 50, "Профессиональный кредит");
    }

    private static void AddLoan(LoanSource source, int sum, float dailyPercent, int requiredLevel, string title) {
        var loanId = LoanInfos.Count + 1;
        var loanInfo = new LoanInfo(source, title, sum, dailyPercent, requiredLevel);

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

    private static bool LoanExists(List<BankCreditSO> loans, int id) {
        return loans.Find(loan => loan.ID == id);
    }

    private static bool CanAddExtraLoan(int loanId) {
        return LoanInfos.ContainsKey(loanId);
    }

    private static void AddExtraLoan(ICollection<BankCreditSO> loans, int loanId) {
        loans.Add(LoanInfos[loanId].Instance);
    }

    private static bool CanTakeLoan(BankCreditSO loan) {
        return !BankManager.Instance.Loans
                .Any(data => data.Taken && IsSameSource(loan, data));
    }

    private static bool IsSameSource(BankCreditSO loan, LoanData data) {
        return LoanInfos[data.LoanID].Source == LoanInfos[loan.ID].Source;
    }

    private static void AddLoanListeners() {
        BankManager.Instance.onTakenLoan += _ => UpdateLoanUis();
        BankManager.Instance.onCompletedLoan += _ => UpdateLoanUis();
    }

}
