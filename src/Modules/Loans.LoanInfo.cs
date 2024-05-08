using System;
using QMarketPlugin.Utils;

namespace QMarketPlugin.Modules;

public static partial class Loans {
    private class LoanInfo : ScriptableObjectInfo<BankCreditSO> {

        public readonly LoanSource Source;

        public readonly string Title;

        public LoanInfo(LoanSource source, string title, int sum, float dailyPercent, int requiredLevel)
                : base(CreateFiller(sum, dailyPercent, requiredLevel)) {
            Source = source;
            Title = title;
        }

        private static Action<BankCreditSO> CreateFiller(int sum, float dailyPercent, int requiredLevel) {
            return loan => {
                loan.Amount = sum;
                loan.DailyInterestPercent = dailyPercent;
                loan.RequiredPlayerLevel = requiredLevel;
            };
        }
    }
}
