using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DocumentBuilderParser
{
    public const string type = @"(?:(?:[<,]\s*)?\b(\w+)(?:\[.*?\])?[>]*)+";
    public const string functype = @"((?:(?:[<,]\s*)?\b(\w+)(?:\[.*?\])?[>]*)+)";
    public const string prefix = @"(?:(?:(public|private|protected|internal|static|readonly|override)\s*)*\s+)?";
    public const string test = "class|interface|enum|if|else|switch|case|default|do|while|for|foreach|break|continue|goto|return|using|using static|new";
    public const string equal = @"";
    public const string classPattern = @"\b" + prefix + @"(class|enum|interface)\s+(?:(?:\s*[:,]\s*)?(\w+))*";
    public const string stringPattern = @"""([^""]*)""";
    public const string controlReservedWordPattern = @"\b(?:if|else|switch|case|do|while|for|foreach|break|continue|goto|return|catch|try)\b";
    public const string funcPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + functype + @"\s+" + type + @"\s*\((?:,?\s*(params|this|in|out|ref)?\s*(" + type + @")\s+(\w+)(?:\s*=\s*[^\)]*)?)*\)";
    public const string fieldPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + type + @"\s+(\w+)\s*(?:=.*?)?\s*(?:;|,|{)";
    public const string commentPattern = @"//.*[\n]|/\*[\s\S]*?\*/";
    public const string newPattern = @"\bnew\s+" + type + @"(?:;|\()";
    public const string reservedWordPattern = @"\b(?:in|get|set|public|private|protected|static|abstract|as|base|bool|byte|char|checked|const|decimal|default|delegate|double|enum|event|explicit|extern|false|finally|fixed|float|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|params|readonly|ref|sbyte|sealed|short|sizeof|stackalloc|string|struct|this|throw|true|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile)\b";
    public const string methodPattern = @"\b" + type + @"\(";
    public const string numberPattern = @"[^""A-Za-z_][+-]?(\d+(?:\.\d+)?[fx]?)";
    public const string functionPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + functype + @"\s+" + type + @"\s*\((?:,?\s*(params|this|in|out|ref)?\s*(" + type + @")\s+(\w+)(?:\s*\=\s*[^\)]*)?)*\)";

    public static string CalGenericTypeName(Type type)
    {
        string name = type.Name;
        int i = name.IndexOf('`');
        if (i != -1)
            name = name.Substring(0, i);
        else
            return GetTypeName(name);
        i = 0;
        name += "<";
        foreach (var arg in type.GenericTypeArguments)
        {
            name += ((i != 0) ? ", " : "") + CalGenericTypeName(arg);
            i++;
        }
        name += ">";
        return name;
    }

    public static string GetTypeName(string typeName)
    {
        switch (typeName)
        {
            case "Void":
                return "void";
            case "Int32":
                return "int";
            case "String":
                return "string";
            case "Single":
                return "float";
            case "Boolean":
                return "bool";
        }

        return typeName;
    }

    public static string ParseSyntax(string synatx)
    {
        try
        {
            if (string.IsNullOrEmpty(synatx)) return "";
            string[] strs = synatx.Split('(');
            strs[1] = strs[1].Substring(0, strs[1].LastIndexOf(")"));

            string[] prefixs = strs[0].Split(" ");
            StringBuilder stringBuilder = new StringBuilder();
            string subFrontGroundColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(SODocStyle.Current.SubFrontgroundColor)}>";
            string prefixColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(SODocStyle.Current.PrefixColor)}>";
            string funcColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(SODocStyle.Current.FuncColor)}>";
            string typeColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(SODocStyle.Current.TypeColor)}>";
            string paramColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(SODocStyle.Current.ArgsColor)}>";
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

    public static string FunctionParser(string func)
    {
        StringBuilder stringBuilder = new StringBuilder(func);

        int offset = 0;
        MatchCollection matches = Regex.Matches(stringBuilder.ToString(), funcPattern);
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Groups[1].Captures)
                offset += stringBuilder.UnityRTF(offset + capture.Index, capture.Length, SODocStyle.Current.PrefixColor);
            foreach (Capture capture in match.Groups[3].Captures)
                offset += stringBuilder.UnityRTF(offset + capture.Index, capture.Length, SODocStyle.Current.TypeColor);
            CaptureCollection funcCaptures = match.Groups[4].Captures;
            offset += stringBuilder.UnityRTF(offset + funcCaptures[0].Index, funcCaptures[0].Length, SODocStyle.Current.FuncColor);
            for (int i = 1; i < funcCaptures.Count; i++)
            {
            offset += stringBuilder.UnityRTF(offset + funcCaptures[i].Index, funcCaptures[i].Length, SODocStyle.Current.TypeColor);
            }
            CaptureCollection prefixCaptures = match.Groups[5].Captures;
            CaptureCollection typeCaptures = match.Groups[7].Captures;
            CaptureCollection argsCaptures = match.Groups[8].Captures;
            int count = argsCaptures.Count;
            bool hasPrefix = match.Groups[5].Captures.Count > 0;
            int index = 0, typeIndex, argsIndex;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                typeIndex = typeCaptures[i].Index;
                if (hasPrefix && prefixCaptures[index].Index < typeIndex)
                {
                    offset += stringBuilder.UnityRTF(offset + prefixCaptures[index].Index, prefixCaptures[index].Length, SODocStyle.Current.PrefixColor);
                    index++;
                    if (index >= prefixCaptures.Count)
                        hasPrefix = false;
                }
                argsIndex = argsCaptures[i].Index;
                while (j < typeCaptures.Count && typeCaptures[j].Index < argsIndex)
                {
                    offset += stringBuilder.UnityRTF(offset + typeCaptures[j].Index, typeCaptures[j].Length, SODocStyle.Current.TypeColor);
                    j++;
                }
                offset += stringBuilder.UnityRTF(offset + argsCaptures[i].Index, argsCaptures[i].Length, SODocStyle.Current.ArgsColor);
            }
        }

        return stringBuilder.ToString();
    }

    public static string CSharpParser(string data)
    {
        SortedDictionary<int, MatchData> table = new SortedDictionary<int, MatchData>();

        StringBuilder args = new StringBuilder();
        MatchCollection matches;
        StringBuilder stringBuilder = new StringBuilder(data);

        matches = Regex.Matches(stringBuilder.ToString(), commentPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 0, SODocStyle.Current.CommentsColor, ParseType.Single, "Comment");
        }
        matches = Regex.Matches(stringBuilder.ToString(), stringPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 0, SODocStyle.Current.StringColor, ParseType.Single, "String");
        }
        matches = Regex.Matches(stringBuilder.ToString(), fieldPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
            addTable(table, match, 2, SODocStyle.Current.TypeColor, ParseType.Capture, "Type");
            addTable(table, match, 3, SODocStyle.Current.ArgsColor, ParseType.Single, "Args");
            Debug.Log(match.Value);
            args.Append(match.Groups[3].Value);
            args.Append("|");
        }
        matches = Regex.Matches(stringBuilder.ToString(), reservedWordPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 0, SODocStyle.Current.PrefixColor, ParseType.Single, "ReserveWord");
        }
        matches = Regex.Matches(stringBuilder.ToString(), numberPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.NumberColor, ParseType.Single, "Number");
        }
        matches = Regex.Matches(stringBuilder.ToString(), funcPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
            addTable(table, match, 3, SODocStyle.Current.TypeColor, ParseType.Capture, "Type");
            addTable(table, match, 4, SODocStyle.Current.TypeColor, ParseType.Func, "Type");
            MatchData matchData;
            CaptureCollection typeCaptures = match.Groups[7].Captures;
            CaptureCollection argsCaptures = match.Groups[8].Captures;
            int count = argsCaptures.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                int argsIndex = argsCaptures[i].Index;
                while (j < typeCaptures.Count && typeCaptures[j].Index < argsIndex)
                {
                    matchData = new MatchData(typeCaptures[j].Length, typeCaptures[j].Value, "Type", SODocStyle.Current.TypeColor);
                    checkAndAdd(table, typeCaptures[j].Index, matchData);
                    j++;
                }
                matchData = new MatchData(argsCaptures[i].Length, argsCaptures[i].Value, "Arg", SODocStyle.Current.ArgsColor);
                checkAndAdd(table, argsCaptures[i].Index, matchData);
                args.Append(matchData.Value);
                args.Append("|");
            }
        }
        matches = Regex.Matches(stringBuilder.ToString(), classPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
            addTable(table, match, 2, SODocStyle.Current.PrefixColor, ParseType.Single, "Prefix");
            addTable(table, match, 3, SODocStyle.Current.TypeColor, ParseType.Capture, "Type");
        }
        matches = Regex.Matches(stringBuilder.ToString(), controlReservedWordPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 0, SODocStyle.Current.ControlColor, ParseType.Single, "Control");
        }
        matches = Regex.Matches(stringBuilder.ToString(), newPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.TypeColor, ParseType.Capture, "Type");
        }
        matches = Regex.Matches(stringBuilder.ToString(), methodPattern);
        foreach (Match match in matches)
        {
            addTable(table, match, 1, SODocStyle.Current.TypeColor, ParseType.Func, "Type");
        }
        matches = Regex.Matches(stringBuilder.ToString(), @"\b(?:" + args.ToString() + @")\b");
        foreach (Match match in matches)
        {
            addTable(table, match, 0, SODocStyle.Current.ArgsColor, ParseType.Single, "Arg");
        }
        int index = 0, offset = 0;
        foreach (var key in table.Keys)
        {
            if (key < index)
                continue;
            MatchData matchData = table[key];
            offset += stringBuilder.UnityRTF(offset + key, matchData.length, matchData.color);
            if (matchData.type == "Comment" || matchData.type == "String")
                index = key + matchData.length;
        }

        return stringBuilder.ToString();
    }

    private static void addTable(SortedDictionary<int, MatchData> table, Match match, int index, Color color, ParseType parseType, string type)
    {
        MatchData matchData;
        switch (parseType)
        {
            case ParseType.Single:
                matchData = new MatchData(match.Groups[index].Length, match.Groups[index].Value, type, color);
                checkAndAdd(table, match.Groups[index].Index, matchData);
                break;
            case ParseType.Capture:
                foreach (Capture capture in match.Groups[index].Captures)
                {
                    matchData = new MatchData(capture.Length, capture.Value, type, color);
                    checkAndAdd(table, capture.Index, matchData);
                }
                break;
            case ParseType.Func:
                CaptureCollection captures = match.Groups[index].Captures;
                matchData = new MatchData(captures[0].Length, captures[0].Value, "Func", SODocStyle.Current.FuncColor);
                checkAndAdd(table, captures[0].Index, matchData);
                for (int i = 1; i < captures.Count; i++)
                {
                    matchData = new MatchData(captures[i].Length, captures[i].Value, type, color);
                    checkAndAdd(table, captures[i].Index, matchData);
                }
                break;
        }
    }

    private static void checkAndAdd(SortedDictionary<int, MatchData> table, int index, MatchData matchData)
    {
        MatchData match;
        if (table.TryGetValue(index, out match))
        {
            if (match.type == matchData.type || match.color == matchData.color)
                return;
            else if (matchData.type == "ReserveWord")
            {
                table.Remove(index);
                table.Add(index, matchData);
            }
        }
        else
        {
            table.Add(index, matchData);
        }
    }

    public enum ParseType
    {
        Single, Capture, Func 
    }

    public class FuncData
    {
        public string ReturnType = "", Name = "";
        public List<string> paramsType = new List<string>();
        public List<string> paramsName = new List<string>();

        public FuncData() { }

        public FuncData(string syntax)
        {
            ReturnType = "";
            paramsType.Clear();
            paramsName.Clear();
            MatchCollection matches = Regex.Matches(syntax, functionPattern);
            foreach (Match match in matches)
            {
                ReturnType = match.Groups[2].Value;
                Name = match.Groups[4].Captures[0].Value;
                CaptureCollection typeCaptures = match.Groups[6].Captures;
                CaptureCollection argsCaptures = match.Groups[8].Captures;
                int count = argsCaptures.Count;
                for (int i = 0; i < count; i++)
                {
                    paramsType.Add(typeCaptures[i].Value);
                    paramsName.Add(argsCaptures[i].Value);
                }
            }
        }
    }

    public class MatchData
    {
        public int length;
        public string Value;
        public string type;
        public Color color;

        public MatchData(int length, string Value, string type, Color color)
        {
            this.length = length;
            this.Value = Value;
            this.type = type;
            this.color = color;
        }
    }
}
