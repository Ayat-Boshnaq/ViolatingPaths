using System;
using System.Collections.Generic;

namespace ViolatingPaths.Models
{
    public class Circuit : ICircuit
    {
             private Dictionary<string, List<string>> graph;
        public List<string> Regs { get; }
        public List<string> Inputs { get; }
        public List<string> Outputs { get; }
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


        //*****************************************************************************************************************

        public void GetViolatingPathsAux(int threshold, string vertix, int path_length, List<string> path,
            Dictionary<string, Boolean> visited_regs, List<List<string>> violating_paths)
        {
            //1.the current vertix in the path is a register
            path.Add(vertix);
            path_length += 1;
            if (Regs.Contains(vertix))
            {
                if (path_length > threshold)
                {    
                    violating_paths.Add(new List<string>(path));
                }
                if (visited_regs[vertix] == false)
                {
                    visited_regs[vertix] = true;
                    foreach (var ver in graph[vertix])
                    {
                        List<string> new_path = new List<string>();
                        new_path.Add(vertix);
                        GetViolatingPathsAux(threshold,ver, 1, new_path, visited_regs, violating_paths);
                    }
                }

            }
            //2.the current vertix is an output
            else if (Outputs.Contains(vertix))
                {
                    if (path_length > threshold)
                    {
                    if (!Inputs.Contains(path[0]))
                    {
                        violating_paths.Add(new List<string>(path));
                        return;
                    }

                    }
                }

            //3.the current vertex is neither a register nor an output
            //we add the current verex to the path and increment the length of the path by one
            //and continue recursively searching from the next vertix....
            else
            {
                foreach (var ver in graph[vertix])
                {
                    List<string>  new_path = new List<string>(path);
                    GetViolatingPathsAux(threshold, ver, path_length, new_path, visited_regs,violating_paths);
                }
            }


        }
        //*****************************************************************************************************************
        public void GetViolatingPaths(int threshold)
        {
            List<List<string>> violating_paths = new List<List<string>>();
            Dictionary<string, Boolean> visited_regs = new Dictionary<string, Boolean>();
            foreach (var ver in Regs)
            {
                visited_regs.Add(ver, false);
            }
            foreach (var input in Inputs)
            {
                List<string> path = new List<string>();
                GetViolatingPathsAux(threshold, input, 1, path, visited_regs, violating_paths);
            }
            Console.WriteLine();
            Console.WriteLine("number of violating paths is :" + violating_paths.Count);
            Console.WriteLine("These are the violating paths :");
            foreach (var path in violating_paths)
            {
                foreach (var vertix in path)
                {
                    Console.Write(vertix+" ");
                }
                Console.WriteLine();
            }

        }
    }
}
