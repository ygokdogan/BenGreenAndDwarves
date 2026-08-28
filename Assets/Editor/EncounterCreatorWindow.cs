using System;
using System.Collections.Generic;
using System.IO;
using Effects;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

/// <summary>Creates fully configured encounter assets in Resources/Encounters.</summary>
public sealed class EncounterCreatorWindow : EditorWindow
{
    private const string EncountersFolder = "Assets/Resources/Encounters";
    private const string TruthsFolder = EncountersFolder + "/Truths";
    private const string LiesFolder = EncountersFolder + "/Lies";

    private enum EncounterType { True, Lie }

    [Serializable]
    private sealed class EffectDraft
    {
        public StatType statType;
        public int amount;
        public int revealDelay;
    }

    [SerializeField] private string vendorName = "";
    [SerializeField] private EncounterType encounterType;
    [SerializeField] private string offerText = "";
    [SerializeField] private string acceptResultText = "";
    [SerializeField] private string rejectResultText = "";
    [SerializeField] private string rejectedDelayedText = "";
    [SerializeField] private string acceptedDelayedText = "";
    [SerializeField] private List<EffectDraft> acceptedEffects = new();
    [SerializeField] private List<EffectDraft> rejectedEffects = new();
    [SerializeField] private List<EffectDraft> claimedEffects = new();
    [SerializeField] private Vector2 scrollPosition;

    private GUIStyle titleStyle;
    private GUIStyle cardStyle;
    private GUIStyle sectionStyle;

    [MenuItem("Tools/Encounter Creator")]
    private static void ShowWindow()
    {
        var window = GetWindow<EncounterCreatorWindow>("Encounter Creator");
        window.minSize = new Vector2(510, 610);
    }

    private void OnGUI()
    {
        InitialiseStyles();

        using (new EditorGUILayout.VerticalScope(cardStyle))
        {
            EditorGUILayout.LabelField("ENCOUNTER CREATOR", titleStyle);
            EditorGUILayout.LabelField("Create a complete vendor encounter in one place.", EditorStyles.miniLabel);
        }

        EditorGUILayout.Space(8);
        using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scroll.scrollPosition;
            DrawIdentitySection();
            DrawCopySection();
            DrawEffectsSection();
            DrawCreateButton();
        }
    }

    private void DrawIdentitySection()
    {
        BeginSection("01  IDENTITY");
        vendorName = EditorGUILayout.TextField(new GUIContent("Vendor Name", "Also becomes the asset file name."), vendorName);
        encounterType = (EncounterType)EditorGUILayout.EnumPopup("Encounter Type", encounterType);
        var destination = encounterType == EncounterType.True ? TruthsFolder : LiesFolder;
        EditorGUILayout.HelpBox($"Saved automatically to {destination}. The asset name will be the vendor name.", MessageType.None);
        EndSection();
    }

    private void DrawCopySection()
    {
        BeginSection("02  DIALOGUE");
        DrawTextField("Offer Text", "What the vendor says or offers.", ref offerText, 54);
        DrawTextField("Accept Result", "Shown after accepting the offer.", ref acceptResultText, 42);
        DrawTextField("Reject Result", "Shown after rejecting the offer.", ref rejectResultText, 42);
        DrawTextField("Accepted Delayed Effect Text", "Shown when an accepted effect with a delay is applied.", ref acceptedDelayedText, 42);
        DrawTextField("Rejected Delayed Effect Text", "Shown when an declined effect with a delay is applied.", ref rejectedDelayedText, 42);
        EndSection();
    }

    private void DrawEffectsSection()
    {
        BeginSection("03  EFFECTS");
        if (encounterType == EncounterType.True)
            DrawEffectList("Accepted Effects", "Applied when the offer is accepted.", acceptedEffects);
        else
        {
            DrawEffectList("Claimed Effects", "Effects previewed to the player.", claimedEffects);
            DrawEffectList("Actual Accepted Effects", "Effects truly applied after accepting.", acceptedEffects);
        }

        DrawEffectList("Rejected Effects", "Applied when the offer is rejected.", rejectedEffects);
        EndSection();
    }

    private void DrawEffectList(string title, string description, List<EffectDraft> effects)
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(description, EditorStyles.miniLabel);

        for (var i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                effect.statType = (StatType)EditorGUILayout.EnumPopup(effect.statType, GUILayout.Width(112));
                effect.amount = EditorGUILayout.IntField("Amount", effect.amount);
                effect.revealDelay = Mathf.Max(0, EditorGUILayout.IntField("Delay", effect.revealDelay));
                if (GUILayout.Button("−", GUILayout.Width(25)))
                {
                    effects.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
            }
        }

        if (GUILayout.Button("+ Add Effect", EditorStyles.miniButton)) effects.Add(new EffectDraft());
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.Space(14);
        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(vendorName)))
        {
            var previousColour = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.35f, 0.78f, 0.55f);
            if (GUILayout.Button("CREATE ENCOUNTER", GUILayout.Height(38))) CreateEncounter();
            GUI.backgroundColor = previousColour;
        }
    }

    private void CreateEncounter()
    {
        var cleanName = vendorName.Trim();
        var filename = SanitizeFileName(cleanName);
        if (string.IsNullOrWhiteSpace(filename))
        {
            EditorUtility.DisplayDialog("Invalid Vendor Name", "Enter a valid vendor name.", "OK");
            return;
        }

        EncounterData encounter = encounterType == EncounterType.True
            ? CreateInstance<TrueEncounterData>() : CreateInstance<LieEncounterData>();

        encounter.vendorName = cleanName;
        encounter.offerText = offerText;
        encounter.acceptResultText = acceptResultText;
        encounter.rejectResultText = rejectResultText;
        encounter.acceptedDelayedText = acceptedDelayedText;
        encounter.rejectedDelayedText = rejectedDelayedText;
        encounter.rejectedEffects = ToEffects(rejectedEffects);

        if (encounter is TrueEncounterData trueEncounter)
            trueEncounter.effects = ToEffects(acceptedEffects);
        else if (encounter is LieEncounterData lieEncounter)
        {
            lieEncounter.claimedEffects = ToEffects(claimedEffects);
            lieEncounter.actualEffects = ToEffects(acceptedEffects);
        }

        var destinationFolder = encounterType == EncounterType.True ? TruthsFolder : LiesFolder;
        EnsureFolderExists(destinationFolder);
        var requestedPath = Path.Combine(destinationFolder, filename + ".asset").Replace("\\", "/");
        AssetDatabase.CreateAsset(encounter, AssetDatabase.GenerateUniqueAssetPath(requestedPath));
        AssetDatabase.SaveAssets();
        Selection.activeObject = encounter;
        EditorGUIUtility.PingObject(encounter);
        ResetForm();
    }

    private static StatEffect[] ToEffects(List<EffectDraft> drafts)
    {
        var effects = new StatEffect[drafts.Count];
        for (var i = 0; i < drafts.Count; i++)
            effects[i] = new StatEffect { type = drafts[i].statType, amount = drafts[i].amount, revealDelay = drafts[i].revealDelay };
        return effects;
    }

    private void ResetForm()
    {
        vendorName = offerText = acceptResultText = rejectResultText = acceptedDelayedText = rejectedDelayedText = "";
        acceptedEffects.Clear();
        rejectedEffects.Clear();
        claimedEffects.Clear();
    }

    private void BeginSection(string title)
    {
        EditorGUILayout.Space(7);
        EditorGUILayout.LabelField(title, sectionStyle);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    }

    private static void EndSection() => EditorGUILayout.EndVertical();

    private static void DrawTextField(string label, string tooltip, ref string value, float minHeight)
    {
        EditorGUILayout.LabelField(new GUIContent(label, tooltip), EditorStyles.boldLabel);
        value = EditorGUILayout.TextArea(value, GUILayout.MinHeight(minHeight));
        EditorGUILayout.Space(3);
    }

    private void InitialiseStyles()
    {
        titleStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 19, normal = { textColor = new Color(0.45f, 0.9f, 0.67f) } };
        cardStyle ??= new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(14, 14, 12, 12) };
        sectionStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, normal = { textColor = new Color(0.55f, 0.78f, 1f) } };
    }

    private static string SanitizeFileName(string value)
    {
        foreach (var invalidCharacter in Path.GetInvalidFileNameChars()) value = value.Replace(invalidCharacter.ToString(), string.Empty);
        return value;
    }

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        var parentFolder = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(parentFolder)) EnsureFolderExists(parentFolder);
        AssetDatabase.CreateFolder(parentFolder, Path.GetFileName(folderPath));
    }
}
