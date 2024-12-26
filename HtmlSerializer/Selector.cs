using HtmlSerializer;
using System;
using System.Collections.Generic;
using System.Linq;

public class Selector
{
    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    public Selector() { }

    public static Selector ParseSelectorQuery(string query)
    {
        var queryParts = query.Split(' '); 
        Selector rootSelector = null;
        Selector currentSelector = null;

        foreach (var part in queryParts)
        {
            var newSelector = new Selector();

            string tagPart = part;

            // Search for Id
            if (part.Contains('#'))
            {
                var idIndex = part.IndexOf('#');
                var idEndIndex = part.IndexOf('.', idIndex + 1); // Search for where classes start
                if (idEndIndex == -1) idEndIndex = part.Length; // If no classes exist, use the full length of the string
                newSelector.Id = part.Substring(idIndex + 1, idEndIndex - idIndex - 1);
                tagPart = part.Substring(0, idIndex); // The part before the Id is the Tag name
            }

            // Search for Tag
            if (!string.IsNullOrEmpty(tagPart) && HtmlHelper.Instance.HtmlTags.Contains(tagPart))
                newSelector.TagName = tagPart;

            // Search for Classes
            if (part.Contains('.'))
            {
                var classIndex = part.IndexOf('.');
                var classPart = part.Substring(classIndex + 1); // Everything after the dot (.)
                newSelector.Classes.AddRange(classPart.Split('.')); // Split the classes by dots
            }

            // Build the selector tree
            if (rootSelector == null)
            {
                rootSelector = newSelector;
                currentSelector = rootSelector;
            }
            else
            {
                currentSelector.Child = newSelector;
                newSelector.Parent = currentSelector;
                currentSelector = newSelector;
            }
        }

        return rootSelector;
    }
}
