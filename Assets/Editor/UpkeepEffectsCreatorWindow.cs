using System;
using System.Collections.Generic;
using System.IO;
using Effects;
using UnityEditor;
using UnityEngine;

/// <summary>Creates day-end upkeep effect assets in Resources/Upkeeps.</summary>
public sealed class UpkeepEffectsCreatorWindow : EditorWindow
{
    private const string UpkeepsFolder = "Assets/Resources/Upkeeps";

    [Serializable]
    private sealed class EffectDraft
    {
        public StatType statType;
        public int amount;
    }

    [SerializeField] private string upkeepTitle = "";
    [SerializeField] private string summaryText = "";
    [SerializeField, Range(0f, 1f)] private float minimumAcceptanceRate;
    [SerializeField] private List<EffectDraft> effects = new();
    [SerializeField] private GameManager targetGameManager;
    [SerializeField] private Vector2 scrollPosition;

    private GUIStyle titleStyle;
    private GUIStyle cardStyle;
    private GUIStyle sectionStyle;

    [MenuItem("Tools/Upkeep Effects Creator")]
    private static void ShowWindow()
    {
        UpkeepEffectsCreatorWindow window = GetWindow<UpkeepEffectsCreatorWindow>("Upkeep Effects Creator");
        window.minSize = new Vector2(500f, 530f);
    }

    private void OnGUI()
    {
        InitialiseStyles();

        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            EditorGUILayout.LabelField("UPKEEP EFFECTS CREATOR", titleStyle);
            EditorGUILayout.LabelField("Create the result applied at the end of a game day.", EditorStyles.miniLabel);
        }

        EditorGUILayout.Space(8f);
        using (EditorGUILayout.ScrollViewScope scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scroll.scrollPosition;
            DrawIdentitySection();
            DrawSummarySection();
            DrawEffectsSection();
            DrawAssignmentSection();
            DrawCreateButton();
        }
    }

    private void DrawIdentitySection()
    {
        BeginSection("01  IDENTITY");
        upkeepTitle = EditorGUILayout.TextField(new GUIContent("Title", "Shown below the completed-day title and used as the asset file name."), upkeepTitle);
        minimumAcceptanceRate = EditorGUILayout.Slider(new GUIContent("Minimum Acceptance", "This upkeep is chosen when the daily accept rate is at least this value."), minimumAcceptanceRate, 0f, 1f);
        EditorGUILayout.LabelField($"Threshold: {minimumAcceptanceRate:P0} or higher", EditorStyles.miniLabel);
        EditorGUILayout.HelpBox($"Saved to {UpkeepsFolder}. Lower thresholds act as fallbacks, so create at least one with a 0% threshold.", MessageType.None);
        EndSection();
    }

    private void DrawSummarySection()
    {
        BeginSection("02  DAY-END MESSAGE");
        EditorGUILayout.LabelField(new GUIContent("Summary Text", "Shown in the day-end popup before the next day begins."), EditorStyles.boldLabel);
        summaryText = EditorGUILayout.TextArea(summaryText, GUILayout.MinHeight(70f));
        EndSection();
    }

    private void DrawEffectsSection()
    {
        BeginSection("03  STAT EFFECTS");
        EditorGUILayout.LabelField("Effects are applied together when this upkeep is selected.", EditorStyles.miniLabel);

        for (int i = 0; i < effects.Count; i++)
        {
            EffectDraft effect = effects[i];
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                effect.statType = (StatType)EditorGUILayout.EnumPopup(effect.statType, GUILayout.Width(135f));
                effect.amount = EditorGUILayout.IntField("Amount", effect.amount);
                if (GUILayout.Button("−", GUILayout.Width(25f)))
                {
                    effects.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
            }
        }

        if (GUILayout.Button("+ Add Effect", EditorStyles.miniButton))
            effects.Add(new EffectDraft());
        EndSection();
    }

    private void DrawAssignmentSection()
    {
        BeginSection("04  OPTIONAL ASSIGNMENT");
        targetGameManager = (GameManager)EditorGUILayout.ObjectField(
            new GUIContent("Game Manager", "Optional. Add the new asset to this GameManager's upkeep list immediately."),
            targetGameManager,
            typeof(GameManager),
            true);
        EditorGUILayout.LabelField("Leave empty to create only the asset and assign it later in the GameManager inspector.", EditorStyles.miniLabel);
        EndSection();
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.Space(14f);
        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(upkeepTitle)))
        {
            Color previousColour = GUI.backgroundColor;
            GUI.backgroundColor = new Color(.35f, .78f, .55f);
            if (GUILayout.Button("CREATE UPKEEP EFFECT", GUILayout.Height(38f)))
                CreateUpkeepEffect();
            GUI.backgroundColor = previousColour;
        }
    }

    private void CreateUpkeepEffect()
    {
        string cleanTitle = upkeepTitle.Trim();
        string fileName = SanitizeFileName(cleanTitle);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            EditorUtility.DisplayDialog("Invalid Title", "Enter a valid upkeep title.", "OK");
            return;
        }

        UpkeepEffects upkeep = CreateInstance<UpkeepEffects>();
        upkeep.title = cleanTitle;
        upkeep.summaryText = summaryText;
        upkeep.minAvg = minimumAcceptanceRate;
        upkeep.effects = ToEffects(effects);

        EnsureFolderExists(UpkeepsFolder);
        string requestedPath = Path.Combine(UpkeepsFolder, fileName + ".asset").Replace("\\", "/");
        AssetDatabase.CreateAsset(upkeep, AssetDatabase.GenerateUniqueAssetPath(requestedPath));

        if (targetGameManager != null)
        {
            Undo.RecordObject(targetGameManager, "Add Upkeep Effect");
            List<UpkeepEffects> assignedUpkeeps = new List<UpkeepEffects>(targetGameManager.upkeepEffects ?? Array.Empty<UpkeepEffects>());
            assignedUpkeeps.Add(upkeep);
            targetGameManager.upkeepEffects = assignedUpkeeps.ToArray();
            EditorUtility.SetDirty(targetGameManager);
        }

        AssetDatabase.SaveAssets();
        Selection.activeObject = upkeep;
        EditorGUIUtility.PingObject(upkeep);
        ResetForm();
    }

    private static StatEffect[] ToEffects(List<EffectDraft> drafts)
    {
        StatEffect[] result = new StatEffect[drafts.Count];
        for (int i = 0; i < drafts.Count; i++)
            result[i] = new StatEffect { type = drafts[i].statType, amount = drafts[i].amount, revealDelay = 0 };
        return result;
    }

    private void ResetForm()
    {
        upkeepTitle = summaryText = "";
        minimumAcceptanceRate = 0f;
        effects.Clear();
    }

    private void BeginSection(string title)
    {
        EditorGUILayout.Space(7f);
        EditorGUILayout.LabelField(title, sectionStyle);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    }

    private static void EndSection() => EditorGUILayout.EndVertical();

    private void InitialiseStyles()
    {
        titleStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 19, normal = { textColor = new Color(.45f, .9f, .67f) } };
        cardStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(14, 14, 12, 12) };
        sectionStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, normal = { textColor = new Color(.55f, .78f, 1f) } };
    }

    private static string SanitizeFileName(string value)
    {
        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            value = value.Replace(invalidCharacter.ToString(), string.Empty);
        return value;
    }

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string parentFolder = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(parentFolder)) EnsureFolderExists(parentFolder);
        AssetDatabase.CreateFolder(parentFolder, Path.GetFileName(folderPath));
    }
}
