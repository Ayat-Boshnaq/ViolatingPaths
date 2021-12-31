using System.Collections.Generic;
using System.IO;
using ViolatingPaths.Models;

namespace ViolatingPaths.Parser
{
    public class PredefinedGatesParser : IPredefinedGatesParser
    {
        public Dictionary<string, Gate> Parse(StreamReader stream)
        {
            Dictionary<string, Gate> parsedGates = new Dictionary<string, Gate>();
            var text = stream.ReadToEnd();
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            text = text.Replace("\t", "");
            text = text.Replace(" ", "");
            var gates = text.Split(";");

            foreach (var gate in gates)
            {
                if (gate.Equals("")) continue;
                var sections = gate.Split(",");
                var type = sections[0];
                var inputs = sections[1].Split("$");
                var outputs = sections[2].Split("$");
                var isReg = sections[3] == "true";

                var parsedGate = new Gate(type, inputs, outputs, isReg);
                parsedGates.Add(type, parsedGate);
            }

            return parsedGates;
        }
    }
}
