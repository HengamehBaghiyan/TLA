using System.Text.RegularExpressions;
namespace TLA_Library
{
    public class NFA
    {
        public List<State> States;
        public InputSymbols InputSymbols;
        public List<State> FinalStates;
        public State InitialState = null;
        public NFA(List<State> states, State initial_state, List<State> final_states,

                List<string> input_symbols, Dictionary<string, Dictionary<string, string>> transitions, int x)
        {
            States = states;
            InitialState = initial_state;
            FinalStates = final_states;
            InputSymbols = new InputSymbols();
            InputSymbols.setInputs(input_symbols);

            for (int i = 0; i < States.Count(); i++)
            {
                var trans = transitions[States[i].state_ID];
                States[i].transitions = new Dictionary<string, List<State>>();
                foreach (var item in trans)
                {
                    int q = States.FindIndex(x => x.state_ID == item.Value);
                    List<State> tmp = new List<State>();
                    tmp.Add(States[q]);
                    States[i].transitions.Add(item.Key, tmp);
                }
            }
        }
        public NFA(List<State> states, State initial_state, List<State> final_states, List<string> input_symbols, Dictionary<string, Dictionary<string, string>> transitions)
        {
            States = states;
            InitialState = initial_state;
            FinalStates = final_states;
            InputSymbols = new InputSymbols();
            InputSymbols.inputs = input_symbols;
            for (int i = 0; i < States.Count(); i++)
            {
                var trans = transitions[States[i].state_ID];
                foreach (var item in trans)
                {
                    var sta = Regex.Replace(item.Value, @"[{}']", "").Split(",").ToList();
                    var tr = sta.Select(x => States[int.Parse(x.Substring(1))]).ToList();
                    States[i].transitions.Add(item.Key, tr);
                }
            }
        }
        public NFA(List<string> allStates, List<string> allInputSymbols, string initialState, List<string> finalStates)
        {
            States = new List<State>();
            FinalStates = new List<State>();
            InputSymbols inputSymbols = new InputSymbols();
            inputSymbols.setInputs(allInputSymbols);
            InputSymbols = inputSymbols;

            for (int i = 0; i < finalStates.Count; i++)
            {

                FinalStates.Add(new State(finalStates[i]));
            }
            for (int i = 0; i < allStates.Count; i++)
                States.Add(new State(allStates[i]));

            for (int i = 0; i < allStates.Count; i++)
                if (allStates[i].Equals(initialState))
                    InitialState = States[i];

        }
    }
    public class NFAWithSingleFinalState
    {
        public List<State> States;
        public InputSymbols InputSymbols;
        public State FinalState;
        public State InitialState;
        public NFAWithSingleFinalState(List<State> allStates, List<string> allInputSymbols, State initialState, State finalState)
        {
            States = new List<State>();
            FinalState = finalState;
            InitialState = initialState;
            InputSymbols = new InputSymbols();
            InputSymbols.setInputs(allInputSymbols);
            States = allStates;
        }
    }
    public class State
    {
        public Dictionary<string, List<State>> transitions;
        public string state_ID;
        public State(string stateName)
        {
            transitions = new Dictionary<string, List<State>>();
            state_ID = stateName;
        }
    }
}

