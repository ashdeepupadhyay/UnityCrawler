using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TagsCollecterForScripts
{

    const string FEATURE_TAG_SIGNATURE = "FEATUREID:";
    const string LOC_TAG_SIGNATURE = "LOCTAG:";
    const string COMMENT_TAG_SIGNATURE = "//";

    public IEnumerable<string> GetFilesRecursively(string root, string extention)
    {
        foreach (string a in Directory.GetFiles(root))
        {
            if (Path.GetExtension(a) == extention)
            {
                yield return a;
            }
        }
        foreach (string a in Directory.GetDirectories(root))
        {
            foreach (string b in GetFilesRecursively(a, extention))
            {
                yield return b;
            }
        }
    }

    public void UpdateLocTags()
    {
        List<string> csFileDirectories = new List<string>(GetFilesRecursively(Application.dataPath, ".cs"));
        List<string> existingIDs = new List<string>();

        foreach (string fileDirectory in csFileDirectories)
        {
            string fileName = Path.GetFileName(fileDirectory);
            if (fileName == "Check.cs")
            {
                continue;
            }

            string fileText = File.ReadAllText(fileDirectory);
            if (fileText.Contains(FEATURE_TAG_SIGNATURE))
            {
                string locfeatureid = "";
                string[] fileLines = fileText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in fileLines)
                {
                    if (line.Contains(FEATURE_TAG_SIGNATURE) || line.Contains(LOC_TAG_SIGNATURE))
                    {

                        string[] splitLine = line.Contains(FEATURE_TAG_SIGNATURE) ? line.Split(new string[] { FEATURE_TAG_SIGNATURE }, StringSplitOptions.None) :
                                                                                    line.Split(new string[] { LOC_TAG_SIGNATURE }, StringSplitOptions.None);
                        string locTextValue = splitLine[1].Trim();
                        if (line.Contains(FEATURE_TAG_SIGNATURE))
                        {
                            locfeatureid = locTextValue;
                        }
                        if (!string.IsNullOrEmpty(locTextValue))
                        {
                            existingIDs.Add(locTextValue);
                            if (!TagsChecker.tagsDict.ContainsKey(locfeatureid) && locfeatureid != "")
                            {
                                TagsChecker.tagsDict.Add(locTextValue, new Dictionary<string, string>());
                            }
                            else if (locfeatureid != "" && locTextValue != locfeatureid)
                            {

                                string newTagValue = splitLine[0].Trim();

                                if (newTagValue.Contains(COMMENT_TAG_SIGNATURE))
                                {
                                    string[] newValue = newTagValue.Split(new string[] { COMMENT_TAG_SIGNATURE }, StringSplitOptions.None);
                                    string locValue = newValue[1].Trim(new char[] { '"', ' ' });
                                    if (!TagsChecker.tagsDict[locfeatureid].ContainsKey(locTextValue))
                                    {
                                        TagsChecker.tagsDict[locfeatureid].Add(locTextValue, locValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

