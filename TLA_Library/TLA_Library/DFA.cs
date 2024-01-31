using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLA_Library
{
    public partial class DFA
    {
        public List<NewState> States;
        public InputSymbols InputSymbols;
        public List<NewState> FinalStates;
        public NewState InitialState = null;
        public DFA(int x, List<NewState> allStates, InputSymbols allInputSymbols, NewState initialState, List<NewState> finalStates)
        {
            States = allStates;
            FinalStates = finalStates;
            this.InputSymbols = allInputSymbols;
            InitialState = initialState;
        }
        public DFA(List<string> allStates, List<string> allInputSymbols, string initialState, List<string> finalStates)
        {
            States = new List<NewState>();
            FinalStates = new List<NewState>();
            InputSymbols inputSymbols = new InputSymbols();
            inputSymbols.setInputs(allInputSymbols);
            InputSymbols = inputSymbols;

            for (int i = 0; i < finalStates.Count; i++)
                FinalStates.Add(new NewState(finalStates[i]));

            for (int i = 0; i < allStates.Count; i++)
                States.Add(new NewState(allStates[i]));

            for (int i = 0; i < allStates.Count; i++)
                if (allStates[i].Equals(initialState))
                    InitialState = States[i];
        }
        public DFA(List<NewState> states, NewState initial_state, List<NewState> final_states,

                List<string> input_symbols, Dictionary<string, Dictionary<string, string>> transitions)
        {
            States = states;
            InitialState = initial_state;
            FinalStates = final_states;
            InputSymbols = new InputSymbols();
            InputSymbols.setInputs(input_symbols);

            for (int i = 0; i < States.Count(); i++)
            {
                var trans = transitions[States[i].state_ID];
                States[i].transitions = new Dictionary<string, NewState>();
                foreach (var item in trans)
                {
                    int q = States.FindIndex(x => x.state_ID == item.Value);
                    States[i].transitions.Add(item.Key, States[q]);
                }
            }
        }
    }
    public partial class NewState
    {
        public Dictionary<string, NewState> transitions;
        public List<State> includedNFAStates;
        public string string_nfa_states = "";
        public string state_ID;
        public bool Visit;
        public NewState(string stateName)
        {
            state_ID = stateName;
            transitions = new Dictionary<string, NewState>();
            includedNFAStates = new List<State>();
        }

    }
}
