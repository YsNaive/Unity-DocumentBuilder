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
    public const string prefix = @"(?:(?:(public|private|protected|internal|static|readonly|override)\s*)*\s+)?";
    public const string classPattern = @"\b" + prefix + @"(class|enum|interface)\s+(\w+)\s*((?::\s*(?:\w+))(?:(?:\s*,\s*(?:\w+))*))?";
    public const string stringPattern = @"""([^""]*)""";
    public const string test = "class|interface|enum|if|else|switch|case|default|do|while|for|foreach|break|continue|goto|return|using|using static|new";
    public const string controlReservedWordPattern = @"\b(?:if|else|switch|case|do|while|for|foreach|break|continue|goto|return|catch|try)\b";
    public const string funcPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + type + @"\s+(\w+)\s*\((?:,?\s*(params|this|in|out|ref)?\s*(" + type + @")\s+(\w+))*\)";
    public const string fieldPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + type + @"\s+(\w+)\s*(?:=.*?)?\s*(?:;|,|{)";
    public const string commentPattern = @"//.*[\n]|/\*[\s\S]*?\*/";
    public const string newPattern = @"\bnew\s+" + type + @"(?:;|\()";
    public const string reservedWordPattern = @"\b(?:in|get|set|public|private|protected|static|abstract|as|base|bool|byte|char|checked|const|decimal|default|delegate|double|enum|event|explicit|extern|false|finally|fixed|float|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|params|readonly|ref|sbyte|sealed|short|sizeof|stackalloc|string|struct|this|throw|true|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile)\b";
    public const string methodPattern = @"\b" + type + @"\(";
    public const string numberPattern = @"[^""A-Za-z_][+-]?(\d+(?:\.\d+)?[fx]?)";
    public const string functype = @"((?:(?:[<,]\s*)?\b(\w+)(?:\[.*?\])?[>]*)+)";
    public const string functionPattern = @"\b" + prefix + @"(?!" + test + @"\b)" + functype + @"\s+(\w+)\s*\((?:,?\s*(params|this|in|out|ref)?\s*(" + functype + @")\s+(\w+))*\)";

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
            foreach (Capture capture in match.Groups[2].Captures)
                offset += stringBuilder.UnityRTF(offset + capture.Index, capture.Length, SODocStyle.Current.TypeColor);
            offset += stringBuilder.UnityRTF(offset + match.Groups[3].Index, match.Groups[3].Length, SODocStyle.Current.FuncColor);
            CaptureCollection prefixCaptures = match.Groups[4].Captures;
            CaptureCollection typeCaptures = match.Groups[6].Captures;
            CaptureCollection argsCaptures = match.Groups[7].Captures;
            int count = argsCaptures.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (prefixCaptures[i].Value != "")
                    offset += stringBuilder.UnityRTF(offset + prefixCaptures[i].Index, prefixCaptures[i].Length, SODocStyle.Current.PrefixColor);
                int argsIndex = argsCaptures[i].Index;
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
        int offset = 0; 
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Length, match.Value, "Comment", SODocStyle.Current.CommentsColor);
            checkAndAdd(table, match.Index, matchData);
        }
        matches = Regex.Matches(stringBuilder.ToString(), stringPattern);
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Length, match.Value, "String", SODocStyle.Current.StringColor);
            checkAndAdd(table, match.Index, matchData);
        }
        matches = Regex.Matches(stringBuilder.ToString(), fieldPattern);
        foreach (Match match in matches)
        {
            MatchData matchData;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                matchData = new MatchData(capture.Length, capture.Value, "Prefix", SODocStyle.Current.PrefixColor);
                checkAndAdd(table, capture.Index, matchData);
            }
            foreach (Capture capture in match.Groups[2].Captures)
            {
                matchData = new MatchData(capture.Length, capture.Value, "Type", SODocStyle.Current.TypeColor);
                checkAndAdd(table, capture.Index, matchData);
            }
            matchData = new MatchData(match.Groups[3].Length, match.Groups[3].Value, "Arg", SODocStyle.Current.ArgsColor);
            checkAndAdd(table, match.Groups[3].Index, matchData);
            args.Append(matchData.Value);
            args.Append("|");
        }
        matches = Regex.Matches(stringBuilder.ToString(), reservedWordPattern);
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Length, match.Value, "ReserveWord", SODocStyle.Current.PrefixColor);
            checkAndAdd(table, match.Index, matchData);
        }
        matches = Regex.Matches(stringBuilder.ToString(), numberPattern);
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Groups[1].Length, match.Groups[1].Value, "Number", SODocStyle.Current.NumberColor);
            checkAndAdd(table, match.Groups[1].Index, matchData);
        }
        matches = Regex.Matches(stringBuilder.ToString(), funcPattern);
        foreach (Match match in matches)
        {
            MatchData matchData;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                matchData = new MatchData(capture.Length, capture.Value, "Prefix", SODocStyle.Current.PrefixColor);
                checkAndAdd(table, capture.Index, matchData);
            }
            foreach (Capture capture in match.Groups[2].Captures)
            {
                matchData = new MatchData(capture.Length, capture.Value, "Type", SODocStyle.Current.TypeColor);
                checkAndAdd(table, capture.Index, matchData);
            }
            matchData = new MatchData(match.Groups[3].Length, match.Groups[3].Value, "Func", SODocStyle.Current.FuncColor);
            checkAndAdd(table, match.Groups[3].Index, matchData);
            CaptureCollection typeCaptures = match.Groups[6].Captures;
            CaptureCollection argsCaptures = match.Groups[7].Captures;
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
            MatchData matchData;
            foreach (Capture capture in match.Groups[1].Captures)
            {
                matchData = new MatchData(capture.Length, capture.Value, "Prefix", SODocStyle.Current.PrefixColor);
                checkAndAdd(table, capture.Index, matchData);
            }
            matchData = new MatchData(match.Groups[2].Length, match.Groups[2].Value, "Prefix", SODocStyle.Current.PrefixColor);
            checkAndAdd(table, match.Groups[2].Index, matchData);
            matchData = new MatchData(match.Groups[3].Length, match.Groups[3].Value, "Type", SODocStyle.Current.TypeColor);
            checkAndAdd(table, match.Groups[3].Index, matchData);
            if (match.Groups[4].Value != "")
            {
                matchData = new MatchData(match.Groups[4].Length, match.Groups[4].Value, "Type", SODocStyle.Current.TypeColor);
                checkAndAdd(table, match.Groups[4].Index, matchData);
            }
        }
        matches = Regex.Matches(stringBuilder.ToString(), controlReservedWordPattern);
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Length, match.Value, "Control", SODocStyle.Current.ControlColor);
            checkAndAdd(table, match.Index, matchData);
        }
        matches = Regex.Matches(stringBuilder.ToString(), newPattern);
        foreach (Match match in matches)
        {
            foreach (Capture capture in match.Groups[1].Captures)
            {
                MatchData matchData = new MatchData(capture.Length, capture.Value, "Type", SODocStyle.Current.TypeColor);
                checkAndAdd(table, capture.Index, matchData);
            }
        }
        matches = Regex.Matches(stringBuilder.ToString(), methodPattern);
        foreach (Match match in matches)
        {
            CaptureCollection captures = match.Groups[1].Captures;
            MatchData matchData = new MatchData(captures[0].Length, captures[0].Value, "Func", SODocStyle.Current.FuncColor);
            checkAndAdd(table, captures[0].Index, matchData);
            for (int i = 1; i < captures.Count; i++)
            {
                matchData = new MatchData(captures[i].Length, captures[i].Value, "Type", SODocStyle.Current.TypeColor);
                checkAndAdd(table, captures[i].Index, matchData);
            }
        }/*
        matches = Regex.Matches(stringBuilder.ToString(), instancePattern);
        foreach (Match match in matches)
        {
            MatchData matchData = new MatchData(match.Groups[1].Length, match.Groups[1].Value, "Arg", DocStyle.Current.ArgsColor);
            checkAndAdd(table, match.Groups[1].Index, matchData);
        }*/
        matches = Regex.Matches(stringBuilder.ToString(), @"\b(?:" + args.ToString() + @")\b");
        foreach (Match match in matches)
        {
            if (match.Value != "")
            {
                MatchData matchData = new MatchData(match.Length, match.Value, "Arg", SODocStyle.Current.ArgsColor);
                checkAndAdd(table, match.Index, matchData);
            }
        }
        offset = 0;
        int index = 0;
        foreach (var key in table.Keys)
        {
            if (key < index)
                continue;
            MatchData matchData = table[key];
            int temp = stringBuilder.UnityRTF(offset + key, matchData.length, matchData.color);
            offset += temp;
            if (matchData.type == "Comment" || matchData.type == "String")
                index = key + matchData.length;
            else
                index = -1;
        }

        return stringBuilder.ToString();
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

    public class FuncData
    {
        public string ReturnType = "", Name = "";
        public List<string> paramsType = new List<string>();
        public List<string> paramsName = new List<string>();

        public FuncData()
        {

        }

        public FuncData(string syntax)
        {
            ReturnType = "";
            paramsType.Clear();
            paramsName.Clear();
            MatchCollection matches = Regex.Matches(syntax, functionPattern);
            foreach (Match match in matches)
            {
                ReturnType = match.Groups[2].Value;
                Name = match.Groups[4].Value;
                CaptureCollection typeCaptures = match.Groups[7].Captures;
                CaptureCollection argsCaptures = match.Groups[9].Captures;
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
