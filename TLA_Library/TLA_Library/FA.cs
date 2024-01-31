using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLA_Library
{
    public partial class InputSymbols
    {
        public List<string> inputs;
        public void setInputs(List<string> transitionSygma)
        {
            inputs = transitionSygma;
        }
    }
    public class FA
    {
        public string states { get; set; }
        public string input_symbols { get; set; }
        public Dictionary<string, Dictionary<string, string>> transitions { get; set; }
        public string initial_state { get; set; }
        public string final_states { get; set; }
    }
}
