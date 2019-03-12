using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TagsChecker
{

    public static Dictionary<string, Dictionary<string, string>> tagsDict = new Dictionary<string, Dictionary<string, string>>();
    const string CSV_CHARACTER = "|";
    const string clientList = "clientList.csv";
    string featureCSVPath = Path.Combine(Application.streamingAssetsPath, "FeatureCSV");

    public void Collect()
    {
        TagsCollecterForScripts tag = new TagsCollecterForScripts();
        tag.UpdateLocTags();
        TagsCollecterForPrefabsAndScenes p = new TagsCollecterForPrefabsAndScenes();
        p.CollectLocTextFields();
    }
    public string Serialize(string id, string value)
    {
        return string.Concat(id, CSV_CHARACTER, value);
    }

    public void printACSV(List<string> checkList, string featureName)
    {
        Dictionary<string, string> tempDict = new Dictionary<string, string>();
        string clientListPath;
        createDirectoryForFeatureCSV();
        if (String.IsNullOrEmpty(featureName))
        {
            clientListPath = Path.Combine(Application.streamingAssetsPath, clientList);
        }
        else
        {
            string temp= Path.Combine(Application.streamingAssetsPath, "FeatureCSV");
            clientListPath = Path.Combine(temp, String.Concat(featureName, ".csv"));
        }

        using (StreamWriter sw = new StreamWriter(clientListPath))
        {
            foreach (var list in checkList)
            {
                if (tagsDict.ContainsKey(list))
                {
                    foreach (KeyValuePair<string, string> ka in tagsDict[list])
                    {
                        if (!tempDict.ContainsKey(ka.Key))
                        {
                            tempDict.Add(ka.Key, ka.Value);
                        }
                        else if (tempDict.ContainsKey(ka.Key) && string.IsNullOrEmpty(tempDict[ka.Key]))
                        {
                            tempDict[ka.Key] = ka.Value;
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, string> ka in tempDict)
            {
                sw.WriteLine(Serialize(ka.Key, ka.Value));
            }

        }
        AssetDatabase.Refresh();
    }

    public void createDirectoryForFeatureCSV()
    {
        try
        {
            if (Directory.Exists(featureCSVPath))
            {
                Debug.Log("That directory exists already.");
                return;
            }

            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(featureCSVPath);
            Debug.Log("The directory was created successfully at " + Directory.GetCreationTime(featureCSVPath));
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create directory: " + e.ToString());
        }
    }
}
