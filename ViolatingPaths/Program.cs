using System;
using System.IO;
using ViolatingPaths.Models;
using ViolatingPaths.Parser;

namespace ViolatingPaths
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception("wrong number of arguments!!!!");
            }
            var predefinedGatesPath = args[0];
            var netlistPath = args[1];
            var threshold = int.Parse(args[2]);

#if DEBUG
            var dirPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
#else
            var dirPath = Directory.GetCurrentDirectory();
#endif
            using (var preDefinedGatesStream = new StreamReader(Path.Combine(dirPath, predefinedGatesPath)))
            using (var netlistStream = new StreamReader(Path.Combine(dirPath, netlistPath)))
            {
                var preDefinedGatesParser = new PredefinedGatesParser();
                var preDefinedGates = preDefinedGatesParser.Parse(preDefinedGatesStream);

                var netlistParser = new VerilogParser(preDefinedGates);
                ICircuit circuit = netlistParser.Parse(netlistStream);
                circuit.GetViolatingPaths(threshold);
            }
        }
    }
}
