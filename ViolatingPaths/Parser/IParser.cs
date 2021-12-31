using System.Collections.Generic;
using System.IO;
using ViolatingPaths.Models;

namespace ViolatingPaths.Parser
{
    public interface IParser
    {
        public ICircuit Parse(StreamReader stream);
    }
}
