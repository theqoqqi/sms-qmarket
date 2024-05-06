using System;

namespace QMarketPlugin.Modules;

public static partial class Loans {
    private class LoanInfo {

        public BankCreditSO Instance { get; private set; }

        public readonly LoanSource Source;

        public readonly string Title;

        private readonly Action<BankCreditSO> Filler;

        public LoanInfo(LoanSource source, string title, int sum, float dailyPercent, int requiredLevel) {
            Source = source;
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
}
