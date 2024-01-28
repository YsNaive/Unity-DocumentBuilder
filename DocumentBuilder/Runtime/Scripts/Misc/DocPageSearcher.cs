using System.Collections.Generic;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageSearcher
    {
        public bool MatchName = true;
        public bool MatchContent = true;
        public List<(SODocPage Page, List<DocComponent> MatchComponent)> FoundPages = new();
        public List<(SODocPage Page, List<DocComponent> MatchComponent)> Match(SODocPage root, string value)
        {
            FoundPages.Clear();
            foreach (var page in root.Pages())
            {
                (SODocPage Page, List<DocComponent> MatchComponent) find = (null, new());
                if (MatchName && page.name.Contains(value))
                    find.Page = page;

                if (MatchContent)
                {
                    foreach(var com in page.Components)
                    {
                        if (com.Contains(value))
                        {
                            find.Page = page;
                            find.MatchComponent.Add(com);
                        }
                    }
                }

                if(find.Page != null)
                    FoundPages.Add(find);
            }

            return FoundPages;
        }
    }
}
