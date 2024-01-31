using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLA_Library
{
    public class ConvertFAToJson
    {
        public static void ConvertNFAToJson(NFAWithSingleFinalState nfa)
        {
            string states = "{" + string.Join(',', nfa.States.Select(x => $"'{x.state_ID}'")) + "}";
            string final_states = "{" + string.Join(',', nfa.States.Select(x => $"'{x.state_ID}'")) + "}";
            string initial_state = nfa.InitialState.state_ID;
            string input_symbols = "{" + string.Join(',', nfa.InputSymbols.inputs.Select(x => $"'{x}'")) + "}";
            var trans = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> temp;
            for (int i = 0; i < nfa.States.Count; i++)
            {
                temp = new Dictionary<string, string>();
                foreach (var item in nfa.States[i].transitions)
                {
                    var q = "{" + string.Join(',', item.Value.Select(x => $"'{x.state_ID}'")) + "}";
                    temp.Add(item.Key, q);
                }
                trans.Add(nfa.States[i].state_ID, temp);
            }
            FA result = new FA() { states = states, final_states = final_states, initial_state = initial_state, input_symbols = input_symbols, transitions = trans };
            string jason = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            jason = Regex.Unescape(jason);
            File.WriteAllText(@"C:/Users/ASUS/Desktop/TLA/TLA_4/TLA_4/RFA.json", jason, Encoding.UTF8);

        }
        public static void ConvertDFAToJson(DFA dfa)
        {
            string states = "{" + string.Join(',', dfa.States.Select(x => $"\u0027{x.state_ID}\u0027")) + "}";
            string final_states = "{" + string.Join(',', dfa.FinalStates.Select(x => $"'{x.state_ID}'")) + "}";
            string initial_state = dfa.InitialState.state_ID;
            string input_symbols = "{" + string.Join(',', dfa.InputSymbols.inputs.Select(x => $"'{x}'")) + "}";
            var trans = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> temp;
            foreach (var item in dfa.States)
            {
                temp = new Dictionary<string, string>();
                foreach (var x in item.transitions)
                    temp.Add(x.Key, x.Value.state_ID);
                trans.Add(item.state_ID, temp);
            }
            FA result = new FA() { states = states, final_states = final_states, initial_state = initial_state, input_symbols = input_symbols, transitions = trans };
            string jason = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            jason = Regex.Unescape(jason);
            File.WriteAllText(@"C:/Users/win_10/Desktop/TLA_1/RFA.json", jason, Encoding.UTF8);

        }
    }
}
