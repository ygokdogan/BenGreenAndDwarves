using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class ChangeProjectFontsTool : EditorWindow
{
    public TMP_FontAsset newTMPFont;
    public Font newLegacyFont;

    [MenuItem("Tools/Global Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<ChangeProjectFontsTool>("Global Font Changer");
    }

    void OnGUI()
    {
        GUILayout.Label("Change Fonts Everywhere", EditorStyles.boldLabel);
        
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newTMPFont, typeof(TMP_FontAsset), false);
        newLegacyFont = (Font)EditorGUILayout.ObjectField("New Legacy Font", newLegacyFont, typeof(Font), false);

        GUILayout.Space(15);

        if (GUILayout.Button("1. Change Fonts in ACTIVE SCENE"))
        {
            ChangeFontsInActiveScene();
        }

        GUILayout.Space(5);

        GUI.backgroundColor = Color.red; // Added color to warn that this is a heavy operation
        if (GUILayout.Button("2. Change Fonts in ALL PREFABS (Project-Wide)"))
        {
            if (EditorUtility.DisplayDialog("Warning", "This will modify all prefabs in your project. Ensure you have a backup. Do you want to proceed?", "Yes, do it", "Cancel"))
            {
                ChangeFontsInAllPrefabs();
            }
        }
        GUI.backgroundColor = Color.white;
    }

    void ChangeFontsInActiveScene()
    {
        int changedCount = 0;

        if (newTMPFont != null)
        {
            TextMeshProUGUI[] tmpTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            foreach (var txt in tmpTexts)
            {
                if (txt.gameObject.scene.isLoaded)
                {
                    txt.font = newTMPFont;
                    EditorUtility.SetDirty(txt);
                    changedCount++;
                }
            }
        }

        if (newLegacyFont != null)
        {
            Text[] legacyTexts = Resources.FindObjectsOfTypeAll<Text>();
            foreach (var txt in legacyTexts)
            {
                if (txt.gameObject.scene.isLoaded)
                {
                    txt.font = newLegacyFont;
                    EditorUtility.SetDirty(txt);
                    changedCount++;
                }
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"Scene update complete! Changed {changedCount} text components.");
    }

    void ChangeFontsInAllPrefabs()
    {
        // Find all prefab files in the project
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int updatedPrefabCount = 0;
        int totalTextComponentsChanged = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            bool isModified = false;

            if (prefab != null)
            {
                // Find all TMP components in this prefab (including disabled ones)
                if (newTMPFont != null)
                {
                    TextMeshProUGUI[] tmpTexts = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (var txt in tmpTexts)
                    {
                        if (txt.font != newTMPFont)
                        {
                            txt.font = newTMPFont;
                            isModified = true;
                            totalTextComponentsChanged++;
                        }
                    }
                }

                // Find all Legacy Text components in this prefab (including disabled ones)
                if (newLegacyFont != null)
                {
                    Text[] legacyTexts = prefab.GetComponentsInChildren<Text>(true);
                    foreach (var txt in legacyTexts)
                    {
                        if (txt.font != newLegacyFont)
                        {
                            txt.font = newLegacyFont;
                            isModified = true;
                            totalTextComponentsChanged++;
                        }
                    }
                }

                // If changes were made, save the prefab asset
                if (isModified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    updatedPrefabCount++;
                }
            }
        }

        // Save and refresh the asset database so Unity registers the changes immediately
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Project-wide update complete! Modified {totalTextComponentsChanged} text components across {updatedPrefabCount} prefabs.");
    }
}