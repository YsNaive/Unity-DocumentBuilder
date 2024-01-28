using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public static class DocumentBuilderParser
    {
        public const string VariableRegex = @"\b([A-Za-z_]\w*)";
        public const string TypeRegex = @"(?:(?:[<,]\s*)?\b" + VariableRegex + @"(?:\[.*?\])?[>]*)+";
        public const string FuncTypeRegex = @"((?:(?:[<,]\s*)?\b" + VariableRegex + @"(?:\[.*?\])?[>]*)+)";
        public const string PrefixRegex = @"(?:(public|private|protected|internal|static|readonly|override|const|abstract)\s*)*";
        public const string FuncEqualRegex = @"(?:\s*=[^\)]*?)?";
        public const string FieldEqualRegex = @"(?:\s*=[^;{]*?)?";
        public const string ClassRegex = @"\b" + PrefixRegex + @"(class|struct|enum|interface)\s+(?:(?:\s*[:,]\s*)?(\w+))*";
        public const string StringRegex = @"""([^""]*)""";
        public const string ControlReservedWordRegex = @"\b(?:if|else|switch|case|do|while|for|foreach|break|continue|goto|return|catch|try)\b";
        private const string temp1 = "(?:class|interface|enum|if|else|switch|case|default|do|while|for|foreach|break|continue|goto|return|using|using static|new)";
        public const string FuncRegex = @"\b" + PrefixRegex + @"(?!\b" + temp1 + @"\b)" + FuncTypeRegex + @"\s+" + TypeRegex +
            @"\s*\((?:(?:,\s*)?(params|this|in|out|ref)?\s*" + FuncTypeRegex + @"\s+(\w+)" + FuncEqualRegex + @")*\)";
        public const string FieldRegex = @"\b" + PrefixRegex + @"(?!\b" + temp1 + @"\b)" + TypeRegex + @"\s+(?:" + VariableRegex + FieldEqualRegex +
            @")(?:,\s*" + VariableRegex + FieldEqualRegex + ")*(?:;|{)";
        public const string CommentRegex = @"//.*(?:\n|$)|/\*[\s\S]*?\*/";
        public const string NewConstructRegex = @"\bnew\s+" + TypeRegex + @"(?:;|\()";
        public const string ReservedWordRegex = @"\b(?:var|in|get|set|public|private|protected|static|abstract|as|base|bool|byte|char|checked|const|decimal|default|delegate|double|enum|event|explicit|extern|false|finally|fixed|float|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|params|readonly|ref|sbyte|sealed|short|sizeof|stackalloc|string|struct|this|throw|true|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile)\b";
        public const string MethodRegex = @"\b" + TypeRegex + @"\(";
        public const string NumberRegex = @"[+-]?\b(\d+(?:\.\d+)?(?:[fFLlDdBbWw]|[Xx]\d+)?)";
        private const string pattern1 = @"\b(?<var>" + VariableRegex + @")\b(?(<)(?:(?'Open'<)|" +
                @"(?<var>" + VariableRegex + @")|(?'Close-Open'>)|(?:,\s*(?<var>" + VariableRegex + @")))*)(?(Open)(?!))";
        public static string ParseSyntax(string synatx)
        {
            try
            {
                if (string.IsNullOrEmpty(synatx)) return "";
                string[] strs = synatx.Split('(');
                strs[1] = strs[1].Substring(0, strs[1].LastIndexOf(")"));

                string[] prefixs = strs[0].Split(" ");
                StringBuilder stringBuilder = new StringBuilder();
                string subFrontGroundColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.SubFrontgroundColor)}>";
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

        public static string ParseMethodSyntax(string func, bool isHTML)
        {
            StringBuilder stringBuilder = new StringBuilder(func);
            Func<StringBuilder, int, int, Color, int> action;
            if (isHTML)
                action = VisualElementExtension.HtmlRTF;
            else
                action = VisualElementExtension.UnityRTF;
            int offset = 0;
            MatchCollection matches = Regex.Matches(stringBuilder.ToString(), FuncRegex);
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Groups[1].Captures)
                    offset += action(stringBuilder, offset + capture.Index, capture.Length, DocStyle.Current.PrefixColor);
                foreach (Capture capture in match.Groups[3].Captures)
                    offset += action(stringBuilder, offset + capture.Index, capture.Length, DocStyle.Current.TypeColor);
                CaptureCollection funcCaptures = match.Groups[4].Captures;
                offset += action(stringBuilder, offset + funcCaptures[0].Index, funcCaptures[0].Length, DocStyle.Current.FuncColor);
                for (int i = 1; i < funcCaptures.Count; i++)
                {
                    offset += action(stringBuilder, offset + funcCaptures[i].Index, funcCaptures[i].Length, DocStyle.Current.TypeColor);
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
                        offset += action(stringBuilder, offset + prefixCaptures[index].Index, prefixCaptures[index].Length, DocStyle.Current.PrefixColor);
                        index++;
                        if (index >= prefixCaptures.Count)
                            hasPrefix = false;
                    }
                    argsIndex = argsCaptures[i].Index;
                    while (j < typeCaptures.Count && typeCaptures[j].Index < argsIndex)
                    {
                        offset += action(stringBuilder, offset + typeCaptures[j].Index, typeCaptures[j].Length, DocStyle.Current.TypeColor);
                        j++;
                    }
                    offset += action(stringBuilder, offset + argsCaptures[i].Index, argsCaptures[i].Length, DocStyle.Current.ArgsColor);
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
            /*
            matches = Regex.Matches(stringBuilder.ToString(), pattern1);
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Groups["var"].Captures)
                    Debug.Log(capture.Value);
            }*/
            matches = Regex.Matches(stringBuilder.ToString(), CommentRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 0, DocStyle.Current.CommentsColor, ParseType.Single, "Comment");
            }
            matches = Regex.Matches(stringBuilder.ToString(), StringRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 0, DocStyle.Current.StringColor, ParseType.Single, "String");
            }
            matches = Regex.Matches(stringBuilder.ToString(), FieldRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
                addTable(table, match, 2, DocStyle.Current.TypeColor, ParseType.Capture, "Type");
                addTable(table, match, 3, DocStyle.Current.ArgsColor, ParseType.Single, "Arg");
                addTable(table, match, 4, DocStyle.Current.ArgsColor, ParseType.Capture, "Arg");
                args.Append(match.Groups[3].Value);
                args.Append("|");
                foreach (Capture capture in match.Groups[4].Captures)
                {
                    args.Append(capture.Value);
                    args.Append("|");
                }
            }
            matches = Regex.Matches(stringBuilder.ToString(), ReservedWordRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 0, DocStyle.Current.PrefixColor, ParseType.Single, "ReserveWord");
            }
            matches = Regex.Matches(stringBuilder.ToString(), NumberRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.NumberColor, ParseType.Single, "Number");
            }
            matches = Regex.Matches(stringBuilder.ToString(), FuncRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
                addTable(table, match, 3, DocStyle.Current.TypeColor, ParseType.Capture, "Type");
                addTable(table, match, 4, DocStyle.Current.TypeColor, ParseType.Func, "Type");
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
                        matchData = new MatchData(typeCaptures[j].Length, typeCaptures[j].Value, "Type", DocStyle.Current.TypeColor);
                        checkAndAdd(table, typeCaptures[j].Index, matchData);
                        j++;
                    }
                    matchData = new MatchData(argsCaptures[i].Length, argsCaptures[i].Value, "Arg", DocStyle.Current.ArgsColor);
                    checkAndAdd(table, argsCaptures[i].Index, matchData);
                    args.Append(matchData.Value);
                    args.Append("|");
                }
            }
            matches = Regex.Matches(stringBuilder.ToString(), ClassRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.PrefixColor, ParseType.Capture, "Prefix");
                addTable(table, match, 2, DocStyle.Current.PrefixColor, ParseType.Single, "Prefix");
                addTable(table, match, 3, DocStyle.Current.TypeColor, ParseType.Capture, "Type");
            }
            matches = Regex.Matches(stringBuilder.ToString(), ControlReservedWordRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 0, DocStyle.Current.ControlColor, ParseType.Single, "Control");
            }
            matches = Regex.Matches(stringBuilder.ToString(), NewConstructRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.TypeColor, ParseType.Capture, "Type");
            }
            matches = Regex.Matches(stringBuilder.ToString(), MethodRegex);
            foreach (Match match in matches)
            {
                addTable(table, match, 1, DocStyle.Current.TypeColor, ParseType.Func, "Type");
            }
            if (args.ToString() != "")
            {
                matches = Regex.Matches(stringBuilder.ToString(), @"(?<!\.)\b(" + args.ToString().Substring(0, args.Length - 1) + @")\b");
                foreach (Match match in matches)
                {
                    addTable(table, match, 1, DocStyle.Current.ArgsColor, ParseType.Single, "Arg");
                }
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
                    matchData = new MatchData(captures[0].Length, captures[0].Value, "Func", DocStyle.Current.FuncColor);
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
                MatchCollection matches = Regex.Matches(syntax, FuncRegex);
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

        public static int LevenshteinDistance(this string lhs, string rhs)
        {
            int[,] dp = new int[lhs.Length+1, rhs.Length+1];
            dp[0,0] = 0;
            for (int i = 0; i < lhs.Length; i++)
                dp[i + 1, 0] = i + 1;
            for (int i = 0; i < rhs.Length; i++)
                dp[0, i + 1] = i + 1;
            for(int y=0; y < lhs.Length; y++)
            {
                for(int x=0; x < rhs.Length; x++)
                {
                    var val = Math.Min(Math.Min(dp[y, x], dp[y + 1, x]), dp[y, x + 1]);
                    if (lhs[y] != rhs[x]) val++;
                    dp[y + 1, x + 1] = val;
                }
            }
            return dp[lhs.Length, rhs.Length];
        }
    }
}