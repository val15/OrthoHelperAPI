using OrthoHelper.Application.Features.TextTranslation.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Features.TextTranslation
{
    public interface ITranslateHtmlFileUseCase
    {
        Task<HtmlTranslationOutputDto> ExecuteAsync(HtmlTranslationInputDto input);
    }
}
