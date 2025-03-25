using OrthoHelper.Application.Features.TextCorrection.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoHelper.Application.Tests.Features.TextCorrection.UseCases
{
    public interface ICorrectTextUseCase
    {
        Task<CorrectTextOutputDto> ExecuteAsync(CorrectTextInputDto input);
    }

}
