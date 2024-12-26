using HtmlSerializer;

var serializer = new HtmlSerializer.HtmlSerializer();
string html = await serializer.Load("https://www.w3schools.com/js/");
serializer.ProcessHtml(html);

//PrintTree(serializer.Head);

string query = "a#navbtn_certified.tnb-nav-btn.w3-bar-item .fa.fa-caret-down";
Selector root = Selector.ParseSelectorQuery(query);

PrintSelectorHierarchy(root);

serializer.Head.FindBySelector(root).ToList().ForEach(x =>
{
    Console.WriteLine($"name: {(x.Name ?? "null")}, parent: {(x.Parent?.Name ?? "null")}");
});

Console.ReadLine();


//auxiliary functions
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

