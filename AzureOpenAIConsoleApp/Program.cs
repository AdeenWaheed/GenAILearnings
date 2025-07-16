using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace AzureOpenAIConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var endpoint = "replace-with-your-open-ai-service-url";
            var key = "replace-with-your-open-ai-service-key";
            var model = "replace-with-your-model-name";

            if (string.IsNullOrEmpty(endpoint))
            {
                Console.WriteLine("Please set the AZURE_OPENAI_ENDPOINT environment variable.");
                return;
            }
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine("Please set the AZURE_OPENAI_KEY environment variable.");
                return;
            }

            AzureKeyCredential credential = new AzureKeyCredential(key);

            // Initialize the AzureOpenAIClient
            AzureOpenAIClient azureClient = new(new Uri(endpoint), credential);

            // Initialize the ChatClient with the specified deployment name
            ChatClient chatClient = azureClient.GetChatClient(model);

            // Create a list of chat messages
            Console.WriteLine("You own AI chat model.....\n\n");
            Console.WriteLine("Ask me anything: ");
            string prompMsg = Console.ReadLine();
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
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0
            };

            try
            {
                // Create the chat completion request
                ChatCompletion completion = chatClient.CompleteChat(messages, options);

                // Print the response
                if (completion != null)
                {
                    Console.WriteLine("\n\nResponse received:");
                    // Deserialize the response and extract the Content
                    if (completion != null)
                    {
                        var content = completion.Content?.FirstOrDefault()?.Text;
                        if (!string.IsNullOrEmpty(content))
                        {
                            Console.WriteLine(content);
                        }
                        else
                        {
                            Console.WriteLine("No content found in the response.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No response received.");
                    }
                }
                else
                {
                    Console.WriteLine("No response received.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
