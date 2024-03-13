using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Models
{
    public sealed class ConfigurationParameterGetOutput
    {
        public ConfigurationParameterGetOutput(int id, string name)
        {
            Id = id; 
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
