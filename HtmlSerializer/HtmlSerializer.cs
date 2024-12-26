using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlSerializer
    {
        string[] htmlTags = HtmlHelper.Instance.HtmlTags;
        string[] htmlVoidTags = HtmlHelper.Instance.HtmlVoidTags;
        public HtmlElement Head { get; set; }
        public async Task ProcessHtml(string html)
        {
            string cleanHtml = new Regex("\\s+").Replace(html, " ");
            List<string> htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            Head = new HtmlElement(htmlLines[0], null);
            CreateHtmlTree(htmlLines, Head, htmlTags, htmlVoidTags);
        }

        public async Task<string> Load(string url)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }

        public void CreateHtmlTree(List<string> htmlLines, HtmlElement head, string[] htmlTags, string[] htmlVoidTags)
        {
            int i = 1;
            HtmlElement current = head, child;

            while (i < htmlLines.Count)
            {
                var firstArg = htmlLines[i].Split()[0];

                if (firstArg == "/html")
                    break;

                if (htmlTags.Contains(firstArg))
                {
                    child = new HtmlElement(htmlLines[i], current);
                    current.Children.Add(child);

                    if (!htmlVoidTags.Contains(firstArg))
                        current = child;
                }
                else if (firstArg.StartsWith('/'))
                {
                    if (current.Parent != null)
                        current = current.Parent;
                }
                else
                {
                    current.InnerHtml += htmlLines[i];
                }

                i++;
            }
        }
        
    }

}
