using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryAnalyzer.Domain.ValueObjects;

namespace RepositoryAnalyzer.Domain.Entities
{
    public class Module :IEntity<string>
    {
        public string Id { get; set; } // File path
        private Complexity Complexity { get; set; } = new Complexity();

        public int GetLineOfCodes() => Complexity.LineOfCodes;
        public void SetLineOfCodes(int value) => Complexity.LineOfCodes = value;

        public int GetWhiteSpace() => Complexity.WhiteSpace;
        public void SetWhiteSpace(int value) => Complexity.WhiteSpace = value;

        public int GetCyclomatic() => Complexity.Cyclomatic;
        public void SetCyclomatic(int value) => Complexity.Cyclomatic = value;
    }
}
