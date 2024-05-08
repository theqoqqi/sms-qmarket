using System.Collections.Generic;
using System.Linq;
using QMarketPlugin.Utils;

namespace QMarketPlugin.Modules;

public static partial class Loans {

    private static readonly ScriptableObjectListManager<BankCreditSO, int> LoanManager =
            new ScriptableObjectListManager<BankCreditSO, int>(
                    bankCredit => bankCredit.ID,
                    (bankCredit, id) => bankCredit.ID = id
            );

    static Loans() {
        AddLoanInfo(LoanSource.Private, 100, 0, 0, "Занять у родственников");
        AddLoanInfo(LoanSource.Private, 500, 0, 10, "Занять у друга");
        AddLoanInfo(LoanSource.Private, 1500, 0, 25, "Занять у близкого друга");
        AddLoanInfo(LoanSource.Private, 2500, 0, 40, "Занять у знакомого бизнесмена");

        AddLoanInfo(LoanSource.Criminals, 300, 0.1f, 0, "Занять у местной группировки");
        AddLoanInfo(LoanSource.Criminals, 2000, 0.15f, 15, "Средний займ у группировки");
        AddLoanInfo(LoanSource.Criminals, 4000, 0.20f, 30, "Большой займ у группировки");
        AddLoanInfo(LoanSource.Criminals, 8000, 0.25f, 45, "Крупный займ у группировки");

        AddLoanInfo(LoanSource.Bank, 1000, 0.01f, 5, "Малый кредит");
        AddLoanInfo(LoanSource.Bank, 2500, 0.02f, 20, "Средний кредит");
        AddLoanInfo(LoanSource.Bank, 5000, 0.03f, 35, "Большой кредит");
        AddLoanInfo(LoanSource.Bank, 10000, 0.04f, 50, "Профессиональный кредит");
    }

    private static void AddLoanInfo(LoanSource source, int sum, float dailyPercent, int requiredLevel, string title) {
        var loanId = LoanManager.Count + 1;
        var loanInfo = new LoanInfo(source, title, sum, dailyPercent, requiredLevel);

        LoanManager.AddInfo(loanId, loanInfo);
    }

    private static void Setup() {
        LoanManager.Setup(IDManager.Instance.Loans);
    }

    private static void AddMissingLoans(List<BankCreditSO> loans) {
        LoanManager.PatchList(loans);
    }

    private static void AddMissingLoanDatas(ICollection<LoanData> loanDatas, IReadOnlyList<BankCreditSO> loans) {
        while (loanDatas.Count < loans.Count) {
            var loan = loans[loanDatas.Count];

            loanDatas.Add(new LoanData {
                    LoanID = loan.ID
            });
        }
    }

    private static bool CanTakeLoan(BankCreditSO loan) {
        return !BankManager.Instance.Loans
                .Any(data => data.Taken && IsSameSource(loan, data));
    }

    private static bool IsSameSource(BankCreditSO loan, LoanData data) {
        var loanInfo = LoanManager.GetInfo<LoanInfo>(loan.ID);
        var dataInfo = LoanManager.GetInfo<LoanInfo>(data.LoanID);

        return loanInfo.Source == dataInfo.Source;
    }

    private static void AddLoanListeners() {
        BankManager.Instance.onTakenLoan += _ => UpdateLoanUis();
        BankManager.Instance.onCompletedLoan += _ => UpdateLoanUis();
    }
}
