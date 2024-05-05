using BepInEx.Configuration;
using UnityEngine;

namespace QMarketPlugin.Modules; 

public class SkipOnboarding : MonoBehaviour {

    private KeyboardShortcut shortcut = new KeyboardShortcut(KeyCode.End, KeyCode.LeftAlt);

    private static OnboardingManager Manager => OnboardingManager.Instance;

    public void Update() {
        if (shortcut.IsDown()) {
            if (!Manager.Completed) {
                FinishOnboarding();
            }
            else {
                FinishMissions();
            }
        }
    }

    private void FinishOnboarding() {
        Manager.FinishStep(Manager.Step);
        Manager.Step = 17;
        Manager.FinishStep(Manager.Step);
        Manager.NextStep();
        Manager.SkipOnboarding = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void FinishMissions() {
        var missionSystem = FindObjectOfType<MissionSystem>();

        if (missionSystem) {
            missionSystem.gameObject.SetActive(false);
        }
    }
}
