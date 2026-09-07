using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalAIAgent.Core.Models
{
    public class AgentResponse
    {
        public string? ResponseText { get; set; }

        /// <summary>
        /// 0 means success, any other value indicates an error or failure.
        /// </summary>
        public int ResponseStatusCode { get; set; }
    }
}
