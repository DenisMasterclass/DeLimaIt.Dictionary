using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeLimaIt.Dictionary.Application.Features.Configuration.Repository.Entities
{
    public class ParameterFilter
    {
        public ParameterFilter()
        {
            
        }

        public ParameterFilter(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }
}
