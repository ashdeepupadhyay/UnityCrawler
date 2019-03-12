using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LiveLikeLoc
{
    public class LocalizationChecklist : ScriptableWizard
    {

        TagsChecker checkingTag;
        static int lengthOfEnum = Enum.GetNames(typeof(FeatureId)).Length;
        bool[] features = new bool[lengthOfEnum];
        bool buttonPressed;
        Vector2 scrollPosition;

        [MenuItem("Tools/Localisation Checklist")]

        static void Init()
        {
            LocalizationChecklist checklistWindow = (LocalizationChecklist)EditorWindow.GetWindow(typeof(LocalizationChecklist));
            checklistWindow.Show();
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label("Features", EditorStyles.boldLabel);
            for (int i = 0; i < lengthOfEnum; i++)
            {
                features[i] = EditorGUILayout.ToggleLeft(Enum.GetName(typeof(FeatureId), i), features[i]);
            }
            buttonPressed = GUILayout.Button("Make CSV");
            EditorGUILayout.EndScrollView();
            if (buttonPressed)
            {
                CreateCSV(features);
            }
        }

        private void CreateCSV(bool[] features)
        {
            checkingTag = new TagsChecker();
            TagsChecker.tagsDict.Clear();
            List<String> featureNames = new List<String>();
            string featureName = null;
            checkingTag.Collect();
            for (int i = 0; i < lengthOfEnum; i++)
            {
                if (features[i])
                {
                    featureName = Enum.GetName(typeof(FeatureId), i);
                    featureNames.Add(featureName);
                    try
                    {
                        checkingTag.printACSV(new List<String>() { featureName }, featureName);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Can't make CSV for " + featureName + " feature. Error : " + e.ToString());
                    }
                }
            }
            try
            {
                checkingTag.printACSV(featureNames, null);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't make CSV. Error : " + e.ToString());
            }
        }

    }
}