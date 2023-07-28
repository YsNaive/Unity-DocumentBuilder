using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DocumentBuilderParser
{
    public static string ParseSyntax(string synatx)
    {
        try
        {
            if (string.IsNullOrEmpty(synatx)) return "";
            string[] strs = synatx.Split('(');
            strs[1] = strs[1].Substring(0, strs[1].LastIndexOf(")"));

            string[] prefixs = strs[0].Split(" ");
            StringBuilder stringBuilder = new StringBuilder();
            string subFrontGroundColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.SubFrontGroundColor)}>";
            string prefixColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.PrefixColor)}>";
            string funcColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.FuncColor)}>";
            string typeColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.TypeColor)}>";
            string paramColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.ArgsColor)}>";
            string postfixColor = "</color>";
            foreach (string str in prefixs)
            {
                if (str == prefixs[^1] && str != "void")
                {
                    stringBuilder.Append(funcColor);
                }
                else if (str == prefixs[^2] && str != "void")
                {
                    stringBuilder.Append(typeColor);
                }
                else
                {
                    stringBuilder.Append(prefixColor);
                }
                stringBuilder.Append(str);
                stringBuilder.Append(postfixColor);
                if (str != prefixs[^1])
                    stringBuilder.Append(" ");
            }

            stringBuilder.Append("(");
            foreach (string str in strs[1].Split(","))
            {
                if (str == "")
                {
                    stringBuilder.Append("  ");
                    break;
                }
                string[] param = str.Split(" ");
                int index = 0;
                while (param[index] == "")
                    index++;
                if (param[index] == "ref" || param[index] == "out" || param[index] == "in")
                {
                    stringBuilder.Append(prefixColor);
                    stringBuilder.Append(param[index]);
                    stringBuilder.Append(postfixColor);
                    stringBuilder.Append(" ");
                    index++;
                }
                stringBuilder.Append(typeColor);
                stringBuilder.Append(param[index++]);
                stringBuilder.Append(postfixColor);
                while (param[index] == "")
                    index++;
                stringBuilder.Append(" ");
                stringBuilder.Append(paramColor);
                stringBuilder.Append(param[index++]);
                stringBuilder.Append(postfixColor);
                if (param.Length > index + 1)
                {
                    stringBuilder.Append(subFrontGroundColor);
                    while (index < param.Length)
                    {
                        stringBuilder.Append(" ");
                        stringBuilder.Append(param[index++]);
                    }
                    stringBuilder.Append(postfixColor);
                }
                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(");");

            return stringBuilder.ToString();
        }
        catch
        {
            Debug.LogWarning("DocVisual : DocFuncDisplay Syntax Error");
            return synatx;
        }
    }

    public static string CSharpParser(string data)
    {
        string classPattern = @"\b((?:(?:public|private|protected|internal|static|readonly|override)\s*)*\s+)?(class)\s+(\w+)\s*((?::\s*(?:\w+))(?:(?:\s*,\s*(?:\w+))*))?";
        string stringPattern = @"""([^""]*)""";
        string test = "class|interface|enum|if|else|switch|case|default|do|while|for|foreach|in|break|continue|goto|return|using|using static|new";
        string controlReservedWordPattern = @"\b(?:if|else|switch|case|do|while|for|foreach|in|break|continue|goto|return|catch|try)\b";
        string funcPattern = @"\b((?:(?:public|private|protected|internal|static|readonly|override)\s*)*\s+)?(?!" + test + @"\b)(\w+\s+)(\w+\s*)\(((?:[^)])*)\)";
        string fieldPattern = @"\b(?:((?:(?:public|private|protected|internal|static|readonly|override)\s*)*)\s+)?(?!" + test + @"\b)(\w+\s+)(\w+\s*)(?:=.*?)?(?:;|{)";

        string reservedWordPattern = @"\b(?:public|private|protected|abstract|as|base|bool|byte|char|checked|const|decimal|default|delegate|double|enum|event|explicit|extern|false|finally|fixed|float|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|params|readonly|ref|sbyte|sealed|short|sizeof|stackalloc|string|struct|this|throw|true|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile)\b";

        string instancePattern = @"[^.]\b([A-Za-z_]\w*)[.]";
        string methodPattern = @"\b[.](\w+)(?:<(\w+)>)?\(";
        string numberPattern = @"[^""][+-]?(\d+(?:\.\d+)?[fx]?)";

        //string data = File.ReadAllText("C:/Users/howar/Desktop/Unity/Document Builder/Assets/DocumentBuilder/Editor/DocComponents/DocEditFuncDisplay.cs");
        MatchCollection matches;
        StringBuilder stringBuilder = new StringBuilder(data);
        int offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), numberPattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Groups[1].Index, match.Groups[1].Length, DocStyle.Current.NumberColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), fieldPattern);
        foreach (Match match in matches)
        {
            if (match.Groups[1].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[1].Index, match.Groups[1].Length, DocStyle.Current.PrefixColor);
            offset += stringBuilder.UnityRTF(offset + match.Groups[2].Index, match.Groups[2].Length, DocStyle.Current.TypeColor);
            offset += stringBuilder.UnityRTF(offset + match.Groups[3].Index, match.Groups[3].Length, DocStyle.Current.ArgsColor);
        }
        matches = Regex.Matches(stringBuilder.ToString(), funcPattern);
        foreach (Match match in matches)
        {
            string str = DocumentBuilderParser.ParseSyntax(match.Value);
            stringBuilder.Replace(match.Value, str);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), classPattern);
        foreach (Match match in matches)
        {
            if (match.Groups[1].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[1].Index, match.Groups[1].Length, DocStyle.Current.PrefixColor);
            if (match.Groups[2].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[2].Index, match.Groups[2].Length, DocStyle.Current.PrefixColor);
            if (match.Groups[3].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[3].Index, match.Groups[3].Length, DocStyle.Current.TypeColor);
            if (match.Groups[4].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[4].Index, match.Groups[4].Length, DocStyle.Current.TypeColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), methodPattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Groups[1].Index, match.Groups[1].Length, DocStyle.Current.FuncColor);
            if (match.Groups[2].Value != "")
                offset += stringBuilder.UnityRTF(offset + match.Groups[2].Index, match.Groups[2].Length, DocStyle.Current.TypeColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), instancePattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Groups[1].Index, match.Groups[1].Length, DocStyle.Current.ArgsColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), stringPattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Index, match.Length, DocStyle.Current.StringColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), reservedWordPattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Index, match.Length, DocStyle.Current.PrefixColor);
        }
        offset = 0;
        matches = Regex.Matches(stringBuilder.ToString(), controlReservedWordPattern);
        foreach (Match match in matches)
        {
            offset += stringBuilder.UnityRTF(offset + match.Index, match.Length, DocStyle.Current.ControlColor);
        }

        return stringBuilder.ToString();
    }
}
