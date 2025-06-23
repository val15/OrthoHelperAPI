using OrthoHelper.Application.Features.TextTranslation.DTOs;
using OrthoHelper.Domain.Features.TextCorrection.Ports;
using OrthoHelper.Domain.Features.TextCorrection.ValueObjects;
using OrthoHelper.Domain.Features.TextProcess;
using OrthoHelper.Domain.Features.TextTranslation.Entities;
using OrthoHelper.Domain.Features.TextTranslation.Ports;
using OrthoHelper.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.TextTranslation
{
    public class TranslateHtmlFileUseCase : ITranslateHtmlFileUseCase
    {
        private readonly IHtmlParser _htmlParser;
        private readonly ITextProcessingEngine _textProcessingEngine;

        public TranslateHtmlFileUseCase(IHtmlParser htmlParser, ITextProcessingEngine textProcessingEngine)
        {
            _htmlParser = htmlParser;
            _textProcessingEngine = textProcessingEngine;
        }

        public async Task<HtmlTranslationOutputDto> ExecuteAsync(HtmlTranslationInputDto input)
        {
            // Créer une session de traduction
            var session = HtmlTranslationSession.Create(input.HtmlFilePath);
            _textProcessingEngine.SetModelName(input.ModelName, EngineType.Translator);
            // Extraire le texte traductible
            var translatableText = await _htmlParser.ExtractTranslatableTextAsync(input.HtmlFilePath);

            // Traduire le texte
           var translations = await TranslationHelper.TranslateTextsAsync(translatableText, _textProcessingEngine);

          
            // Remplacer le texte traduit dans le fichier HTML
            var translatedHtmlPath = await _htmlParser.ReplaceTranslatedTextAsync(input.HtmlFilePath, translations,_textProcessingEngine.GetModelName(EngineType.Translator));
            session.SetTranslatedHtmlPath(translatedHtmlPath);

            // Retourner le résultat
            return new HtmlTranslationOutputDto
            {
                OriginalHtmlPath = session.OriginalHtmlPath,
                TranslatedHtmlPath = session.TranslatedHtmlPath,
                CreatedAt = session.CreatedAt,
                ModelName = input.ModelName
            };
        }
    }
}
