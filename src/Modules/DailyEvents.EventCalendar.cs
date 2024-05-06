using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using QMarketPlugin.Utils;

namespace QMarketPlugin.Modules;

public static partial class DailyEvents {
    private class EventCalendar {

        private readonly Random random;

        private readonly RangedInt firstInterval;

        private readonly RangedInt interval;

        private readonly IDictionary<int, DailyEvent> scheduledEvents = new Dictionary<int, DailyEvent>();

        private int LastScheduledDay => scheduledEvents.Count == 0 ? 0 : scheduledEvents.Keys.Max();

        private readonly Func<Random, int, DailyEvent> eventFactory;

        public EventCalendar(
                int seed,
                RangedInt firstInterval,
                RangedInt interval,
                Func<Random, int, DailyEvent> eventFactory
        ) {
            Plugin.GetLogger().LogInfo("EventCalendar seed: " + seed);
            random = new Random(seed);
            this.firstInterval = firstInterval;
            this.interval = interval;
            this.eventFactory = eventFactory;
        }

        public bool HasEvent(int day) {
            return GetEvent(day) != null;
        }

        public DailyEvent GetEvent(int day) {
            ScheduleUntil(day);

            return SafeGetEvent(day);
        }

        private DailyEvent SafeGetEvent(int day) {
            return scheduledEvents.TryGetValue(day, out var dailyEvent)
                    ? dailyEvent : null;
        }

        private void ScheduleUntil(int day) {
            while (day > LastScheduledDay) {
                ScheduleNextEvent(day);
            }
        }

        private void ScheduleNextEvent(int requestedDay) {
            var nextDay = LastScheduledDay + GetNextInterval();
            var storeLevel = SaveManager.Instance.Progression.CurrentStoreLevel;
            // Костыль: если запрашивается один из прошедших дней, нужно передавать 0 уровень,
            //          чтобы не расходовались заголовки, подходящие под текущий уровень.
            var levelToPass = nextDay >= requestedDay ? storeLevel : 0;

            scheduledEvents[nextDay] = eventFactory(random, levelToPass);
        }

        private int GetNextInterval() {
            return scheduledEvents.Count == 0
                    ? firstInterval.Random(random)
                    : interval.Random(random);
        }

        public void LogEvents() {
            Plugin.GetLogger().LogInfo("Events scheduled at days: " + string.Join(", ", scheduledEvents.Keys));
        }
    }
}
