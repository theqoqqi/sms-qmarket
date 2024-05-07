using System.Collections.Generic;
using HarmonyLib;
using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

namespace QMarketPlugin.Patches;

public static class CustomerManagerPatch {

    private static IList<int> DisplayedProducts => InventoryManager.Instance.DisplayedProducts;

    private static IList<int> UnlockedProducts => ProductLicenseManager.Instance.UnlockedProducts;

    private static IList<int> ExpectedProducts => ProductLicenseManager.Instance.ProductsExpectedByLevel;
    
    private static CustomerStrategySO CurrentStrategy => CustomerStrategiesManager.Instance.CurrentStrategy;

    [HarmonyPatch(typeof(CustomerManager), "CreateShoppingList")]
    [HarmonyPrefix]
    private static void CreateShoppingList_Fix(ref ItemQuantity __result, ref bool __runOriginal) {
        __result = CreateShoppingList();
        __runOriginal = false;
    }
    
    private static ItemQuantity CreateShoppingList() {
        var shoppingList = new ItemQuantity();
        var productsToBuy = GetProductsToBuy();

        foreach (var productId in productsToBuy) {
            shoppingList.Products[productId] = Random.Range(1, CurrentStrategy.MaxProductCountToBuy + 1);
        }

        return new ItemQuantity(shoppingList);
    }

    private static HashSet<int> GetProductsToBuy() {
        var productsToBuy = new HashSet<int>();
        var productsToBuyByCategories = GetRandomProductsToBuyByCategories();

        foreach (var pair in productsToBuyByCategories) {
            AddRandomProducts(productsToBuy, pair.Key, pair.Value);
        }

        return productsToBuy;
    }

    private static Dictionary<IList<int>, int> GetRandomProductsToBuyByCategories() {
        var totalProductCountToBuy = Random.Range(1, CurrentStrategy.MaxProductVariantsCountToBuy + 1);
        var allCategories = new Dictionary<IList<int>, float> {
                {DisplayedProducts, CurrentStrategy.DisplayedProductsRate},
                {UnlockedProducts, CurrentStrategy.UnlockedProductsRate},
                {ExpectedProducts, CurrentStrategy.ExpectedProductsRate},
        };
        
        var countsByCategories = new Dictionary<IList<int>, int> {
                {DisplayedProducts, 0},
                {UnlockedProducts, 0},
                {ExpectedProducts, 0},
        };

        for (var i = 0; i < totalProductCountToBuy; i++) {
            var products = allCategories.GetWeightedRandom(pair => pair.Value).Key;

            countsByCategories[products]++;
        }
        
        return countsByCategories;
    }

    private static void AddRandomProducts(ISet<int> toProducts, ICollection<int> fromProducts, int amount) {
        toProducts.UnionWith(GetRandomProducts(fromProducts, amount));
    }
    
    private static IEnumerable<int> GetRandomProducts(ICollection<int> fromProducts, int amount) {
        if (amount <= 0) {
            return new HashSet<int>();
        }

        var clampedAmount = Mathf.Clamp(amount, 1, fromProducts.Count);

        var unusedProducts = new List<int>(fromProducts);
        var collection = new List<int>();

        for (var i = 0; i < clampedAmount; ++i) {
            var randomIndex = Random.Range(0, unusedProducts.Count);

            collection.Add(unusedProducts[randomIndex]);
            unusedProducts.RemoveAt(randomIndex);
        }

        return new HashSet<int>(collection);
    }
}
