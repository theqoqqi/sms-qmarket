using System.Collections.Generic;
using QMarketPlugin.Utils;

namespace QMarketPlugin.Modules;

public static partial class CustomerStrategies {

    private static readonly ScriptableObjectListManager<CustomerStrategySO, string> StrategyManager =
            new ScriptableObjectListManager<CustomerStrategySO, string>(
                    so => so.name,
                    (so, name) => so.name = name
            );

    static CustomerStrategies() {
        AddFiller("1_OnboardingStrategy", 1, 3, 100, 0, 0);
        AddFiller("2_BeginnerStrategy", 2, 4, 75, 25, 0);
        AddFiller("3_EasyStrategy", 2, 5, 50, 50, 0);
        AddFiller("4_MediumStrategy", 2, 6, 25, 75, 0);
        AddFiller("5_MediumStrategy", 3, 6, 0, 100, 0);
        AddFiller("6_MediumStrategy", 3, 8, 0, 80, 20);
        AddFiller("7_HardStrategy", 3, 9, 0, 60, 40);
        AddFiller("8_HardStrategy", 3, 10, 0, 40, 60);
        AddFiller("9_ExtremeStrategy", 4, 9, 0, 20, 80);
        AddFiller("10_ExtremeStrategy", 4, 10, 0, 0, 100);
    }
    
    private static void AddFiller(
            string name,
            int maxProductsToBuy,
            int maxProductVariantsToBuy,
            float displayedRate,
            float unlockedRate,
            float expectedRate
    ) {
        StrategyManager.AddInfo(name, strategy => {
            strategy.MaxProductCountToBuy = maxProductsToBuy;
            strategy.MaxProductVariantsCountToBuy = maxProductVariantsToBuy;
            strategy.DisplayedProductsRate = displayedRate;
            strategy.UnlockedProductsRate = unlockedRate;
            strategy.ExpectedProductsRate = expectedRate;
        });
    }

    private static void Setup(List<CustomerStrategySO> customerStrategies) {
        StrategyManager.Setup(customerStrategies);
    }
}
