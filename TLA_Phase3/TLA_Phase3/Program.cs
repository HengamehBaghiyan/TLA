using System.Text.Json;
using System.Text.RegularExpressions;
using TLA_Library;
using phase1;

class program2
{
    public static bool IsAccepted(string input, DFA automata)
    {
        NewState new_sate = automata.InitialState;
        for (int i = 0; i < input.Length; i++)
        {
            new_sate = new_sate.transitions[input[i].ToString()];
        }
        if (automata.FinalStates.Contains(new_sate))
            return true;
        else
            return false;
    }
    public static void Main(string[] args)
    {
        var text = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/in/input2.json");
        FA fa = JsonSerializer.Deserialize<FA>(text);
        var sta = Regex.Replace(fa.states, @"[{}']", "").Split(",").ToList();
        var _input_symbols = Regex.Replace(fa.input_symbols, @"[{}']", "").Split(",").ToList();
        var sta1 = Regex.Replace(fa.final_states, @"[{}']", "").Split(",").ToList();
        var m = fa.transitions.First().Value;
        DFA dfa = null;
        NFA nfa = null;
        if (m.First().Value.Contains('{'))
        {
            var _states = sta.Select(x => new State(x)).ToList();
            List<State> _final_states = new List<State>();
            for (int i = 0; i < sta1.Count(); i++)
                _final_states.Add(_states.Where(x => x.state_ID == sta1[i]).First());
            var _initial_state = _states.Where(x => x.state_ID == fa.initial_state).First();
            nfa = new NFA(_states, _initial_state, _final_states, _input_symbols, fa.transitions);
            dfa = program.ConvertNFAToDFA(nfa);
        }
        else
        {
            var _states = sta.Select(x => new NewState(x)).ToList();
            List<NewState> _final_states = new List<NewState>();
            for (int i = 0; i < sta1.Count(); i++)
                _final_states.Add(_states.Where(x => x.state_ID == sta1[i]).First());
            var _initial_state = _states.Where(x => x.state_ID == fa.initial_state).First();
            dfa = new DFA(_states, _initial_state, _final_states, _input_symbols, fa.transitions);
        }
        var input_string = Console.ReadLine();
        if (IsAccepted(input_string, dfa))
            Console.WriteLine("Accepted");
        else
            Console.WriteLine("Rejected");
    }
}

