// See https://aka.ms/new-console-template for more information
using HtmlSerializer;
using System.Text.RegularExpressions;
using System.Xml.Linq;

Console.WriteLine("Program Begin");

var html = await Load("https://www.w3schools.com/js/");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
string[] htmlTags = HtmlHelper.Instance.HtmlTags;
string[] htmlVoidTags = HtmlHelper.Instance.HtmlVoidTags;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

HtmlElement head = new HtmlElement(htmlLines[0], null);
void CreateHtmlTree()
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

    void PrintTree(HtmlElement element, int level = 0)
    {
        string indent = new string(' ', level * 2);

        string parentName = element.Parent != null ? element.Parent.Name : "None";
        Console.WriteLine($"{indent}<Name: {element.Name}, Id: {element.Id}, Parent: {parentName}, Classes: {string.Join(", ", element.Classes ?? new List<string>())}>");

        if (!string.IsNullOrEmpty(element.InnerHtml))
        {
            Console.WriteLine($"{indent}  InnerHtml: {element.InnerHtml}");
        }

        foreach (var child in element.Children)
        {
            PrintTree(child, level + 1);
        }
    }
 void PrintSelectorHierarchy(Selector selector)
{
    while (selector != null)
    {
        Console.WriteLine($"TagName: {selector.TagName}, Id: {selector.Id}, Classes: {string.Join(", ", selector.Classes)}");
        selector = selector.Child;
    }
}

CreateHtmlTree();
//PrintTree(head);
//head.Descendants().ToList().ForEach(e =>
//{
//    Console.WriteLine($"name: {(e.Name ?? "null")},parent: {(e.Parent?.Name ?? "null")}");
//});

string query = "a#navbtn_certified.tnb-nav-btn.w3-bar-item .fa.fa-caret-down";
Selector root = Selector.ParseSelectorQuery(query);
PrintSelectorHierarchy(root);

head.FindBySelector(root).ToList().ForEach(x=> {
    Console.WriteLine($"name: {(x.Name ?? "null")},parent: {(x.Parent?.Name ?? "null")}");
});

Console.ReadLine();

