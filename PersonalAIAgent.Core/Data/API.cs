using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PersonalAIAgent.Core.Data
{
    public class API
    {
        [Key]
        public int APIId { get; set; }

        [MaxLength(300)]
        public string APIString { get; set; } = string.Empty;
    }
}