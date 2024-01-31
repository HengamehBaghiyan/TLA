using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLA_Library
{
    public class ConvertJsonToFA
    {
        public static DFA ConvertJsonToDFA(string text)
        {
            //var text = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/in/input2.json");
            FA fa = JsonSerializer.Deserialize<FA>(text);
            var sta = Regex.Replace(fa.states, @"[{}']", "").Split(",").ToList();
            var _input_symbols = Regex.Replace(fa.input_symbols, @"[{}']", "").Split(",").ToList();
            var sta1 = Regex.Replace(fa.final_states, @"[{}']", "").Split(",").ToList();
            var m = fa.transitions.First().Value;
            DFA dfa = null;
            //NFA nfa = null;
            if (!m.First().Value.Contains('{'))
            {
                var _states = sta.Select(x => new NewState(x)).ToList();
                List<NewState> _final_states = new List<NewState>();
                for (int i = 0; i < sta1.Count(); i++)
                    _final_states.Add(_states.Where(x => x.state_ID == sta1[i]).First());
                var _initial_state = _states.Where(x => x.state_ID == fa.initial_state).First();
                dfa = new DFA(_states, _initial_state, _final_states, _input_symbols, fa.transitions);
            }
            return dfa;
        }
        public static NFA ConvertJsonToNFA(string text)
        {
            //var text = File.ReadAllText(@"C:/Users/win_10/TLA01-Projects/samples/phase1-sample/in/input2.json");
            FA fa = JsonSerializer.Deserialize<FA>(text);
            var sta = Regex.Replace(fa.states, @"[{}']", "").Split(",").ToList();
            var _states = sta.Select(x => new State(x)).ToList();
            var _input_symbols = Regex.Replace(fa.input_symbols, @"[{}']", "").Split(",").ToList();
            sta = Regex.Replace(fa.final_states, @"[{}']", "").Split(",").ToList();
            List<State> _final_states = new List<State>();
            for (int i = 0; i < sta.Count(); i++)
                _final_states.Add(_states.Where(x => x.state_ID == sta[i]).First());
            var _initial_state = _states.Where(x => x.state_ID == fa.initial_state).First();
            var m = fa.transitions.First().Value;
            NFA nfa = null;
            if (m.First().Value.Contains('{'))
                nfa = new NFA(_states, _initial_state, _final_states, _input_symbols, fa.transitions);
            return nfa;
        }
    }
}
