using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement(string htmlElement, HtmlElement parent)
        {
            Name = htmlElement.Split()[0];
            List<Match> matches = new Regex("([^\\s]*?=\"(.*?)\")").Matches(htmlElement).ToList();
            Attributes = matches.Select(m => m.Value).ToList();
            Classes = Attributes.Where(a => a.StartsWith("class=")).Select(a => a.Split('=')[1].Replace("\"", "").Split(' ')).SelectMany(a => a).ToList();
            Id = Attributes.FirstOrDefault(a => a.StartsWith("id="))?.Split('=')[1].Replace("\"", "");
            Children = new List<HtmlElement>();
            Parent = parent;
            InnerHtml = "";
        }
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> que = new Queue<HtmlElement>();
            que.Enqueue(this);

            while (que.Count > 0)
            {
                HtmlElement current = que.Dequeue();
                yield return current;

                foreach (HtmlElement child in current.Children)
                    que.Enqueue(child);
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current.Parent != null)
            {
                yield return current.Parent;
                current = current.Parent;
            }
        }
        public IEnumerable<HtmlElement> FindBySelector(Selector selector)
        {
            var results = new HashSet<HtmlElement>(); 
            FindBySelectorRecursive(this, selector, results);
            return results;
        }

        private void FindBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            var descendants = element.Descendants();

            var descendantsLst = new List<HtmlElement>();
            foreach (var descendant in descendants)
                descendantsLst.Add(descendant);

            var filtered = descendantsLst.Where(e =>
                (string.IsNullOrEmpty(selector.TagName) || e.Name == selector.TagName) &&
                (string.IsNullOrEmpty(selector.Id) || e.Id == selector.Id) &&
                (!selector.Classes.Any() || selector.Classes.All(c => e.Classes.Contains(c)))
            );

            if (selector.Child == null)
            {
                foreach (var match in filtered)
                {
                    results.Add(match); 
                }
            }
            else
            {
                foreach (var item in filtered)
                {
                    FindBySelectorRecursive(item, selector.Child, results);
                }
            }
        }
    }
}
