﻿using OrthoHelper.Domain.Features.TextCorrection.Ports;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{
    public class OrthoEngineAdapter : ITextProcessingEngine
    {
        private readonly IOrthoEngine _orthoEngine;

        public OrthoEngineAdapter(IOrthoEngine orthoEngine)
        {
            _orthoEngine = orthoEngine;
        }

        public void SetModelName(string modelName)
        {
            _orthoEngine.SetModelName(modelName);
        }

        public async Task<string> ProcessTextAsync(string inputText)
        {
            return await _orthoEngine.ProcessTextAsync(inputText);
        }

        public string GetModelName()
        {
           return _orthoEngine.GetModelName();
        }

        //public async Task InitializeUserSession(string username)
        //{
        //    await _orthoEngine.InitializeUserSession(username);
        //}
    }
}
