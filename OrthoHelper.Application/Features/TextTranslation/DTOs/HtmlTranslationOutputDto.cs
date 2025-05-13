using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.TextTranslation.DTOs
{
    public class HtmlTranslationOutputDto
    {
        /// <summary>
        /// Chemin du fichier HTML original.
        /// </summary>
        public string OriginalHtmlPath { get; set; }

        /// <summary>
        /// Chemin du fichier HTML traduit.
        /// </summary>
        public string TranslatedHtmlPath { get; set; }

        /// <summary>
        /// Date et heure de création de la session de traduction.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

