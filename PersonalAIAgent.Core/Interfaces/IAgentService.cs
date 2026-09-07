using System.Threading.Tasks;
using PersonalAIAgent.Core.Models;

namespace PersonalAIAgent.Core.Interfaces
{
    public interface IAgentService
    {
        Task<AgentResponse> CallAgent(CommandRequest request);
    }
}
