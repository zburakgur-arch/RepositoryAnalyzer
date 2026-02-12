using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryAnalyzer.Domain.ValueObjects
{
    public class Complexity : IValueObject
    {
        public int LineOfCodes { get; set; }
        public int WhiteSpace { get; set; }
        public int Cyclomatic { get; set; }
    }
}
