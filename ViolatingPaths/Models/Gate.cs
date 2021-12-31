using System.Collections.Generic;
using System.Linq;

namespace ViolatingPaths.Models
{
    public class Gate
    {
        private string type;
        private string[] inputs;
        private string[] outputs;
        private bool isReg;

        public Gate(string type, string[] inputs, string[] outputs, bool isReg)
        {
            this.type = type;
            this.inputs = inputs;
            this.outputs = outputs;
            this.isReg = isReg;
        }

        public List<string> GetInputs() => inputs.ToList();
    }
}
