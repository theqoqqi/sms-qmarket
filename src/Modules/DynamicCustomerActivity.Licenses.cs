using UnityEngine;

namespace QMarketPlugin.Modules;

internal static partial class DynamicCustomerActivity {
    private const float ActivityBonusPerLicense = 0.02f;

    private static float GetLicenseFactor() {
        var unlockedLicenses = ProductLicenseManager.Instance.UnlockedLicenseCount;

        return Mathf.Pow(1 + ActivityBonusPerLicense, unlockedLicenses);
    }
}
