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
            MyMain(new string[] { "Examples/gates4.txt", "Examples/netlist5.v", "3" });
        }
        static void MyMain(string[] args)
        {
            if (args.Length != 3)
            {
                throw new Exception("wrong number of arguments!!!!");
            }
            var predefinedGatesPath = args[0];
            if (predefinedGatesPath == null) throw new Exception("No gates file");
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
