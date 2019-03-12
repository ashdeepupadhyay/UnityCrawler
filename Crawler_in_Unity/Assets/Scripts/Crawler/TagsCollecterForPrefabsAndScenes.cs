using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections.Generic;


public class TranslatedLocLine
{
    public string id;
    public string locValue;
    public FeatureId featureid;
}

public class TagsCollecterForPrefabsAndScenes
{

    private IEnumerable<T> GetAllComponentsRecursive<T>(Transform root) where T : Component
    {
        T[] components = root.gameObject.GetComponents<T>();

        foreach (T a in components)
        {
            yield return a;
        }
        foreach (Transform a in root)
        {
            foreach (T b in GetAllComponentsRecursive<T>(a))
            {
                yield return b;
            }
        }
    }
    private void AddTranslatedLocLineToDictionary(TranslatedLocLine locText)
    {
        if (!TagsChecker.tagsDict.ContainsKey(locText.featureid.ToString()))
        {
            TagsChecker.tagsDict.Add(locText.featureid.ToString(), new Dictionary<string, string>());
        }
        if (!TagsChecker.tagsDict[locText.featureid.ToString()].ContainsKey(locText.id))
        {
            TagsChecker.tagsDict[locText.featureid.ToString()].Add(locText.id, locText.locValue);
        }
        else
        {
            if (string.IsNullOrEmpty(TagsChecker.tagsDict[locText.featureid.ToString()][locText.id]) && TagsChecker.tagsDict[locText.featureid.ToString()].ContainsKey(locText.id))
            {
                TagsChecker.tagsDict[locText.featureid.ToString()][locText.id] = locText.locValue;
            }
        }
    }
    public void CollectLocTextFields()
    {
        System.Action<Localisation> recordLocTextfield = (Localisation locTextfield) =>
        {
            string locTextValue;
            if (locTextfield.GetComponent<Text>() != null)
            {
                locTextValue = locTextfield.GetComponent<Text>().text;
            }
            else
            {
                locTextValue = "";
            }
            locTextValue = locTextValue.Replace('\n', ' ');
            locTextValue = locTextValue.Replace('\r', ' ');

            TranslatedLocLine newLine = new TranslatedLocLine();
            if (!string.IsNullOrEmpty(locTextfield.locID))
            {
                newLine.id = locTextfield.locID;
            }
            newLine.locValue = locTextValue;
            newLine.featureid = locTextfield.feature;
            AddTranslatedLocLineToDictionary(newLine);
        };
       
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        //For Prefabs
        foreach (string assetPath in assetPaths)
        {
            try
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (asset != null)
                {
                    List<Localisation> textFields = new List<Localisation>(GetAllComponentsRecursive<Localisation>(asset.transform));
                    
                    foreach (Localisation locTextfield in textFields)
                    {
                        if (locTextfield)
                        {

                            recordLocTextfield(locTextfield);
                        }
                    }                 
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error when loading file: " + assetPath + "\n" + e.ToString());
            }
        }
        //For Scenes
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
            {
                continue;
            }
            EditorApplication.OpenScene(scene.path);

            foreach (Transform b in GameObject.FindObjectsOfType<Transform>())
            {
                if (b.parent == null)
                {
                    foreach (Localisation c in GetAllComponentsRecursive<Localisation>(b))
                    {                      
                        recordLocTextfield(c);                       
                    }                  
                }
            }
        }
    }
}
