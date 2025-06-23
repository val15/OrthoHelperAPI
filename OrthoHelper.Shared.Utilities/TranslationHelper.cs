using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextProcess;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace OrthoHelper.Shared.Utilities
{
    public static class TranslationHelper
    {
        /// <summary>
        /// Traduit une liste de textes en parallèle en utilisant un moteur de traitement de texte.
        /// </summary>
        /// <param name="texts">Liste des textes à traduire.</param>
        /// <param name="textProcessingEngine">Moteur de traitement de texte à utiliser pour la traduction.</param>
        /// <returns>Dictionnaire contenant les textes originaux et leurs traductions.</returns>
        public static async Task<Dictionary<string, string>> TranslateTextsInParallelAsync(
            string translatableText,
            ITextProcessingEngine textProcessingEngine)
        {

            var texts = translatableText.Split('\n');

            var translations = new ConcurrentDictionary<string, string>();
            var totalCount = texts.Count();
            var currentCount = 0;

            await Parallel.ForEachAsync(texts, async (text, cancellationToken) =>
            {

           
                var translatedText = await textProcessingEngine.ProcessTextAsync(text, EngineType.Translator);
                translations[text] = translatedText;

                // Log ou debug pour suivre la progression
                Interlocked.Increment(ref currentCount);
                Debug.WriteLine($"Translated '{text}' to '{translatedText}'");
                Debug.WriteLine($"{currentCount}/{totalCount}");
            });

            return new Dictionary<string, string>(translations);
        }


        public static async Task<Dictionary<string, string>> TranslateTextsAsync(
              string translatableText,
              ITextProcessingEngine textProcessingEngine)
        {
            var texts = translatableText.Split('\n');

            var translations = new ConcurrentDictionary<string, string>();
            var totalCount = texts.Count();
            var currentCount = 0;

            
            foreach (var text in texts)
            {


                var translatedText = await textProcessingEngine.ProcessTextAsync(text, EngineType.Translator);
                translations[text] = translatedText;

                // Log ou debug pour suivre la progression
             //   Debug.C
                Interlocked.Increment(ref currentCount);
                Debug.WriteLine($"=> '{text}' \n=> '{translatedText}'");
                Debug.WriteLine($"{currentCount}/{totalCount}");
            }

            return new Dictionary<string, string>(translations);
        }


    }
}
