using System;
using System.Collections.Generic;

namespace ViolatingPaths.Models
{
    public class Circuit : ICircuit
    {
        private Dictionary<string, List<string>> graph;
        public Circuit(
            List<Tuple<string, string>> edges,
            List<string> vertexes,
            List<string> regs,
            List<string> inputs,
            List<string> outputs)
        {
            Regs = regs;
            Inputs = inputs;
            Outputs = outputs;

            graph = new Dictionary<string, List<string>>();
            foreach (var ver in vertexes)
            {
                graph.Add(ver, new List<string>());
            }

            foreach (var edge in edges)
            {
                graph[edge.Item1].Add(edge.Item2);
            }
        }

        public List<string> Regs { get; }
        public List<string> Inputs { get; }
        public List<string> Outputs { get; }

        public void GetViolatingPaths(int threshold)
        {
            // Marwa is here
            throw new NotImplementedException();
        }
    }
}
