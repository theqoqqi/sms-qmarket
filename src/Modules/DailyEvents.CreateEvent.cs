using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MyBox;
using QMarketPlugin.Utils;
using UnityEngine;
using Random = System.Random;

namespace QMarketPlugin.Modules;

public static partial class DailyEvents {

    private const int AverageExpensesPerLevel = 100;

    private static readonly RangedInt SmallExpenses = new RangedInt(100, 1000);

    private static readonly RangedInt MediumExpenses = new RangedInt(500, 2500);

    private static readonly RangedInt LargeExpenses = new RangedInt(1500, 5000);

    private static readonly RangedInt VeryLargeExpenses = new RangedInt(3000, 10000);

    private static readonly RangedInt HugeExpenses = new RangedInt(5000, 100000);

    private static readonly IList<Reason> Reasons = new List<Reason> {
            new Reason("Неожиданный ремонт в квартире. Не хватает на материалы.", SmallExpenses),
            new Reason("Заболел питомец. Требуется срочное лечение.", SmallExpenses),
            new Reason("Нужно оплатить штраф за нарушение ПДД.", SmallExpenses),
            new Reason("Непредвиденные расходы на дорогие лекарства.", SmallExpenses),
            new Reason("Покупка подарка на юбилей близкому человеку.", SmallExpenses),

            new Reason("Срочная медицинская помощь для родственника.", MediumExpenses),
            new Reason("Ремонт кухонной техники. Нужна новая плита.", MediumExpenses),
            new Reason("Ремонт автомобиля после ДТП.", MediumExpenses),
            new Reason("Близкий человек попал в беду. Необходимо оказать финансовую поддержку.", MediumExpenses),
            new Reason("Внезапное обострение хронического заболевания. Нужно оплатить лечение.", MediumExpenses),

            new Reason("Необходим крупный ремонт в квартире. Не хватает на материалы и услуги.", LargeExpenses),
            new Reason("Близкому человеку нужно срочное дорогостоящее лечение. Требуется оплата.", LargeExpenses),
            new Reason("Нужно оплатить штраф за серьезное нарушение ПДД и ремонт авто.", LargeExpenses),
            new Reason("Пожар в квартире. Требуется помощь семьям и ремонт помещения.", LargeExpenses),
            new Reason("Покупка крупного бытового оборудования. Необходима новая стиральная машина.", LargeExpenses),

            new Reason("Срочное лечение серьезного заболевания члена семьи. Требуются операция.", VeryLargeExpenses),
            new Reason("Налоговая проверка. Требуется оплата штрафов и услуг адвоката.", VeryLargeExpenses),
            new Reason("Расходы на организацию крупного мероприятия.", VeryLargeExpenses),
            new Reason("Вы решили отправиться путешествие с семьей в экзотическую страну.", VeryLargeExpenses),
            new Reason("Шантаж со стороны преступной группировки. Требуется оплатить молчание.", VeryLargeExpenses),

            new Reason("Вас подставили или обвинили в крупном преступлении. Нужен адвокат.", HugeExpenses),
            new Reason("Вашего сына взяли в заложники. Похитители требуют круглую сумму.", HugeExpenses),
            new Reason("Ваши личные данные попали в руки хакеров, которые требуют выкуп.", HugeExpenses),
            new Reason("Конкуренты запускают негативные кампании против вас. Требуется суд.", HugeExpenses),
            new Reason("Местная преступная группировка угрожает вашему бизнесу. Нужно откупиться.", HugeExpenses),
    };

    private const string DefaultReason = "Срочно нужны деньги";

    private static readonly IList<Reason> UsedReasons = new List<Reason>();

    private static DailyEvent CreateEvent(Random random, int level) {
        var sum = GetRandomSum(random, level);
        var title = GetRandomTitle(random, sum);
        var deadlineHour = PossibleDeadlineHours.Random(random);

        return new DailyEvent(title, sum, deadlineHour);
    }

    private static int GetRandomSum(Random random, int level) {
        var randomFactor = 1 + (float) (random.NextDouble() - random.NextDouble()) * 0.2f;
        var sum = level * AverageExpensesPerLevel * randomFactor;
        var roundBy = Mathf.RoundToInt(Mathf.Pow(10, CountDigits((int) sum) - 2));
        var roundedSum = Mathf.RoundToInt(sum / roundBy) * roundBy;

        return level == 0 ? 0 : Math.Max(roundedSum, 100);
    }

    private static int CountDigits(int number) {
        return number.ToString(CultureInfo.InvariantCulture).Length;
    }

    private static string GetRandomTitle(Random random, int sum) {
        var suitableReasons = Reasons
                .Where(reason => reason.IsSuitableForSum(sum))
                .Where(reason => !UsedReasons.Contains(reason))
                .ToList();

        if (suitableReasons.Count == 0) {
            random.Next(1); // Skip one call to get same state
            return DefaultReason;
        }

        var randomIndex = random.Next(suitableReasons.Count);
        var reason = suitableReasons[randomIndex];
        
        UsedReasons.Add(reason);

        return reason.Title;
    }

    private static void ResetUsedReasons() {
        UsedReasons.Clear();
    }

    private class Reason {

        public readonly string Title;

        private readonly RangedInt possibleSum;

        public Reason(string title, RangedInt possibleSum) {
            Title = title;
            this.possibleSum = possibleSum;
        }

        public bool IsSuitableForSum(int sum) {
            return sum >= possibleSum.Min && sum <= possibleSum.Max;
        }
    }
}
