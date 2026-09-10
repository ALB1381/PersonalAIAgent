using System.Threading.Tasks;
using PersonalAIAgent.Core.Data;
using PersonalAIAgent.Core.Models;

namespace PersonalAIAgent.Core.Interfaces
{
    public interface IAgentService
    {
        Task<AgentResponse> CallAgent(CommandRequest request);
        Task AddAPIKey(string apiKey);
        Task EditAPIKey(string APIkeyString);
        Task DeleteAPIKey();
        Task<API?> GetApiKey();
    }
}
