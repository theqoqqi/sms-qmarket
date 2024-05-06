using System;
using System.Collections;
using System.IO;
using MyBox;
using UnityEngine;

namespace QMarketPlugin.Modules;

public static partial class DailyEvents {

    private static EventCalendar eventCalendar;

    private static readonly RangedInt FirstEventInterval = new RangedInt(7, 10);

    private static readonly RangedInt EventInterval = new RangedInt(5, 10);

    private static readonly RangedInt PossibleDeadlineHours = new RangedInt(12, 21);
    
    private const float DelayFromStartOfDay = 5f;

    private static DayCycleManager Manager => DayCycleManager.Instance;

    private static int CurrentDay => Manager.CurrentDay;

    private static int CurrentHour => Manager.CurrentHour + (Manager.AM || Manager.CurrentHour == 12 ? 0 : 12);

    private static bool HasEventToday => !isCurrentEventDone && eventCalendar.HasEvent(CurrentDay);

    private static DailyEvent CurrentEvent => eventCalendar.GetEvent(CurrentDay);

    private static bool IsDeadlineReached => HasEventToday && CurrentHour >= CurrentEvent.DeadlineHour;

    private static bool isCurrentEventDone;

    private static void Setup(string saveFilePath) {
        Plugin.Instance.StartCoroutine(SetupCoroutine(saveFilePath));
    }

    private static IEnumerator SetupCoroutine(string saveFilePath) {
        yield return new WaitForSeconds(0.5f);
        
        var saveName = Path.GetFileNameWithoutExtension(saveFilePath);

        eventCalendar = CreateEventCalendar(saveName);

        ResetUsedReasons();
        SetupGui();
        SetupCurrentDay();
    }

    private static EventCalendar CreateEventCalendar(string seed) {
        return new EventCalendar(seed.GetHashCode(), FirstEventInterval, EventInterval, CreateEvent);
    }

    private static void OnStartNextDay() {
        Plugin.Instance.StartCoroutine(SetupCurrentDayCoroutine());
    }

    private static IEnumerator SetupCurrentDayCoroutine() {
        yield return new WaitForSeconds(DelayFromStartOfDay);

        SetupCurrentDay();
    }

    private static void SetupCurrentDay() {
        isCurrentEventDone = IsDeadlineReached;

        if (HasEventToday) {
            SFXManager.Instance.PlayCheckoutWarningSFX();
            ShowGui(CurrentEvent);
        }
        else {
            HideGui();
        }
        
        eventCalendar.LogEvents();
    }

    private static void Update() {
        if (eventCalendar == null) {
            return;
        }
        
        if (IsDeadlineReached) {
            FinishEvent();
        }
    }

    private static void FinishEvent() {
        TakeMoney();
        HideGui();
        SFXManager.Instance.PlayMoneyPaperSFX();
        isCurrentEventDone = true;
    }

    private static void TakeMoney() {
        MoneyManager.Instance.MoneyTransition(
                -(float) Math.Round(CurrentEvent.Sum, 2),
                MoneyManager.TransitionType.NONE
        );

        if (MoneyManager.Instance.Money < 0) {
            DayCycleManager.Instance.FinishTheDay();
        }
    }
}
