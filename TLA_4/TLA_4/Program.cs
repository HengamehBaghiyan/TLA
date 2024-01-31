using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace TLA
{
    public class FAOperations
    {
        public partial class NFA
        {
            public List<NFAState> States;
            public InputSymbols InputSymbols;
            public List<NFAState> FinalStates;
            public NFAState InitialState;
            public NFA(List<string> allStates, List<string> allInputSymbols, string initialState, List<string> finalStates)
            {
                States = new List<NFAState>(allStates.Count); ;
                FinalStates = new List<NFAState>(finalStates.Count);
                InputSymbols inputSymbols = new InputSymbols();
                inputSymbols.setInputs(allInputSymbols);


                for (int i = 0; i < finalStates.Count; i++)
                    FinalStates.Add(new NFAState(finalStates[i]));

                for (int i = 0; i < allStates.Count; i++)
                    States.Add(new NFAState(allStates[i]));

                for (int i = 0; i < allStates.Count; i++)
                    if (allStates[i].Equals(initialState))
                        InitialState = States[i];

            }
        }
        public partial class DFA
        {
            public List<DFAState> States;
            public InputSymbols InputSymbols;
            public List<DFAState> FinalStates;
            public DFAState InitialState;
            public DFA(List<string> allStates, List<string> allInputSymbols, string initialState, List<string> finalStates)
            {
                States = new List<DFAState>(allStates.Count); ;
                FinalStates = new List<DFAState>(finalStates.Count);
                InputSymbols inputSymbols = new InputSymbols();
                inputSymbols.setInputs(allInputSymbols);

                for (int i = 0; i < finalStates.Count; i++)
                    FinalStates.Add(new DFAState(finalStates[i]));

                for (int i = 0; i < allStates.Count; i++)
                    States.Add(new DFAState(allStates[i]));

                for (int i = 0; i < allStates.Count; i++)
                    if (allStates[i].Equals(initialState))
                        InitialState = States[i];
            }
        }
        public partial class NFAWithSingleFinalState
        {
            public List<NFAState> States;
            public InputSymbols InputSymbols;
            public NFAState FinalState;
            public NFAState InitialState;
            public NFAWithSingleFinalState(List<NFAState> allStates, List<string> allInputSymbols, NFAState initialState, NFAState finalState)
            {
                States = new List<NFAState>();
                FinalState = finalState;
                InitialState = initialState;
                InputSymbols inputSymbols = new InputSymbols();
                inputSymbols.setInputs(allInputSymbols);
                States = allStates;
            }
        }
        public partial class NFAState
        {
            public Dictionary<string, List<NFAState>> transitions;
            public string state_ID;
            public NFAState(string stateName)
            {
                state_ID = stateName;
            }
        }
        public partial class DFAState
        {
            public List<Tuple<string, DFAState>> transitions;
            public string state_ID;
            public DFAState(string stateName)
            {
                state_ID = stateName;
            }
        }
        public partial class InputSymbols
        {
            public List<string> inputs;
            public void setInputs(List<string> transitionSygma)
            {
                inputs = transitionSygma;
            }
        }
        public static NFAWithSingleFinalState NFA_Single_FinalState(NFA nfa)
        {

            NFAState new_finalState = new NFAState("q" + nfa.States.Count().ToString());
            List<NFAState> t_previousFinalState = new List<NFAState>();
            t_previousFinalState.Add(new_finalState);
            for (int i = 0; i < nfa.FinalStates.Count; i++)
                nfa.States[i].transitions.Add("", t_previousFinalState);

            NFAWithSingleFinalState new_nfa_withSingleFinalState = new NFAWithSingleFinalState(nfa.States, nfa.InputSymbols.inputs, nfa.InitialState, new_finalState);
            return new_nfa_withSingleFinalState;
        }
        public static NFAWithSingleFinalState Star(NFA nfa)
        {

            //new NFA with a single final state 
            NFAWithSingleFinalState new_nfa_single_final = NFA_Single_FinalState(nfa);
            //state avaliye jadid - checked
            NFAState new_initialState = new NFAState("q" + (new_nfa_single_final.States.Count).ToString());
            //state payani jadid - checked
            NFAState new_finalState = new NFAState("q" + (new_nfa_single_final.States.Count + 1).ToString());

            //be state haye koli add mikonim 2 ta state ezafe shode ro
            new_nfa_single_final.States.Add(new_initialState);
            new_nfa_single_final.States.Add(new_finalState);

            //az state avaliye jadid ba lambda mirim be state avaliye qabli 
            //az statte avaliye jadidi ba lambda mirim be state payani jadid
            List<NFAState> t_newinitialState = new List<NFAState>();
            t_newinitialState.Add(new_nfa_single_final.States[new_nfa_single_final.States.IndexOf(new_nfa_single_final.InitialState)]);
            t_newinitialState.Add(new_finalState);
            new_nfa_single_final.InitialState = new_initialState;
            new_nfa_single_final.InitialState.transitions.Add("", t_newinitialState);

            //az payani jadid ba lambda mirim be state avaliye jadid
            //az final qabli ba lambda mirim be final jadid
            List<NFAState> t_newfinalState = new List<NFAState>();
            t_newfinalState.Add(new_initialState);
            t_newfinalState.Add(new_finalState);
            new_nfa_single_final.FinalState.transitions.Add("", t_newinitialState);

            //state payani qabli ro non final mikonam :)
            new_nfa_single_final.FinalState = new_finalState;

            return new_nfa_single_final;
        }
        public static NFAWithSingleFinalState Concat(NFA f_NFA, NFA s_NFA)
        {
            //NFA ba yek node payani 
            NFAWithSingleFinalState concatedNfa;

            //2 NFA mojod ro tabdil mikonim be NFA ba tak state payani 
            NFAWithSingleFinalState f_NFA_S = NFA_Single_FinalState(f_NFA);
            NFAWithSingleFinalState s_NFA_S = NFA_Single_FinalState(s_NFA);

            //Ye final jadid ezafe mikonim baraye inke 2vomin nfa concat shode ba lambda bere behesh 
            NFAState new_finalState = new NFAState("q" + (f_NFA_S.States.Count + s_NFA_S.States.Count));
            //if --> age nfa aval ba lambda mire be state ei ono update kon 
            //else --> age nfa aval ba lambda nemire be state ei ye list jadid initialize kon add kon 
            if (f_NFA_S.FinalState.transitions.ContainsKey(""))
                f_NFA_S.FinalState.transitions[""].Add(s_NFA_S.InitialState);
            else
            {
                List<NFAState> f_NFA_pre_final_first = new List<NFAState>();
                f_NFA_pre_final_first.Add(s_NFA_S.InitialState);
                f_NFA_S.FinalState.transitions.Add("", f_NFA_pre_final_first);
            }

            //if --> age nfa dovom ba lambda mire be state ei ono update kon 
            //else --> age nfa dovom ba lambda nemire be state ei ye list jadid initialize kon add kon final state jadi ro toosh 
            if (f_NFA_S.FinalState.transitions.ContainsKey(""))
                f_NFA_S.FinalState.transitions[""].Add(new_finalState);
            else
            {
                List<NFAState> s_NFA_pre_final_first = new List<NFAState>();
                s_NFA_pre_final_first.Add(new_finalState);
                s_NFA_S.FinalState.transitions.Add("", s_NFA_pre_final_first);
            }

            //NFA jadid ro misazim az tarkib in 2 transition System
            List<NFAState> new_NFA_all_states = new List<NFAState>();
            List<string> all_input_symbols = new List<string>();

            //first state haye nfa aval ro ezafe mikonim
            for (int i = 0; i < f_NFA.States.Count; i++)
            {
                NFAState new_state = new NFAState("q" + i.ToString());
                new_state = f_NFA.States[i];
                new_NFA_all_states.Add(new_state);
            }
            //next state haye nfa dovom ro ezafe mikonim
            for (int i = 0; i < s_NFA.States.Count; i++)
            {
                NFAState new_state = new NFAState("q" + (i.ToString() + new_NFA_all_states.Count.ToString()));
                new_state = s_NFA.States[i];
                new_NFA_all_states.Add(new_state);
            }

            //Then hameye input symbol haro add mikonim(nfa(1))
            for (int i = 0; i < f_NFA_S.InputSymbols.inputs.Count; i++)
                all_input_symbols.Add(f_NFA_S.InputSymbols.inputs[i]);
            //Then hameye input symbol haro add mikonim(nfa(2))
            for (int i = 0; i < s_NFA_S.InputSymbols.inputs.Count; i++)
                all_input_symbols.Add(s_NFA_S.InputSymbols.inputs[i]);
            //??????????????????????????????????
            //in tike akhar doroste ya na ?
            concatedNfa = new NFAWithSingleFinalState(new_NFA_all_states, all_input_symbols, f_NFA_S.InitialState, new_finalState);
            return concatedNfa;
        }
        public static NFAWithSingleFinalState Union(NFA f_NFA, NFA s_NFA)
        {
            //NFA union jadid
            NFAWithSingleFinalState concatedNfa;

            //2 NFA mojod ro tabdil mikonim be NFA ba tak state payani 
            NFAWithSingleFinalState f_NFA_S = NFA_Single_FinalState(f_NFA);
            NFAWithSingleFinalState s_NFA_S = NFA_Single_FinalState(s_NFA);

            //Ye initial jadid ezafe mikonim baraye inke 2vomin nfa concat shode ba lambda bere behesh 
            NFAState new_intialState = new NFAState("q" + (f_NFA_S.States.Count + s_NFA_S.States.Count));

            //Ye final jadid ezafe mikonim baraye inke 2vomin nfa concat shode ba lambda bere behesh 
            NFAState new_finalState = new NFAState("q" + (f_NFA_S.States.Count + s_NFA_S.States.Count) + 1);

            //state avaliye jadid ba lambda mire state avaliye nfa(1) va nfa(2)
            List<NFAState> t_newInitialState = new List<NFAState>();
            t_newInitialState.Add(f_NFA_S.InitialState);
            t_newInitialState.Add(s_NFA_S.InitialState);
            new_intialState.transitions.Add("", t_newInitialState);

            //state haye payani nfa(1) va nfa(2) ba lambda miran be state final jadid

            List<NFAState> t_previousFinalState = new List<NFAState>();
            t_previousFinalState.Add(new_finalState);
            f_NFA_S.FinalState.transitions.Add("", t_previousFinalState);
            s_NFA_S.FinalState.transitions.Add("", t_previousFinalState);

            List<NFAState> new_NFA_all_states = new List<NFAState>();
            List<string> all_input_symbols = new List<string>();

            //first state haye nfa aval ro ezafe mikonim
            for (int i = 0; i < f_NFA.States.Count; i++)
            {
                NFAState new_state = new NFAState("q" + i.ToString());
                new_state = f_NFA.States[i];
                new_NFA_all_states.Add(new_state);
            }
            //next state haye nfa dovom ro ezafe mikonim
            for (int i = 0; i < s_NFA.States.Count; i++)
            {
                NFAState new_state = new NFAState("q" + (i.ToString() + new_NFA_all_states.Count.ToString()));
                new_state = s_NFA.States[i];
                new_NFA_all_states.Add(new_state);
            }

            //Then hameye input symbol haro add mikonim(nfa(1))
            for (int i = 0; i < f_NFA_S.InputSymbols.inputs.Count; i++)
                all_input_symbols.Add(f_NFA_S.InputSymbols.inputs[i]);
            //Then hameye input symbol haro add mikonim(nfa(2))
            for (int i = 0; i < s_NFA_S.InputSymbols.inputs.Count; i++)
                all_input_symbols.Add(s_NFA_S.InputSymbols.inputs[i]);
            //??????????????????????????????????
            //in tike akhar doroste ya na ?
            concatedNfa = new NFAWithSingleFinalState(new_NFA_all_states, all_input_symbols, new_intialState, new_finalState);
            return concatedNfa;
        }

        public static void Main(string[] args)
        {
            string operation = Console.ReadLine();
            switch (operation)
            {
                case "Star":
                    {
                        string json = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/in/input1.json");

                        dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
                        string states = jsonObject.states;
                        string inputSymbols = jsonObject.input_symbols;
                        string finalStates = jsonObject.final_states;
                        string initialState = jsonObject.initial_state;

                        List<string> allstates = states.Trim(new char[] { '{', '}' }).Split(',')
                        .Select(s => s.Trim(new char[] { '\'', ' ' })).ToList();
                        List<string> inputs = inputSymbols.Trim(new char[] { '{', '}' }).Split(',')
                        .Select(s => s.Trim(new char[] { '\'', ' ' })).ToList();
                        List<string> finals = finalStates.Trim(new char[] { '{', '}' }).Split(',')
                        .Select(s => s.Trim(new char[] { '\'', ' ' })).ToList();


                        NFA automata = new NFA(allstates, inputs, initialState, finals);
                        for (int i = 0; i < automata.States.Count; i++)
                        {
                            automata.States[i].transitions = new Dictionary<string, List<NFAState>>();
                            List<NFAState> transitionStates = new List<NFAState>();
                            string a = jsonObject.transitions[allstates[i]].ToString().Trim(new char[] { '{', '}' });
                            string[] m = a.Split(',');
                            for (int f = 0; f < m.Length; f++)
                            {
                                string first_part = m[f].Split(":")[0].Replace(" ", "").Replace("\"", "").Replace(" ", "").ToString();
                                string second_part = m[f].Split(":")[1].Replace("'", "").Replace("{", "").Replace("}", "").Replace("\"", "").Split(" ")[1].Replace(" ", "").ToString();
                                string[] transitions = second_part.Split(" ");
                                //Console.WriteLine(first_part + " " + second_part);

                                if (string.IsNullOrWhiteSpace(first_part))
                                    automata.States[i].transitions.Add("", transitionStates);
                                else
                                    //Console.WriteLine(second_part.Replace("\"", ""));
                                    automata.States[i].transitions.Add(second_part.Replace("\"", ""), transitionStates);


                            }
                        }
                    }

                    break;
                case "Concat":
                    //2 ta fa bayad begirim
                    // code block
                    break;
                case "Union":
                    //2 ta fa bayad begirim hatman
                    // code block
                    break;
                default:
                    // code block
                    break;
            }

        }
    }
}