using System.Collections.Generic;

namespace QMarketPlugin.Modules;

internal static partial class DynamicCustomerActivity {
    public class Weekday {
        
        public static readonly Dictionary<int, Weekday> Weekdays = new Dictionary<int, Weekday> {
                {0, new Weekday("Понедельник", 0.8f, 0.05f, false)},
                {1, new Weekday("Вторник", 0.95f, 0.05f, false)},
                {2, new Weekday("Среда", 0.85f, 0.05f, false)},
                {3, new Weekday("Четверг", 0.9f, 0.05f, false)},
                {4, new Weekday("Пятница", 1f, 0.15f, false)},
                {5, new Weekday("Суббота", 1.3f, 0.35f, true)},
                {6, new Weekday("Воскресенье", 1.2f, 0.25f, true)},
        };

        public readonly string Title;

        public readonly float ActivityRate;

        public readonly float LargePurchaseChance;

        public readonly bool IsWeekend;

        public Weekday(string title, float activityRate, float largePurchaseChance, bool isWeekend) {
            Title = title;
            ActivityRate = activityRate;
            LargePurchaseChance = largePurchaseChance;
            IsWeekend = isWeekend;
        }

        public static Weekday Of(int day) {
            return Weekdays[day % Weekdays.Count];
        }
    }
}
