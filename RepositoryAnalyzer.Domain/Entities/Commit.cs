using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryAnalyzer.Domain.ValueObjects;

namespace RepositoryAnalyzer.Domain.Entities
{
    public class Commit : IEntity<string>
    {
        public string Id { get; set; } //Hash
        public Developer Author { get; set; }
        public DateTime Date { get; set; }
        public List<Change> Changes { get; set; }
    }
}
