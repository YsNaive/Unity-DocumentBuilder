using UnityEngine;
namespace NaiveAPI_UI
{
    /// <summary>
    /// Include all IS component
    /// </summary>
    [System.Serializable]
    public class ISStyle : ISerializationCallbackReceiver
    {
        public ISStyle()
        {
            EnableMask = 4095;
        }
        /// <summary>
        /// If you don't know what it's means, check the docs first.
        /// </summary>
        public ISStyle(int enableMask)
        {
            EnableMask = enableMask;
            initByMask();
        }
        public ISStyle(ISType[] types)
        {
            EnableMask = 0;
            foreach (var type in types)
            {
                EnableMask += (int)type;
            }
            initByMask();
        }

        public int EnableMask;
        public ISDisplay    Display;
        public ISPosition   Position;
        public ISFlex       Flex;
        public ISAlign      Align;
        public ISSize       Size;
        public ISMargin     Margin;
        public ISPadding    Padding;
        public ISText       Text;
        public ISBackground Background;
        public ISBorder     Border;
        public ISRadius     Radius;

        public enum ISType
        {
            None = 0,
            EnableMask = 1,
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
            Radius = 2048
        }
        public bool IsEnable(int mask)
        {
            return (EnableMask & (int)mask) == (int)mask;
        }
        public bool IsEnable(ISType type)
        {
            return (EnableMask & (int)type) == (int)type;
        }
        public void SetEnable(ISType type, bool isEnable)
        {
            if (isEnable) 
                EnableMask |= (int)type;
            else
                EnableMask &= (int)type;
        }

        private void initByMask()
        {
            if (IsEnable(ISType.Display)) Display = new ISDisplay();
            else Display = null;

            if (IsEnable(ISType.Position)) Position = new ISPosition();
            else Position = null;

            if (IsEnable(ISType.Flex)) Flex = new ISFlex();
            else Flex = null;

            if (IsEnable(ISType.Align)) Align = new ISAlign();
            else Align = null;

            if (IsEnable(ISType.Size)) Size = new ISSize();
            else Size = null;

            if (IsEnable(ISType.Margin)) Margin = new ISMargin();
            else Margin = null;

            if (IsEnable(ISType.Padding)) Padding = new ISPadding();
            else Padding = null;

            if (IsEnable(ISType.Text)) Text = new ISText();
            else Text = null;

            if (IsEnable(ISType.Background)) Background = new ISBackground();
            else Background = null;

            if (IsEnable(ISType.Border)) Border = new ISBorder();
            else Border = null;

            if (IsEnable(ISType.Radius)) Radius = new ISRadius();
            else Radius = null;
        }
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            if (!IsEnable(ISType.Display))
                Display = null;
            if (!IsEnable(ISType.Position))
                Position = null;
            if (!IsEnable(ISType.Flex))
                Flex = null;
            if (!IsEnable(ISType.Align))
                Align = null;
            if (!IsEnable(ISType.Size))
                Size = null;
            if (!IsEnable(ISType.Margin))
                Margin = null;
            if (!IsEnable(ISType.Padding))
                Padding = null;
            if (!IsEnable(ISType.Text))
                Text = null;
            if (!IsEnable(ISType.Background))
                Background = null;
            if (!IsEnable(ISType.Border))
                Border = null;
            if (!IsEnable(ISType.Radius))
                Radius = null;
        }
    }
}