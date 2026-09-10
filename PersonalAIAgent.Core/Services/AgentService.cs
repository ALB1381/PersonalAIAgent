using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using PersonalAIAgent.Core.Data;
using PersonalAIAgent.Core.Interfaces;
using PersonalAIAgent.Core.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PersonalAIAgent.Core.Services
{
    public class AgentService : IAgentService
    {

        PersonalAIAgent.Core.Data.Context _context; 
        public AgentService(PersonalAIAgent.Core.Data.Context context)
        {
            _context = context;
        
        }

        public async Task<AgentResponse> CallAgent(CommandRequest request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.RequestToDo))
            {
                return Failure("A command is required.");
            }

            try
            {
                var apiKeyobj = await GetApiKey();
                string apiKey = apiKeyobj.APIString;
                GenerativeModel? _aiModel;
                    var googleAI = new GoogleAI(apiKey: apiKey);
                    _aiModel = googleAI.GenerativeModel(model: Mscc.GenerativeAI.Types.Model.Gemini3Flash);
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

                var commandText = response.Text ?? string.Empty;
                cmd.StandardInput.WriteLine(commandText);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());

                return new AgentResponse
                {
                    ResponseText = commandText,
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

        public async Task AddAPIKey(string apiKey)
        {
           await _context.APIs.AddAsync(new PersonalAIAgent.Core.Data.API { APIString = apiKey });
           await _context.SaveChangesAsync();
        }

        public async Task EditAPIKey(string APIkeyString)
        {
            var Target = _context.APIs.FirstOrDefault();
            Target.APIString = APIkeyString;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAPIKey()
        {
            var Target = _context.APIs.FirstOrDefault();
            if (Target != null)
            {
                _context.APIs.Remove(Target);
                await _context.SaveChangesAsync();
            }
        }

    
        public async Task<API?> GetApiKey()
        {
            return await _context.APIs.FirstOrDefaultAsync();
        }
    }
}
