using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ViolatingPaths.Models;

namespace ViolatingPaths.Parser
{
    public class VerilogParser : IParser
    {
        private enum LinePattern
        {
            Input,
            Output,
            Node,
            Headline,
            Wire,
            Reg
        }

        private static readonly string osp = "[\\t\\n\\r ]*";
        private static readonly string sp = "[\\t\\n\\r ]+";
        private static readonly string delayPart = "((?:#\\(" + osp + "([0-9.]+)" + osp + "," + osp + "([0-9.]+)" + osp + "\\))?)";
        private static readonly Regex RegPattern = new Regex($"(reg|input reg|output reg){osp}(\\[\\d+\\:\\d+\\])?{sp}(\\w+)");
        private static readonly Regex InputPattern = new Regex($"(input|inout|input reg){osp}(\\[\\d+\\:\\d+\\])?{sp}(\\w+)");
        private static readonly Regex OutputPattern = new Regex($"(output|inout|output reg){osp}(\\[\\d+\\:\\d+\\])?{sp}(\\w+)");
        private static readonly Regex WirePattern = new Regex($"(wire){osp}(\\[\\d+\\:\\d+\\])?{sp}(\\w+)");
        private static readonly Regex GatePattern = new Regex($"([A-Za-z0-9]+){sp}{delayPart}{osp}([_0-9a-zA-z]+){osp}\\(.+\\)");
        private static readonly Regex HeadlinePattern = new Regex($"(module){sp}([_]*)([0-9a-zA-z]+)([_0-9a-zA-z]*){sp}\\(.+\\)");

        //****************************************
        private Dictionary<string, Module> parsedModules;
        private Dictionary<string, Gate> predefinedGates;
        private Dictionary<string, string> customModulesNamesType;
        private Dictionary<string, string> predefinedGatesNamesType;

        public VerilogParser(Dictionary<string, Gate> predefinedGates)
        {
            parsedModules = new Dictionary<string, Module>();
            customModulesNamesType = new Dictionary<string, string>();
            predefinedGatesNamesType = new Dictionary<string, string>();
            this.predefinedGates = predefinedGates;
        }

        public ICircuit Parse(StreamReader stream)
        {
            Module lastModule = null;
            var modules = GetModulesFromStream(stream);
            foreach(var module in modules)
            {
                var m = ParseModule(module);
                parsedModules.Add(m.Name, m);
                lastModule = m;
            }
            if (lastModule == null) throw new Exception("There're no modules in the file");
            return BuildCircuit(lastModule);
        }

        private ICircuit BuildCircuit(Module lastModule)
        {
            (var edges, var regs) = BuildCircuitEdges(lastModule);
            List<string> vertexes = edges.Select(t => new List<string> { t.Item1, t.Item2 }).SelectMany(l => l).Distinct().ToList();
            return new Circuit(edges, vertexes, regs, lastModule.GetInputs(), lastModule.GetOutputs());
        }

        private (List<Tuple<string, string>>, List<string>) BuildCircuitEdges(Module baseModule)
        {
            var edges = new List<Tuple<string, string>>();
            var regestires = baseModule.Regestries;
            var fetchedModulesNames = new HashSet<string>();
            var edgesToBeRemoved = new List<Tuple<string, string>>();

            foreach (var edge in baseModule.Edges)
            {
                var sourceModuleName = edge.Item1.Split("-_-")[0];
                var destinationModuleName = edge.Item2.Split("-_-")[0];
                var source = baseModule.Name + "-_-" + sourceModuleName;
                var destination = baseModule.Name + "-_-" + destinationModuleName;
                if (customModulesNamesType.ContainsKey(source))
                {
                    if (!fetchedModulesNames.Contains(source))
                    {
                        fetchedModulesNames.Add(source);
                        var module = parsedModules[customModulesNamesType[source]];
                        (var innerModuleEdges, var innerRegs) = BuildCircuitEdges(module);
                        var edgesWithPrefix = AddPrefixToEdges(innerModuleEdges, sourceModuleName);
                        edges = edges.Concat(edgesWithPrefix).ToList(); ;
                        var regestriesWithPrefix = AddPrefixToRegs(innerRegs, sourceModuleName);
                        regestires = regestires.Concat(regestriesWithPrefix).ToList();
                    }
                    var edgesToRemove = edges.Where(e => e.Item2 == edge.Item1).ToList();
                    var edgesToAdd = edgesToRemove.Select(e => new Tuple<string, string>(e.Item1, edge.Item2));
                    edgesToBeRemoved = edgesToBeRemoved.Concat(edgesToRemove).ToList();
                    edges = edges.Concat(edgesToAdd).ToList();
                }
                else if (customModulesNamesType.ContainsKey(destination))
                {
                    if (!fetchedModulesNames.Contains(destination))
                    {
                        fetchedModulesNames.Add(destination);
                        var module = parsedModules[customModulesNamesType[destination]];
                        (var innerModuleEdges, var innerRegs) = BuildCircuitEdges(module);
                        var edgesWithPrefix = AddPrefixToEdges(innerModuleEdges, destinationModuleName);
                        edges = edges.Concat(edgesWithPrefix).ToList();
                        var regestriesWithPrefix = AddPrefixToRegs(innerRegs, destinationModuleName);
                        regestires = regestires.Concat(regestriesWithPrefix).ToList();
                    }
                    var edgesToRemove = edges.Where(e => e.Item1 == edge.Item2).ToList();
                    var edgesToAdd = edgesToRemove.Select(e => new Tuple<string, string>(edge.Item1, e.Item2));
                    edgesToBeRemoved = edgesToBeRemoved.Concat(edgesToRemove).ToList();
                    edges = edges.Concat(edgesToAdd).ToList();

                } else
                {
                    edges.Add(edge);
                }
            }
            foreach (var e in edgesToBeRemoved)
            {
                edges.Remove(e);
            }
            return (edges, regestires);
        }

        private List<string> AddPrefixToRegs(List<string> regestries, string destinationModuleName)
        {
            return regestries
                .Select(reg => destinationModuleName + "-_-" + reg)
                .ToList();
        }

        private List<Tuple<string, string>> AddPrefixToEdges(List<Tuple<string, string>> innerModuleEdges, string name)
        {
            return innerModuleEdges
                .Select(tuple => new Tuple<string, string>(name + "-_-" + tuple.Item1, name + "-_-" + tuple.Item2))
                .ToList();
        }

        private Module ParseModule(string module)
        {
            Module m = new Module();
            var lines = module.Split(";");
            string[] subs;
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed == string.Empty) continue;
                LinePattern pattern = GetLinePattern(trimmed);
                switch (pattern)
                {
                    case LinePattern.Input:
                        subs = trimmed.Split(' ', '\t', '\r', '\n', ',').Where(t => t != string.Empty).ToArray();
                        HandleInputPattern(subs, m);
                        break;
                    case LinePattern.Output:
                        subs = trimmed.Split(' ', '\t', '\r', '\n', ',').Where(t => t != string.Empty).ToArray();
                        HandleOutputPattern(subs, m);
                        break;
                    case LinePattern.Node:
                        subs = trimmed.Split(' ', '\t', '\r', '\n').Where(t => t != string.Empty).ToArray();
                        HandleNodePattern(subs, m);
                        break;
                    case LinePattern.Headline:
                        subs = trimmed.Split(' ', '\t', '\r', '\n').Where(t => t != string.Empty).ToArray();
                        HandleHeadLinePattern(subs, m);
                        break;
                    case LinePattern.Wire:
                    case LinePattern.Reg:
                        subs = trimmed.Split(' ', '\t', '\r', '\n', ',').Where(t => t != string.Empty).ToArray();
                        HandleWirePattern(subs, m);
                        break;
                }
            }
            HandleWireEdges(m);
            return m;
        }

        private void HandleWireEdges(Module m)
        {
            foreach (var wire in m.GetWires())
            {
                var edges = m.Edges;
                var sources = edges.Where(edge => edge.Item2 == wire);
                if (sources.Count() > 1)
                {
                    throw new Exception("wire cannot be an output for multiple gates");
                }
                var source = sources.ElementAtOrDefault(0);
                if (source == default(Tuple<string, string>)) return;

                var destinations = edges.Where(edge => edge.Item1 == wire).ToArray();
                foreach(var edge in destinations)
                {
                    m.AddEdge(source.Item1, edge.Item2);
                    edges.Remove(edge);
                }
                edges.Remove(source);
                m.Edges = edges;
            }
        }

        private void HandleWirePattern(string[] subs, Module m)
        {
            for (int i = 1; i < subs.Length; ++i)
            {
                m.AddWire(subs[i]);
            }
        }

        private LinePattern GetLinePattern(string trimmed)
        {
            if (HeadlinePattern.IsMatch(trimmed))
                return LinePattern.Headline;
            if (GatePattern.IsMatch(trimmed))
                return LinePattern.Node;
            if (InputPattern.IsMatch(trimmed))
                return LinePattern.Input;
            if (OutputPattern.IsMatch(trimmed))
                return LinePattern.Output;
            if (WirePattern.IsMatch(trimmed))
                return LinePattern.Wire;
            if (RegPattern.IsMatch(trimmed))
                return LinePattern.Reg;

            throw new Exception("Unknown pattern");
        }

        private string[] GetModulesFromStream(StreamReader stream)
        {
            var text = stream.ReadToEnd();
            var res = text.Split("endmodule").ToList().Where(module => module != string.Empty);
            return res.ToArray();
        }

        private void HandleHeadLinePattern(string[] subs, Module m)
        {
            m.Name = subs[1];
        }

        private void HandleNodePattern(string[] subs, Module m)
        {
            var nodeType = subs[0];
            var nodeName = subs[1].Replace("\\", "");
            //string name = m.Name + "-_-" + nodeName;
            string name = nodeName;

            var args = GetNodeArgs(subs);

            var edges = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                int j = arg.IndexOf('(');
                //the input of the Gate we're calling 
                string source = arg.Substring(1, j - 1);
                //the input of the module that we give to the gate
                string destination = arg.Substring(j + 1, arg.Length - j - 2);
                //find if the inModule contains [num] and remove it**********************************
                if (destination.Contains('['))
                {
                    int p = destination.IndexOf('[');
                    destination = destination.Substring(0, p);
                }
                edges.Add(source, destination);
            }

            if (predefinedGates.ContainsKey(nodeType))
            {
                predefinedGatesNamesType.Add(m.Name + "-_-" + nodeName, nodeType);
                var gate = predefinedGates[nodeType];
                if (gate.IsReg)
                {
                    m.AddRegestry(nodeName);   
                }

                foreach (var edge in edges)
                {
                    if (gate.GetInputs().Contains(edge.Key))
                    {
                        m.AddEdge(edge.Value, name);
                    } else
                    {
                        m.AddEdge(name, edge.Value);
                    }
                }
            } else
            {
                customModulesNamesType.Add(m.Name + "-_-" + nodeName, nodeType);
                Module internalModule = parsedModules[nodeType];

                foreach (var edge in edges)
                {
                    var vertixName = name + "-_-" + edge.Key;
                    if (internalModule.GetInputs().Contains(edge.Key))
                    {
                        m.AddEdge(edge.Value, vertixName);
                    }
                    else
                    {
                        m.AddEdge(vertixName, edge.Value);
                    }
                }
            }
        }

        private string[] GetNodeArgs(string[] subs)
        {
            string[] argsArray = new string[subs.Length - 2];
            Array.Copy(subs, 2, argsArray, 0, subs.Length - 2);
            var argsLine = string.Join("", argsArray).Replace(" ", "");
            return argsLine.Substring(1, argsLine.Length - 2).Split(",");
        }

        private void HandleOutputPattern(string[] subs, Module m)
        {
            foreach (var sub in subs)
            {
                if (sub.Equals("output") || sub.Equals("reg") || sub.StartsWith('[') || sub.Equals("inout"))
                    continue;
                else
                {
                    m.AddOutput(sub);
                }
            }
        }

        private void HandleInputPattern(string[] subs, Module m)
        {
            foreach (var sub in subs)
            {
                if (sub.Equals("input") || sub.Equals("reg") || sub.StartsWith('[') || sub.Equals("inout"))
                    continue;
                else
                {
                    m.AddInput(sub);
                }
            }
        }
    }
}