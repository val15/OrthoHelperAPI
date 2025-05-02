using Microsoft.Extensions.Logging;
using OrthoHelper.Domain.Features.TextCorrection.Entities;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;
using OrthoHelper.Infrastructure.Features.TextProcessing.Entities;
using System.Text.Json;

namespace OrthoHelper.Infrastructure.Features.TextProcessing.Repositories
{
    public class LLMModelRepository : ILLMModelRepository
    {
        private readonly ILogger<LLMModelRepository> _logger;
        private readonly HttpClient _httpClient;

        public LLMModelRepository(HttpClient httpClient, ILogger<LLMModelRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger; // Assignez l'instance du logger
        }
        public async Task<IEnumerable<LLMModel>> GetAvailableLLMModelsAsync()
        {

            try
            {

                var llmModelList = new List<LLMModel>();
                //Add other, ex : Online:gemini-2.0-flash
                llmModelList.Add(new LLMModel($"Online:gemini-2.0-flash"));

                var OllamaModels = await GetInstalledOllamaModelsAsync();

                if (OllamaModels?.models != null && OllamaModels.models.Length > 0)
                {
                    _logger.LogInformation("Liste des modèles Ollama installés :");
                    foreach (var modelInfo in OllamaModels.models)
                    {
                             
                            llmModelList.Add(new LLMModel($"Ollama:{modelInfo.name}"));
                        
                    }
                }



                return llmModelList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche des Models disponibles");

                return null;
            }

            //IEnumerable<LLMModel> llmModelList = new List<LLMModel>
            //{
            //    new LLMModel("Ollama:Gemma")
            //    ,
            //    new LLMModel("Online:gemini-2.0-flash")

            //};
            //return Task.FromResult(llmModelList);


        }


        public async Task<OllamaModelsResponse> GetInstalledOllamaModelsAsync()
        {

            try
            {
                _logger.LogInformation("Tentative de récupération des modèles installés sur Ollama");
                _logger.LogInformation($"URL de la requête : {_httpClient.BaseAddress}/api/tags");
                HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/api/tags");
                response.EnsureSuccessStatusCode(); // Throw exception if not successful

                string jsonString = await response.Content.ReadAsStringAsync();
                var modelsResponse = JsonSerializer.Deserialize<OllamaModelsResponse>(jsonString);

                return modelsResponse;
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Erreur de requête HTTP : {ex.Message}");
                _logger.LogError(ex, $"Erreur de requête HTTP : {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {

                _logger.LogError(ex, $"Erreur de désérialisation JSON : {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Une erreur inattendue s'est produite : {ex.Message}");
                return null;
            }
        }

    }
}
