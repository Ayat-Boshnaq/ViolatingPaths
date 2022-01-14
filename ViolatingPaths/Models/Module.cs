using System;
using System.Collections.Generic;

namespace ViolatingPaths.Models
{
    public class Module
    {
        private List<string> inputs;
        private List<string> wires;
        private List<string> outputs;
        private List<Tuple<string, string>> edges;
        private List<string> regestries;

        public Module()
        {
            inputs = new List<string>();
            wires = new List<string>();
            outputs = new List<string>();
            Edges = new List<Tuple<string, string>>();
            regestries = new List<string>();
        }

        public string Name { get; set; }
        public List<Tuple<string, string>> Edges { get => edges; set => edges = value; }
        public List<string> Regestries { get => regestries; set => regestries = value; }

        public void AddInput(string input) => inputs.Add(input);

        public void AddWire(string input) => wires.Add(input);

        public void AddOutput(string output) => outputs.Add(output);

        public void AddRegestry(string reg) => regestries.Add(reg);

        public void AddEdge(string source, string destination) => Edges.Add(new Tuple<string, string>(source, destination));

        public List<string> GetInputs() => inputs;

        public List<string> GetWires() => wires;

        public List<string> GetOutputs() => outputs;
    }
}
