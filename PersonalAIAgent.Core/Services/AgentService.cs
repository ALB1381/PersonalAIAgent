using Mscc.GenerativeAI;
using PersonalAIAgent.Core.Interfaces;
using PersonalAIAgent.Core.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PersonalAIAgent.Core.Services
{
    public class AgentService : IAgentService
    {
        private readonly GenerativeModel? _aiModel;
        

        public AgentService()
        {
           
            //You should replace your api that you got from google ai studio
            var apiKey = "Your API Key";
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                var googleAI = new GoogleAI(apiKey: apiKey);
                _aiModel = googleAI.GenerativeModel(model: Mscc.GenerativeAI.Types.Model.Gemini3Flash);
            }
        }

        public async Task<AgentResponse> CallAgent(CommandRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RequestToDo))
            {
                return Failure("A command is required.");
            }

            try
            {
          
                if (_aiModel is null)
                {
                    return Failure("No tool matched the request and GEMINI_API_KEY is not configured.");
                }

                var response = await _aiModel.GenerateContent(
                    "This command is sending from a self made agent, it developed by C# on windows" +
                    " the result should put a command that will be run in CMD to do the task your output will be run in command prompt in windows, please dont add any other words and sentences and just give the output, if the name of application is wrong please make it correct" + request.RequestToDo).ConfigureAwait(false);
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(response.Text.ToString());
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                return new AgentResponse
                {
                    ResponseText = response.Text,
                    ResponseStatusCode = 0
                };
            }
            catch (Exception exception)
            {
                return Failure($"The agent could not process the request: {exception.Message}");
            }
        }

        private static AgentResponse Failure(string message) => new()
        {
            ResponseText = message,
            ResponseStatusCode = 1
        };
    }
}
