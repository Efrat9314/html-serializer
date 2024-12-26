using System;
using System.IO;
using System.Text.Json;

namespace HtmlSerializer
{
    public class HtmlHelper
    {
        public string[] HtmlTags { get; }
        public string[] HtmlVoidTags { get; }

        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

        private HtmlHelper()
        {
            try
            {
                HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("JSON Files/HtmlTags.json")) ?? Array.Empty<string>();
                HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("JSON Files/HtmlVoidTags.json")) ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading HTML tags: {ex.Message}");
                HtmlTags = Array.Empty<string>();
                HtmlVoidTags = Array.Empty<string>();
            }
        }
    }
}
