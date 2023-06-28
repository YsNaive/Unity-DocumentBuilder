using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NaiveAPI_UI
{
    [System.Serializable]
    public class RichTextBuilder
    {
        [SerializeField] private RichText richText;
        public List<Command> Commands = new List<Command>();

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(Command cmd in Commands)
            {
                switch (cmd.CommandType)
                {
                    case CommandType.Text:
                        stringBuilder.Append(cmd.Value);
                        break;
                    case CommandType.NewLine:
                        stringBuilder.Append("\r\n");
                        break;
                    case CommandType.BeginSize:
                        stringBuilder.Append($"<size={cmd.Value}>");
                        break;
                    case CommandType.BeginColor:
                        stringBuilder.Append($"<color={cmd.Value}>");
                        break;
                    case CommandType.BeginBold:
                        stringBuilder.Append("<b>");
                        break;
                    case CommandType.BeginItalics:
                        stringBuilder.Append("<i>");
                        break;
                    case CommandType.BeginUnderline:
                        stringBuilder.Append("<u>");
                        break;
                    case CommandType.BeginStrikethrough:
                        stringBuilder.Append("<s>");
                        break;
                    case CommandType.EndSize:
                        stringBuilder.Append("</size>");
                        break;
                    case CommandType.EndColor:
                        stringBuilder.Append("</color>");
                        break;
                    case CommandType.EndBold:
                        stringBuilder.Append("</b>");
                        break;
                    case CommandType.EndItalics:
                        stringBuilder.Append("</i>");
                        break;
                    case CommandType.EndUnderline:
                        stringBuilder.Append("</u>");
                        break;
                    case CommandType.EndStrikethrough:
                        stringBuilder.Append("</s>");
                        break;
                }
            }
            return stringBuilder.ToString();
        }
        public string ToHTML()
        {
            return ToString().Replace("<color=", "<font color=").Replace("</color>", "</font>");
        }

        #region command func
        public RichTextBuilder Append(string text)
        {
            Commands.Add(new Command { CommandType = CommandType.Text, Value = text });
            return this;
        }
        public RichTextBuilder NewLine()
        {
            Commands.Add(new Command { CommandType = CommandType.NewLine});
            return this;
        }
        public RichTextBuilder BeginSize(int size)
        {
            Commands.Add(new Command { CommandType = CommandType.BeginSize, Value = size.ToString()});
            return this;
        }
        public RichTextBuilder EndSize()
        {
            Commands.Add(new Command { CommandType = CommandType.EndSize});
            return this;
        }
        public RichTextBuilder BeginColor(Color color)
        {
            Commands.Add(new Command { CommandType = CommandType.BeginColor, Value = '#'+ColorUtility.ToHtmlStringRGBA(color) });
            return this;
        }
        public RichTextBuilder EndColor()
        {
            Commands.Add(new Command { CommandType = CommandType.EndColor});
            return this;
        }
        public RichTextBuilder BeginBold()
        {
            Commands.Add(new Command { CommandType = CommandType.BeginBold });
            return this;
        }
        public RichTextBuilder EndBold()
        {
            Commands.Add(new Command { CommandType = CommandType.EndBold });
            return this;
        }
        public RichTextBuilder BeginItalics()
        {
            Commands.Add(new Command { CommandType = CommandType.BeginItalics });
            return this;
        }
        public RichTextBuilder EndItalics()
        {
            Commands.Add(new Command { CommandType = CommandType.EndItalics });
            return this;
        }
        public RichTextBuilder BeginUnderline()
        {
            Commands.Add(new Command { CommandType = CommandType.BeginUnderline });
            return this;
        }
        public RichTextBuilder EndUnderline()
        {
            Commands.Add(new Command { CommandType = CommandType.EndUnderline });
            return this;
        }
        public RichTextBuilder BeginStrikethrough()
        {
            Commands.Add(new Command { CommandType = CommandType.BeginStrikethrough });
            return this;
        }
        public RichTextBuilder EndStrikethrough()
        {
            Commands.Add(new Command { CommandType = CommandType.EndStrikethrough });
            return this;
        }
        #endregion

        [System.Serializable]
        public class Command
        {
            public CommandType CommandType;
            public string Value = "";
        }
        public enum CommandType
        {
            Text,
            NewLine,
            BeginSize,
            BeginColor,
            BeginBold,
            BeginItalics,
            BeginUnderline,
            BeginStrikethrough,
            EndSize,
            EndColor,
            EndBold,
            EndItalics,
            EndUnderline,
            EndStrikethrough,
        }
    }
}
