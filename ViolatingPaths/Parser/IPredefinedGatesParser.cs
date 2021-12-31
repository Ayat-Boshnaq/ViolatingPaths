using System.Collections.Generic;
using System.IO;
using ViolatingPaths.Models;

namespace ViolatingPaths.Parser
{
    public interface IPredefinedGatesParser
    {
        Dictionary<string, Gate> Parse(StreamReader stream);
    }
}
