using Challenges;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ChallengeData))]
    public class ChallengeDataEditor : UnityEditor.Editor
    {
        private SerializedProperty displayName;
        private SerializedProperty description;
        private SerializedProperty hasStartingStatChange;
        private SerializedProperty startingStat;
        private SerializedProperty startingValue;
        private SerializedProperty startingMaxStat;
        private SerializedProperty startingMaxValue;
        private SerializedProperty objectiveType;
        private SerializedProperty durationInDays;
        private SerializedProperty trackedStat;
        private SerializedProperty maximumValue;
        private SerializedProperty minimumValue;
        private SerializedProperty requiredCount;
        private SerializedProperty countScope;
        private SerializedProperty rewardType;
        private SerializedProperty rewardStat;
        private SerializedProperty rewardValue;

        private void OnEnable()
        {
            displayName = serializedObject.FindProperty("displayName");
            description = serializedObject.FindProperty("description");
            hasStartingStatChange = serializedObject.FindProperty("hasStartingStatChange");
            startingStat = serializedObject.FindProperty("startingStat");
            startingValue = serializedObject.FindProperty("startingValue");
            startingMaxStat = serializedObject.FindProperty("startingMaxStat");
            startingMaxValue = serializedObject.FindProperty("startingMaxValue");
            objectiveType = serializedObject.FindProperty("objectiveType");
            durationInDays = serializedObject.FindProperty("durationInDays");
            trackedStat = serializedObject.FindProperty("trackedStat");
            maximumValue = serializedObject.FindProperty("maximumValue");
            minimumValue = serializedObject.FindProperty("minimumValue");
            requiredCount = serializedObject.FindProperty("requiredCount");
            countScope = serializedObject.FindProperty("countScope");
            rewardType = serializedObject.FindProperty("rewardType");
            rewardStat = serializedObject.FindProperty("rewardStat");
            rewardValue = serializedObject.FindProperty("rewardValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawIdentity();
            EditorGUILayout.Space(8f);
            DrawStartingEffect();
            EditorGUILayout.Space(8f);
            DrawObjective();
            EditorGUILayout.Space(8f);
            DrawReward();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawIdentity()
        {
            EditorGUILayout.LabelField("IDENTITY", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(displayName, new GUIContent("Display Name"));
            EditorGUILayout.PropertyField(description, new GUIContent("Description"));
        }

        private void DrawStartingEffect()
        {
            EditorGUILayout.LabelField("STARTING STAT EFFECT", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(hasStartingStatChange, new GUIContent("Override Starting Stat"));

            if (!hasStartingStatChange.boolValue)
            {
                EditorGUILayout.HelpBox("The run uses its normal starting stats.", MessageType.None);
                return;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(startingStat, new GUIContent("Stat"));
                EditorGUILayout.PropertyField(startingValue, new GUIContent("Starting Value"));
            
                EditorGUILayout.PropertyField(startingMaxStat, new GUIContent("Stat"));
                EditorGUILayout.PropertyField(startingMaxValue, new GUIContent("Starting Value"));
            }
        }

        private void DrawObjective()
        {
            EditorGUILayout.LabelField("OBJECTIVE", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(objectiveType, new GUIContent("Objective Type"));
            EditorGUILayout.PropertyField(durationInDays, new GUIContent("Duration (Days)"));

            if (durationInDays.intValue < 1)
                EditorGUILayout.HelpBox("Duration must be at least one day.", MessageType.Error);

            ObjectiveType type = (ObjectiveType)objectiveType.enumValueIndex;

            using (new EditorGUI.IndentLevelScope())
            {
                if (IsRangeObjective(type))
                {
                    EditorGUILayout.PropertyField(trackedStat, new GUIContent("Tracked Stat"));
                    EditorGUILayout.PropertyField(minimumValue, new GUIContent("Minimum Value"));
                    EditorGUILayout.PropertyField(maximumValue, new GUIContent("Maximum Value"));

                    if (minimumValue.intValue > maximumValue.intValue)
                        EditorGUILayout.HelpBox("Minimum Value cannot be greater than Maximum Value.", MessageType.Error);
                }
                else if (IsAboveObjective(type))
                {
                    EditorGUILayout.PropertyField(trackedStat, new GUIContent("Tracked Stat"));
                    EditorGUILayout.PropertyField(minimumValue, new GUIContent("Minimum Value"));
                }
                else if (IsBelowObjective(type))
                {
                    EditorGUILayout.PropertyField(trackedStat, new GUIContent("Tracked Stat"));
                    EditorGUILayout.PropertyField(maximumValue, new GUIContent("Maximum Value"));
                }
                else if (IsCountObjective(type))
                {
                    EditorGUILayout.PropertyField(requiredCount, new GUIContent("Required Count"));
                    EditorGUILayout.PropertyField(countScope, new GUIContent("Count Scope"));

                    if (requiredCount.intValue < 1)
                        EditorGUILayout.HelpBox("Required Count must be at least one.", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Survive until the final challenge day.", MessageType.None);
                }
            }
        }

        private void DrawReward()
        {
            EditorGUILayout.LabelField("SUCCESS REWARD", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(rewardType, new GUIContent("Reward Type"));

            RewardType type = (RewardType)rewardType.enumValueIndex;
            using (new EditorGUI.IndentLevelScope())
            {
                switch (type)
                {
                    case RewardType.IncreaseMaxKeepingPercentage:
                        EditorGUILayout.PropertyField(rewardStat, new GUIContent("Stat"));
                        EditorGUILayout.PropertyField(rewardValue, new GUIContent("New Maximum"));
                        break;

                    case RewardType.CheatDeath:
                        EditorGUILayout.PropertyField(rewardStat, new GUIContent("Protected Stat"));
                        EditorGUILayout.HelpBox("The next game-over caused by this stat restores it to 50 and consumes the shield.", MessageType.None);
                        break;

                    case RewardType.NormalizeStat:
                        EditorGUILayout.PropertyField(rewardStat, new GUIContent("Stat"));
                        EditorGUILayout.HelpBox("Sets this stat to half of its current maximum value.", MessageType.None);
                        break;

                    case RewardType.IgnoreUpkeep:
                        EditorGUILayout.PropertyField(rewardStat, new GUIContent("Protected Stat"));
                        EditorGUILayout.HelpBox("Negative upkeep effects for this stat are removed for the rest of the run.", MessageType.None);
                        break;
                }
            }
        }

        private static bool IsRangeObjective(ObjectiveType type)
        {
            return type == ObjectiveType.KeepStatInRangeAtEnd ||
                   type == ObjectiveType.KeepStatInRangeAlways;
        }

        private static bool IsAboveObjective(ObjectiveType type)
        {
            return type == ObjectiveType.KeepStatAboveAlways ||
                   type == ObjectiveType.KeepStatAboveAtEnd;
        }

        private static bool IsBelowObjective(ObjectiveType type)
        {
            return type == ObjectiveType.KeepStatBelowAlways ||
                   type == ObjectiveType.KeepStatBelowAtEnd;
        }

        private static bool IsCountObjective(ObjectiveType type)
        {
            return type == ObjectiveType.AcceptOffers ||
                   type == ObjectiveType.RejectOffers;
        }
    }
}
