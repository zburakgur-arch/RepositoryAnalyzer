using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryAnalyzer.Domain.ValueObjects;

namespace RepositoryAnalyzer.Domain.Entities
{
    public class File :IEntity<string>
    {
        public string Id { get; set; } // File path
        public Complexity Complexity { get; set; }
    }
}
