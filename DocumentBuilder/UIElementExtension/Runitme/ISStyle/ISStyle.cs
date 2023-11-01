using UnityEngine;
namespace NaiveAPI_UI
{
    public enum ISStyleFlag
    {
        None = 0,
        Editable = 1,
        Display = 2,
        Position = 4,
        Flex = 8,
        Align = 16,
        Size = 32,
        Margin = 64,
        Padding = 128,
        Text = 256,
        Background = 512,
        Border = 1024,
        Radius = 2048,

        MarginPadding = 192,
    }
    /// <summary>
    /// Include all IS component
    /// </summary>
    [System.Serializable]
    public class ISStyle : ISerializationCallbackReceiver
    {
        public ISStyle() : this(4095) { }
        public ISStyle(ISStyleFlag flag) : this((int)flag) { }
        public ISStyle(int enableMask)
        {
            EnableMask = enableMask;
            initByMask();
        }

        public int EnableMask;
        [SerializeReference] public ISDisplay    Display;
        [SerializeReference] public ISPosition   Position;
        [SerializeReference] public ISFlex       Flex;
        [SerializeReference] public ISAlign      Align;
        [SerializeReference] public ISSize       Size;
        [SerializeReference] public ISMargin     Margin;
        [SerializeReference] public ISPadding    Padding;
        [SerializeReference] public ISText       Text;
        [SerializeReference] public ISBackground Background;
        [SerializeReference] public ISBorder     Border;
        [SerializeReference] public ISRadius     Radius;

        public ISStyle Copy()
        {
            return new ISStyle
            {
                EnableMask = EnableMask,
                Display = (Display!=null)? Display.Copy(): null,
                Position = (Position != null) ? Position.Copy() : null,
                Flex = (Flex != null) ? Flex.Copy() : null,
                Align = (Align != null) ? Align.Copy() : null,
                Size = (Size != null) ? Size.Copy() : null,
                Margin = (Margin != null) ? Margin.Copy() : null,
                Padding = (Padding != null) ? Padding.Copy() : null,
                Text = (Text != null) ? Text.Copy() : null,
                Background = (Background != null) ? Background.Copy() : null,
                Border = (Border != null) ? Border.Copy() : null,
                Radius = (Radius != null) ? Radius.Copy() : null,
            };
        }
        public bool IsEnable(ISStyleFlag type)
        {
            return (EnableMask & (int)type) == (int)type;
        }
        public void SetEnable(ISStyleFlag type, bool isEnable)
        {
            if (isEnable) 
                EnableMask |= (int)type;
            else
                EnableMask &= (int)type;
        }

        private void initByMask()
        {
            if (IsEnable(ISStyleFlag.Display)) Display = new ISDisplay();
            else Display = null;

            if (IsEnable(ISStyleFlag.Position)) Position = new ISPosition();
            else Position = null;

            if (IsEnable(ISStyleFlag.Flex)) Flex = new ISFlex();
            else Flex = null;

            if (IsEnable(ISStyleFlag.Align)) Align = new ISAlign();
            else Align = null;

            if (IsEnable(ISStyleFlag.Size)) Size = new ISSize();
            else Size = null;

            if (IsEnable(ISStyleFlag.Margin)) Margin = new ISMargin();
            else Margin = null;

            if (IsEnable(ISStyleFlag.Padding)) Padding = new ISPadding();
            else Padding = null;

            if (IsEnable(ISStyleFlag.Text)) Text = new ISText();
            else Text = null;

            if (IsEnable(ISStyleFlag.Background)) Background = new ISBackground();
            else Background = null;

            if (IsEnable(ISStyleFlag.Border)) Border = new ISBorder();
            else Border = null;

            if (IsEnable(ISStyleFlag.Radius)) Radius = new ISRadius();
            else Radius = null;
        }

        public void ApplyMask()
        {
            if (!IsEnable(ISStyleFlag.Display))
                Display = null;
            else Display ??= new();
            if (!IsEnable(ISStyleFlag.Position))
                Position = null;
            else Position ??= new();
            if (!IsEnable(ISStyleFlag.Flex))
                Flex = null;
            else Flex ??= new();
            if (!IsEnable(ISStyleFlag.Align))
                Align = null;
            else Align ??= new();
            if (!IsEnable(ISStyleFlag.Size))
                Size = null;
            else Size ??= new();
            if (!IsEnable(ISStyleFlag.Margin))
                Margin = null;
            else Margin ??= new();
            if (!IsEnable(ISStyleFlag.Padding))
                Padding = null;
            else Padding ??= new();
            if (!IsEnable(ISStyleFlag.Text))
                Text = null;
            else Text ??= new();
            if (!IsEnable(ISStyleFlag.Background))
                Background = null;
            else Background ??= new();
            if (!IsEnable(ISStyleFlag.Border))
                Border = null;
            else Border ??= new();
            if (!IsEnable(ISStyleFlag.Radius))
                Radius = null;
            else Radius ??= new();
        }
        public void OnBeforeSerialize()
        {
            ApplyMask();
        }
        public void OnAfterDeserialize()
        {
        }
    }
}