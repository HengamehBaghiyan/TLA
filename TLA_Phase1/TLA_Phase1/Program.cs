using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using TLA_Library;
namespace phase1 {
    public class program
    {
        public static DFA ConvertNFAToDFA(NFA nfa)
        {
            //add kardan q0
            List<string> allStates = new List<string>();
            allStates.Add(nfa.InitialState.state_ID);
            DFA new_automata = new DFA(allStates, nfa.InputSymbols.inputs, nfa.InitialState.state_ID, new List<string>());
            List<NewState> new_states = new List<NewState>();
            //state haye baghimande ke braraye an ha transition ha moshakhas nashode
            new_states.Add(new_automata.InitialState);
            //&&&&&&&state shoro dorost beshe
            //&&&&&&yadam bashe new state 0 ro hazf konam
            Queue<State> included_in_new_state = new Queue<State>();
            NewState tmp = new_automata.InitialState;
            tmp.includedNFAStates.Add(nfa.InitialState);
            included_in_new_state.Enqueue(nfa.InitialState);
            List<State> eachIncludedStateDes = new List<State>();
            List<State> new_eachIncludedStateDes = new List<State>();
            while (included_in_new_state.Count > 0)
            {
                State tempe = included_in_new_state.Dequeue();
                new_eachIncludedStateDes.Add(tempe);
                if (tempe.transitions.ContainsKey(""))
                {
                    for (int k = 0; k < tempe.transitions[""].Count; k++)
                    {
                        if (!new_eachIncludedStateDes.Contains(tempe.transitions[""][k]) && !included_in_new_state.Contains(tempe.transitions[""][k]))
                        {
                            included_in_new_state.Enqueue(tempe.transitions[""][k]);
                        }
                    }
                }
            }
            new_eachIncludedStateDes = new_eachIncludedStateDes.OrderBy(x => x.state_ID.Replace("q", "")).ToList();
            //tmp.includedNFAStates = new_eachIncludedStateDes;
            for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                tmp.includedNFAStates.Add(new_eachIncludedStateDes[i]);
            tmp.string_nfa_states = nfa.InitialState.state_ID;
            do
            {
                if (new_automata.InputSymbols.inputs.Count == 2)
                {
                    eachIncludedStateDes.Clear();

                    for (int j = 0; j < new_states[0].includedNFAStates.Count; j++)
                    {
                        if (new_states[0].includedNFAStates[j].transitions.ContainsKey(new_automata.InputSymbols.inputs[0]))
                        {
                            for (int k = 0; k < new_states[0].includedNFAStates[j].transitions[new_automata.InputSymbols.inputs[0]].Count; k++)
                                eachIncludedStateDes.Add(new_states[0].includedNFAStates[j].transitions[new_automata.InputSymbols.inputs[0]][k]);
                        }
                    }
                    eachIncludedStateDes = eachIncludedStateDes.Distinct().ToList();
                    included_in_new_state.Clear();
                    for (int j = 0; j < eachIncludedStateDes.Count; j++)
                    {
                        included_in_new_state.Enqueue(eachIncludedStateDes[j]);
                    }
                    new_eachIncludedStateDes.Clear();
                    while (included_in_new_state.Count > 0)
                    {
                        State tempe = included_in_new_state.Dequeue();
                        new_eachIncludedStateDes.Add(tempe);
                        if (tempe.transitions.ContainsKey(""))
                        {

                            for (int k = 0; k < tempe.transitions[""].Count; k++)
                            {
                                if (!new_eachIncludedStateDes.Contains(tempe.transitions[""][k]) && !included_in_new_state.Contains(tempe.transitions[""][k]))
                                {
                                    included_in_new_state.Enqueue(tempe.transitions[""][k]);
                                }
                            }
                        }
                    }
                    new_eachIncludedStateDes = new_eachIncludedStateDes.OrderBy(x => x.state_ID.Replace("q", "")).ToList();
                    string temp = "";
                    for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                    {
                        temp += new_eachIncludedStateDes[i].state_ID;
                    }
                    // Console.WriteLine(new_states[0].state_ID+":"+temp);
                    if (new_eachIncludedStateDes.Count == 0)
                    {
                        bool existance = false;
                        int find_number = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].state_ID == "TRAP")
                            {
                                existance = true;
                                find_number = i;
                                break;
                            }
                        }
                        if (!existance)
                        {
                            NewState new_St = new NewState("TRAP");
                            new_St.includedNFAStates = new List<State>();
                            new_St.string_nfa_states = "";
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_St);
                            ///pakesh kon
                            new_St.transitions[new_automata.InputSymbols.inputs[0]] = new_St;
                            new_St.transitions[new_automata.InputSymbols.inputs[1]] = new_St;
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_automata.States[find_number]);
                        }
                    }
                    else
                    {
                        bool check = false;
                        int number_StateDFA = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].string_nfa_states == temp)
                            {
                                check = true;
                                number_StateDFA = i;
                                break;
                            }
                        }
                        if (!check)
                        {
                            NewState new_St = new NewState(temp);
                            for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                                new_St.includedNFAStates.Add(new_eachIncludedStateDes[i]);
                            new_St.string_nfa_states = temp;
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_St);
                            new_states.Add(new_St);
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_automata.States[number_StateDFA]);
                        }
                    }
                    eachIncludedStateDes.Clear();
                    for (int j = 0; j < new_states[0].includedNFAStates.Count; j++)
                    {
                        if (new_states[0].includedNFAStates[j].transitions.ContainsKey(new_automata.InputSymbols.inputs[1]))
                            eachIncludedStateDes = eachIncludedStateDes.Concat(new_states[0].includedNFAStates[j].transitions[new_automata.InputSymbols.inputs[1]]).ToList();
                    }
                    eachIncludedStateDes = eachIncludedStateDes.Distinct().ToList();
                    included_in_new_state.Clear();
                    for (int j = 0; j < eachIncludedStateDes.Count; j++)
                    {
                        included_in_new_state.Enqueue(eachIncludedStateDes[j]);
                    }
                    new_eachIncludedStateDes.Clear();
                    while (included_in_new_state.Count > 0)
                    {
                        State tempe = included_in_new_state.Dequeue();
                        new_eachIncludedStateDes.Add(tempe);
                        if (tempe.transitions.ContainsKey(""))
                        {
                            for (int k = 0; k < tempe.transitions[""].Count; k++)
                            {
                                if (!new_eachIncludedStateDes.Contains(tempe.transitions[""][k]) && !included_in_new_state.Contains(tempe.transitions[""][k]))
                                {
                                    included_in_new_state.Enqueue(tempe.transitions[""][k]);
                                }
                            }
                        }
                    }
                    new_eachIncludedStateDes = new_eachIncludedStateDes.OrderBy(x => x.state_ID.Replace("q", "")).ToList();
                    temp = "";
                    for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                    {
                        temp += new_eachIncludedStateDes[i].state_ID;
                    }
                    //Console.WriteLine(new_states[0].state_ID + ":" + temp);
                    if (new_eachIncludedStateDes.Count == 0)
                    {
                        bool existance = false;
                        int find_number = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].state_ID == "TRAP")
                            {
                                existance = true;
                                find_number = i;
                                break;
                            }
                        }
                        if (!existance)
                        {
                            NewState new_St = new NewState("TRAP");
                            new_St.includedNFAStates = new List<State>();
                            new_St.string_nfa_states = "";
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[1], new_St);
                            ///pakesh kon
                            new_St.transitions[new_automata.InputSymbols.inputs[0]] = new_St;
                            new_St.transitions[new_automata.InputSymbols.inputs[1]] = new_St;
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[1], new_automata.States[find_number]);
                        }
                    }
                    else
                    {
                        bool check = false;
                        int number_StateDFA = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].string_nfa_states == temp)
                            {
                                check = true;
                                number_StateDFA = i;
                                break;
                            }
                        }
                        if (!check)
                        {
                            NewState new_St = new NewState(temp);
                            //new_St.includedNFAStates = new_eachIncludedStateDes;
                            for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                                new_St.includedNFAStates.Add(new_eachIncludedStateDes[i]);
                            new_St.string_nfa_states = temp;
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[1], new_St);
                            new_states.Add(new_St);
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[1], new_automata.States[number_StateDFA]);
                        }
                    }
                    new_states.RemoveAt(0);
                }

                if (new_automata.InputSymbols.inputs.Count == 1)
                {
                    eachIncludedStateDes.Clear();
                    for (int j = 0; j < new_states[0].includedNFAStates.Count; j++)
                    {
                        if (new_states[0].includedNFAStates[j].transitions.ContainsKey(new_automata.InputSymbols.inputs[0]))
                            eachIncludedStateDes = eachIncludedStateDes.Concat(new_states[0].includedNFAStates[j].transitions[new_automata.InputSymbols.inputs[0]]).ToList();

                        //eachIncludedStateDes.Concat(new_states[0].includedNFAStates[j].transitions[new_automata.InputSymbols.inputs[0]]);
                    }
                    eachIncludedStateDes = eachIncludedStateDes.Distinct().ToList();
                    included_in_new_state.Clear();
                    for (int j = 0; j < eachIncludedStateDes.Count; j++)
                    {
                        included_in_new_state.Enqueue(eachIncludedStateDes[j]);
                    }
                    new_eachIncludedStateDes.Clear();
                    while (included_in_new_state.Count > 0)
                    {
                        State tempe = included_in_new_state.Dequeue();
                        new_eachIncludedStateDes.Add(tempe);
                        if (tempe.transitions.ContainsKey(""))
                        {
                            for (int k = 0; k < tempe.transitions[""].Count; k++)
                            {
                                if (!new_eachIncludedStateDes.Contains(tempe.transitions[""][k]) && !included_in_new_state.Contains(tempe.transitions[""][k]))
                                {
                                    included_in_new_state.Enqueue(tempe.transitions[""][k]);
                                }
                            }
                        }
                    }
                    new_eachIncludedStateDes = new_eachIncludedStateDes.OrderBy(x => x.state_ID.Replace("q", "")).ToList();
                    string temp = "";
                    for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                    {
                        temp += new_eachIncludedStateDes[i].state_ID;
                    }
                    if (new_eachIncludedStateDes.Count == 0)
                    {
                        bool existance = false;
                        int find_number = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].state_ID == "TRAP")
                            {
                                existance = true;
                                find_number = i;
                                break;
                            }
                        }
                        if (!existance)
                        {
                            NewState new_St = new NewState("TRAP");
                            new_St.includedNFAStates = new List<State>();
                            new_St.string_nfa_states = "";
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_St);
                            ///pakesh kon
                            new_St.transitions[new_automata.InputSymbols.inputs[0]] = new_St;
                            new_St.transitions[new_automata.InputSymbols.inputs[1]] = new_St;
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_automata.States[find_number]);
                        }
                    }
                    else
                    {
                        bool check = false;
                        int number_StateDFA = -1;
                        for (int i = 0; i < new_automata.States.Count; i++)
                        {
                            if (new_automata.States[i].string_nfa_states == temp)
                            {
                                check = true;
                                number_StateDFA = i;
                                break;
                            }
                        }
                        if (!check)
                        {
                            NewState new_St = new NewState(temp);
                            //new_St.includedNFAStates = new_eachIncludedStateDes;
                            for (int i = 0; i < new_eachIncludedStateDes.Count; i++)
                                new_St.includedNFAStates.Add(new_eachIncludedStateDes[i]);
                            new_St.string_nfa_states = temp;
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_St);
                            new_states.Add(new_St);
                            new_automata.States.Add(new_St);
                        }
                        else
                        {
                            new_states[0].transitions.Add(new_automata.InputSymbols.inputs[0], new_automata.States[number_StateDFA]);
                        }
                    }
                    new_states.RemoveAt(0);
                }

            } while (new_states.Count > 0);
            for (int i = 0; i < new_automata.States.Count; i++)
            {
                for (int j = 0; j < new_automata.States[i].includedNFAStates.Count; j++)
                {
                    if (nfa.FinalStates.Contains(new_automata.States[i].includedNFAStates[j]))
                    {
                        new_automata.FinalStates.Add(new_automata.States[i]);
                        break;
                    }
                }
            }
            return new_automata;
        }

        public static void Main(string[] args)
        {
            var text = File.ReadAllText(@"C:/Users/win_10/TLA01-Projects/samples/phase1-sample/in/input2.json");
            NFA nfa = ConvertJsonToFA.ConvertJsonToNFA(text);
            DFA dfa = ConvertNFAToDFA(nfa);
            ConvertFAToJson.ConvertDFAToJson(dfa);
        }
    }
}

