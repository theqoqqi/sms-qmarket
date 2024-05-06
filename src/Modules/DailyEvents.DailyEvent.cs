namespace QMarketPlugin.Modules;

public static partial class DailyEvents {
    private class DailyEvent {

        public readonly string Title;

        public readonly float Sum;

        public readonly int DeadlineHour;

        public DailyEvent(string title, float sum, int deadlineHour) {
            Title = title;
            Sum = sum;
            DeadlineHour = deadlineHour;
        }
    }
}
