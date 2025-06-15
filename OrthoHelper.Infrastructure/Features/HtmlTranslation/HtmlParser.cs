using HtmlAgilityPack;
using OrthoHelper.Domain.Features.TextTranslation.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Infrastructure.Features.TextTranslation
{
    public class HtmlParser : IHtmlParser
    {
        public async Task<string> ExtractTranslatableTextAsync(string htmlFilePath)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(htmlFilePath);

            var translatableText = new List<string>();
            foreach (var node in htmlDoc.DocumentNode.SelectNodes("//text()[normalize-space()]"))
            {
                // Vérifier si le nœud parent est une balise <style> ou <script>
                if (node.Ancestors("style").Any() || node.Ancestors("script").Any())
                {
                    continue; // Ignorer les nœuds dans <style> ou <script>
                }
                translatableText.Add(node.InnerText.Trim());
            }

            return string.Join("\n", translatableText);
        }

        public async Task<string> ReplaceTranslatedTextAsync(string htmlFilePath, Dictionary<string, string> translations,string modelName,string translatedHtmlPath=null)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(htmlFilePath);

            foreach (var node in htmlDoc.DocumentNode.SelectNodes("//text()[normalize-space()]"))
            {
                // Vérifier si le nœud parent est une balise <style> ou <script>
                if (node.Ancestors("style").Any() || node.Ancestors("script").Any())
                {
                    continue; // Ne pas modifier les nœuds dans <style> ou <script>
                }
                var originalText = node.InnerText.Trim();
                if (translations.ContainsKey(originalText))
                {
                    node.InnerHtml = translations[originalText];
                }
            }

            var finalTranslatedHtmlPath = translatedHtmlPath;

            if(string.IsNullOrEmpty(finalTranslatedHtmlPath))
            {
                finalTranslatedHtmlPath = Path.Combine(Path.GetDirectoryName(htmlFilePath)!, $"{Path.GetFileName(htmlFilePath).Replace(".html","")}.{modelName.Replace(":", "")}.Translated.html");


            }

            htmlDoc.Save(finalTranslatedHtmlPath);

            return finalTranslatedHtmlPath;
        }
    }
}
