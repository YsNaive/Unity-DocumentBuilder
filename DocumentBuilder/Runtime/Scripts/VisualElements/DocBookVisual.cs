using NaiveAPI_UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocBookVisual : VisualElement
    {
        DocPageMenu pageMenu;
        public DocPageVisual DisplayingPage;
        public bool DontPlayAnimation = false;
        public bool IsPlayingAinmation { get; private set; }
        public VisualElement ChapterMenu;
        VisualElement leftContainer, rightContainer;
        SplitView splitView;
        public DocBookVisual(SODocPage rootPage)
        {
            style.flexGrow = 1;
            splitView = new(30);
            leftContainer = new();
            rightContainer = new();
            splitView.Add(leftContainer);
            splitView.Add(rightContainer);

            style.backgroundColor = DocStyle.Current.BackgroundColor;

            pageMenu = new DocPageMenu(rootPage);
            pageMenu.EnableAutoHierarchySave = true;
            pageMenu.OnSelected += newVal =>
            {
                var newPage = new DocPageVisual(newVal.TargetPage);
                newPage.style.SetIS_Style(ISMargin.None);
                newPage.style.position = Position.Relative;
                if (DontPlayAnimation)
                {
                    if (DisplayingPage != null)
                        rightContainer.Remove(DisplayingPage);
                    DisplayingPage = newPage;
                    rightContainer.Add(DisplayingPage);
                    repaintChapter();
                }
                else if (DisplayingPage != null)
                {
                    IsPlayingAinmation = true;
                    pageMenu.LockSelect = true;
                }
                else
                {
                    DisplayingPage = newPage;
                    rightContainer.Add(DisplayingPage);
                    IsPlayingAinmation = true;
                    pageMenu.LockSelect = true;
                }
            };
            pageMenu.LoadStateHierarchy();
            leftContainer.Add(pageMenu);
            Add(splitView);
        }
        void repaintChapter()
        {
            var newChapter = createChapterMenu();

            if (newChapter == null && ChapterMenu == null) return;
            if(ChapterMenu != null && newChapter != null)
            {
                Remove(ChapterMenu);
                ChapterMenu = newChapter;
                Add(ChapterMenu);
            }
            else if(ChapterMenu == null)
            {
                ChapterMenu = newChapter;
                ChapterMenu.Fade(0, 1, 250, 20, null);
                Add(ChapterMenu);
            }
            else
            {
                ChapterMenu.Fade(1,0, 250, 20, () =>
                {
                    Remove(ChapterMenu);
                    ChapterMenu = null;
                });
            }
        }
        VisualElement createChapterMenu()
        {
            var ve = new VisualElement();
            ve.style.position = Position.Absolute;
            ve.style.alignItems = Align.FlexStart;
            ve.style.right = Length.Percent(3.5f);
            ve.style.top = Length.Percent(1f);
            int comIndex = 0;
            var chapInfo = new VisualElement();
            foreach (var com in DisplayingPage.Target.Components)
            {
                if (com.VisualID == "1")
                {
                    if (com.TextData.Count != 0)
                    {
                        var text = (new DSTextElement("·  " + com.TextData[0]));
                        chapInfo.Add(text);
                        DocLabel.Data data = JsonUtility.FromJson<DocLabel.Data>(com.JsonData);
                        data ??= new DocLabel.Data();

                        text.style.marginLeft = (data.Level-1) * DocStyle.Current.MainTextSize;
                        int localI = comIndex;
                        text.RegisterCallback<PointerDownEvent>(e =>
                        {
                            DisplayingPage.ScrollTo(DisplayingPage[localI]);
                            DisplayingPage[localI].Highlight(50, DocStyle.Current.SuccessTextColor);
                        });
                    }
                }
                comIndex++;
            }
            if (chapInfo.childCount == 0) return null;
            var color = DocStyle.Current.BackgroundColor * 0.75f;
            color.a = 0.85f;
            chapInfo.style.backgroundColor = color;
            chapInfo.style.SetIS_Style(ISPadding.Percent(15));
            chapInfo.style.SetIS_Style(ISRadius.Percent(10));
            bool isOpen = false;
            var btn = new DSTextElement("=");
            btn.RegisterCallback<PointerEnterEvent>(e =>
            {
                if (!isOpen)
                {
                    isOpen = true;
                    chapInfo.Fade(1, 300,10,null);
                    chapInfo.style.display = DisplayStyle.Flex;
                }
            });
            chapInfo.RegisterCallback<PointerLeaveEvent>((e) =>
            {
                if (isOpen)
                {
                    isOpen = false;
                    chapInfo.Fade(0, 100, 10, () => { chapInfo.style.display = DisplayStyle.None; });
                }
            });
            btn.style.fontSize = btn.style.fontSize.value.value * 1.5f;
            btn.style.position = Position.Absolute;
            btn.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;
            btn.style.right = 10;
            chapInfo.style.display = DisplayStyle.None;
            ve.Add(btn);
            ve.Add(chapInfo);
            return ve;
        }
    }
}
