using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.ValueObjects
{
    public class Change : IValueObject
    {
        public string FilePath { get; set; }
        public int AddedLines { get; set; }
        public int DeletedLines { get; set; }
    }
}
