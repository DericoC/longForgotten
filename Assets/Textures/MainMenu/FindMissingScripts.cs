using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Help/Find Missing Scripts")]

    static void findMissing() {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(prefabPaths => prefabPaths.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        foreach(string path in prefabPaths) {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            foreach(Component com in prefab.GetComponentsInChildren<Component>()) {
                if (com == null) {
                    Debug.Log("FOUND: " + path, prefab);
                    break;
                }
            }
        }
    }
}
