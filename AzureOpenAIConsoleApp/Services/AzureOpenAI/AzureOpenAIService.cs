using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;
using AzureOpenAIConsoleApp.Models;

namespace AzureOpenAIConsoleApp.Services.AzureOpenAI
{
    public class AzureOpenAIService : IAzureOpenAIService
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly AzureOpenAIClient _azureClient;

        public AzureOpenAIService(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;

            var endpoint = "replace-with-your-open-ai-service-url";
            //var endpoint = _configurationRoot[Configurations.AzureOpenAIUrl];

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException("Azure OpenAI endpoint is missing in configuration.");

            // Use Managed Identity instead of API key
            //_azureClient = new AzureOpenAIClient(new Uri(endpoint), new DefaultAzureCredential());
            
            // Use API key for authentication
            var key = "replace-with-your-open-ai-service-key";
            AzureKeyCredential credential = new AzureKeyCredential(key);
            _azureClient = new AzureOpenAIClient(new Uri(endpoint), credential);
        }

        public async Task<OpenAIResp> GetResponseAsync(string prompMsg)
        {
            //var model = _configurationRoot[Configurations.AzureOpenAIModel];
            var model = "replace-with-your-model-name";

            if (string.IsNullOrEmpty(model))
            {
                return new OpenAIResp()
                {
                    IsSuccess = false,
                    RespMsg = "Azure OpenAI mmodel name is missing in configuration."
                };
            }
            if (string.IsNullOrWhiteSpace(prompMsg))
            {
                return new OpenAIResp()
                {
                    IsSuccess = false,
                    RespMsg = "The prompt message cannot be null or empty."
                };
            }
            if (prompMsg.Length > 2000)
            {
                return new OpenAIResp()
                {
                    IsSuccess = false,
                    RespMsg = "The prompt message exceeds the maximum allowed length of 2000 characters."
                };
            }

            // Initialize the ChatClient with the specified deployment name
            ChatClient chatClient = _azureClient.GetChatClient(model);

            // Create a chat message based on the prompt message
            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompMsg)
            };

            // Create chat completion options
            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.7,
                MaxOutputTokenCount = 800,
                TopP = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0
            };

            try
            {
                // Create the chat completion request
                ChatCompletion completion = await chatClient.CompleteChatAsync(messages, options).ConfigureAwait(false);

                // Deserialize the response and extract the Content
                if (completion != null)
                {
                    var content = completion.Content?.FirstOrDefault()?.Text;
                    if (!string.IsNullOrEmpty(content))
                    {
                        return new OpenAIResp()
                        {
                            IsSuccess = true,
                            RespMsg = content
                        };
                    }
                    else
                    {
                        return new OpenAIResp()
                        {
                            IsSuccess = false,
                            RespMsg = "No content found in the response."
                        };
                    }
                }
                else
                {
                    return new OpenAIResp()
                    {
                        IsSuccess = false,
                        RespMsg = "No response received."
                    };
                }
            }
            catch (Exception ex)
            {
                return new OpenAIResp()
                {
                    IsSuccess = false,
                    RespMsg = "An error occurred."
                };
            }
        }
    }
}
