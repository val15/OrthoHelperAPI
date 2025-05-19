using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.TextTranslation.DTOs
{
    public class HtmlTranslationInputDto
    {
        /// <summary>
        /// Chemin du fichier HTML à traduire.
        /// </summary>
        public string HtmlFilePath { get; set; }

        /// <summary>
        /// Nom du modèle de traduction à utiliser.
        /// </summary>
        public string ModelName { get; set; }
    }
}

