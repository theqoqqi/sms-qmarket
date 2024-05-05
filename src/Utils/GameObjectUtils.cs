using System.Linq;
using TMPro;
using UnityEngine;

namespace QMarketPlugin.Utils;

public static class GameObjectUtils {

    public static GameObject CreateText(Transform parent, string name, float fontSize) {

        var textGameObject = new GameObject(name);

        var textTransform = textGameObject.AddComponent<RectTransform>();
        textTransform.SetParent(parent);
        textTransform.anchorMin = Vector2.zero;
        textTransform.anchorMax = Vector2.zero;
        textTransform.offsetMin = Vector2.zero;
        textTransform.offsetMax = Vector2.zero;

        var textComponent = textGameObject.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;

        var tmpFontAsset = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
                .FirstOrDefault(asset => asset.name == "UptownBoy SDF");

        if (tmpFontAsset != null) {
            textComponent.font = tmpFontAsset;
        }

        return textGameObject;
    }

    public static void PrintNamesUpByHierarchy(GameObject gameObject) {
        Debug.Log(gameObject.name);

        if (gameObject.transform.parent != null) {
            PrintNamesUpByHierarchy(gameObject.transform.parent.gameObject);
        }
    }

    public static void PrintNamesDownByHierarchy(GameObject gameObject, int indentLevel = 0) {
        var indentation = "".PadRight(indentLevel * 2, ' ');
        Debug.Log(indentation + gameObject.name);

        foreach (Transform child in gameObject.transform) {
            PrintNamesDownByHierarchy(child.gameObject, indentLevel + 1);
        }
    }

    public static void PrintComponentNames(GameObject gameObject) {
        var components = gameObject.GetComponents<Component>();

        foreach (var component in components) {
            Debug.Log(component.GetType().Name);
        }
    }
}
