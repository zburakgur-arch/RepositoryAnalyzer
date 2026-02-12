using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.Entities
{
    public class Repository : IEntity<string>
    {
        public string Id { get; set; }  //url
        public string LocalPath { get; set; }
        public DateTime ClonedAt { get; set; }
    }
}
