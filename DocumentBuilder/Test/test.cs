using NaiveAPI.DocumentBuilder;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public UIDocument document;
    public SODocPage page;
    public DocComponent doc;

    void Start()
    {
        string classPattern = @"\b(?:public|private|protected|internal|static|sealed)?\s+class\s+(\w+)\s(?::\s+(\w+))?((?:\s*,\s*\w+)*)";
        //string classPattern = @"\b(?:public|private|protected|internal|sealed|static)?\sclass\s+\w+\s(?::\s\w+(\s,\s\w+)?)?\s{";
        string regexPattern = @"""([^""]*)""";
        string funcPattern = @"\b(?:public|private|protected|internal|static|sealed|async)?\s+(?:[\w<>,]+\s+)?\w+\s*\([^)]*\)\s*(?:=>)?\s";
        string fieldPattern = @"\b(?:public|private|protected|internal|static|readonly)?\s+(?!class|interface|enum)\w+\s+\w+\s*;";

        string data = File.ReadAllText("C:/Users/howar/Desktop/Unity/Document Builder/Assets/DocumentBuilder/RunTime/DocComponents/DocFuncDisplay.cs");
        //string data = File.ReadAllText("C:/Users/howar/Desktop/Unity/Document Builder/Assets/DocumentBuilder/Editor/DocComponents/DocEditFuncDisplay.cs");
        Debug.Log(data);
        MatchCollection matches = Regex.Matches(data, classPattern);
        /*
        foreach (Match match in matches)
        {
            Debug.Log(match.Index);
            Debug.Log(match.Length);
            print("class " + match.Value);
        }

        matches = Regex.Matches(data, funcPattern);

        foreach (Match match in matches)
        {
            print("func " + match.Value);
        }

        matches = Regex.Matches(data, fieldPattern);

        foreach (Match match in matches)
        {
            print("field " + match.Value);
        }*/

        matches= Regex.Matches(data, regexPattern);

        foreach (Match match in matches)
        {
            print("string " + match.Value);
        }
    }

    private void ParseClassFields(string scriptCode)
    {
        string pattern = @"public\s+(\w+)\s+(\w+)\s*="; 
        //string classPattern = @"\b(?:public|private|protected|internal|sealed|static)?\sclass\s+\w+\s(?::\s\w+(\s*,\s*\w+)?)?\s{"; 
        string classPattern = @"\b(?:public|private|protected|internal|static|sealed)?\s+class\s+(\w+)\s(?::\s+(\w+))?((?:\s,\s\w+))";
        string funcPattern = @"\b(?:public|private|protected|internal|static|sealed|async)?\s+(?:[\w<>,]+\s+)?\w+\s([^)])\s(?:=>)?\s{";
        string fieldPattern = @"\b(?:public|private|protected|internal|static|readonly)?\s+(?!class|interface|enum)\w+\s+\w+\s;";
        MatchCollection matches = Regex.Matches(scriptCode, classPattern);

        Debug.Log(scriptCode);
        Debug.Log(matches.Count);
        foreach (Match match in matches)
        {
            Debug.Log(match.Value);
            if (match.Groups.Count >= 3)
            {
                string fieldType = match.Groups[1].Value;
                string fieldName = match.Groups[2].Value;
                Debug.Log("Field Type: " + fieldType + ", Field Name: " + fieldName);
            }
        }
    }
}
