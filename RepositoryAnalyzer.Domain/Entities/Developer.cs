using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.Entities
{
    public class Developer : IEntity<string>
    {
        public string Id { get; set; } // Name
    }
}
