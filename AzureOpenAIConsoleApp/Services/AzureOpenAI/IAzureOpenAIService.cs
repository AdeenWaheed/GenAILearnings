using AzureOpenAIConsoleApp.Models;
using System.Threading.Tasks;

namespace AzureOpenAIConsoleApp.Services.AzureOpenAI
{
    public interface IAzureOpenAIService
    {
        Task<OpenAIResp> GetResponseAsync(string prompMsg);
    }
}