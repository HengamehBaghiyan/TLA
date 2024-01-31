using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLA_Library;
namespace Phase4
{
    public class FAOperations
    {
        public static NFAWithSingleFinalState NFA_Single_FinalState(NFA nfa)
        {
            //make a new NFA with single final state 
            NFAWithSingleFinalState new_nfa_withSingleFinalState;

            //if input nfa has less than two final states --> Do Nothing
            //else construct new state az new final state and connect previous final states to the new one with lambda transition and make previous final states non-final
            if (nfa.FinalStates.Count <= 1)
                new_nfa_withSingleFinalState = new NFAWithSingleFinalState(nfa.States, nfa.InputSymbols.inputs, nfa.InitialState, nfa.FinalStates[0]);
            else
            {
                //constrcut new final state and fix state ID
                State new_finalState = new State("q" + nfa.States.Count().ToString());

                //connect all final states 
                List<State> t_previousFinalState = new List<State>();
                t_previousFinalState.Add(new_finalState);

                //check whether previous final states have lambda transition to other states if yes --> update transitions if no --> construct new one
                for (int i = 0; i < nfa.FinalStates.Count; i++)
                {
                    if (nfa.FinalStates[i].transitions.ContainsKey(""))
                        nfa.FinalStates[i].transitions[""].Add(new_finalState);
                    else
                        nfa.FinalStates[i].transitions.Add("", t_previousFinalState);
                }
                //define new NFA with single final state with it's fields
                new_nfa_withSingleFinalState = new NFAWithSingleFinalState(nfa.States, nfa.InputSymbols.inputs, nfa.InitialState, new_finalState);
            }
            return new_nfa_withSingleFinalState;
        }
        public static NFAWithSingleFinalState Star(NFA nfa)
        {

            //check whether input NFA has more than one final state if yes --> build a NFA with single final state if no --> Do Nothing
            NFAWithSingleFinalState new_nfa_single_final = NFA_Single_FinalState(nfa);

            //construct new initial state for NFA after star Operation
            State new_initialState = new State("q" + (new_nfa_single_final.States.Count).ToString());
            //construct new final state for NFA after star Operation
            State new_finalState = new State("q" + (new_nfa_single_final.States.Count + 1).ToString());

            //add these 2 new states to NFA with single final state 
            new_nfa_single_final.States.Add(new_initialState);
            new_nfa_single_final.States.Add(new_finalState);

            //connect new initial state to previous initial state and new final state with lambdaa transition and change NFA's intial state
            List<State> t_newinitialState = new List<State>();
            t_newinitialState.Add(nfa.InitialState);
            t_newinitialState.Add(new_finalState);
            new_nfa_single_final.InitialState = new_initialState;
            new_nfa_single_final.InitialState.transitions.Add("", t_newinitialState);

            //connect previous final state to new final state with lambda transition
            //check whether previous final states have lambda transition to other states if yes --> update transitions if no --> construct new one
            if (new_nfa_single_final.FinalState.transitions.ContainsKey(""))
                new_nfa_single_final.FinalState.transitions[""].Add(new_finalState);
            else
            {
                List<State> t_previousfinalState = new List<State>();
                t_previousfinalState.Add(new_finalState);
                new_nfa_single_final.FinalState.transitions.Add("", t_previousfinalState);
            }

            //update NFA's final state with the new one we constructed
            new_nfa_single_final.FinalState = new_finalState;

            //connect new final state to new initial state with lambda transition 
            List<State> t_newfinalState = new List<State>();
            t_newfinalState.Add(new_initialState);
            new_nfa_single_final.FinalState.transitions.Add("", t_newfinalState);

            return new_nfa_single_final;
        }
        public static NFAWithSingleFinalState Concat(NFA f_NFA, NFA s_NFA)
        {
            //Convert these 2 input NFAS to NFAS with single final state if they have more than one final state
            NFAWithSingleFinalState f_NFA_S = NFA_Single_FinalState(f_NFA);
            NFAWithSingleFinalState s_NFA_S = NFA_Single_FinalState(s_NFA);

            //connect first NFA's final state to second NFA's initial state 
            //if first NFA's final state has lambda transition to other states if yes --> update transitions if no --> construct new one 
            if (f_NFA_S.FinalState.transitions.ContainsKey(""))
                f_NFA_S.FinalState.transitions[""].Add(s_NFA_S.InitialState);
            else
            {
                List<State> f_NFA_pre_final_first = new List<State>();
                f_NFA_pre_final_first.Add(s_NFA_S.InitialState);
                f_NFA_S.FinalState.transitions.Add("", f_NFA_pre_final_first);
            }

            //construct new final state for NFA after concat operation 
            State new_finalState = new State("q" + (s_NFA_S.States.Count + f_NFA_S.States.Count - 2));

            //add new final state to second NFA before concatenation
            s_NFA_S.States.Add(new_finalState);


            //connect second NFA's final state to new final state with lambda transition
            //check whether previous final states have lambda transition to other states if yes --> update transitions if no --> construct new one
            if (s_NFA_S.FinalState.transitions.ContainsKey(""))
                s_NFA_S.FinalState.transitions[""].Add(new_finalState);
            else
            {
                List<State> t_previousFinalState = new List<State>();
                t_previousFinalState.Add(new_finalState);
                s_NFA_S.FinalState.transitions.Add("", t_previousFinalState);
            }
            //change second NFA's final state --> update it to new final state
            s_NFA_S.FinalState = new_finalState;

            //concat second NFA to First NFA 
            //change state IDs for second NFA
            //add second NFA's states + transitions to first NFA
            for (int i = 0; i < s_NFA_S.States.Count; i++)
            {
                s_NFA_S.States[i].state_ID = "q" + (i + s_NFA_S.States.Count - 2).ToString();
                f_NFA_S.States.Add(s_NFA_S.States[i]);
            }
            //change result NFA's final state --> update it to second NFA's final state
            f_NFA_S.FinalState = s_NFA_S.FinalState;

            //add second NFA's input symbols to the result or first NFA --> if common input symbols --> Do nothing else --> add new ones
            for (int i = 0; i < s_NFA_S.InputSymbols.inputs.Count; i++)
            {
                if (!f_NFA_S.InputSymbols.inputs.Contains(s_NFA_S.InputSymbols.inputs[i]))
                    f_NFA_S.InputSymbols.inputs.Add(s_NFA_S.InputSymbols.inputs[i].ToString());
            }
            //return first NFA after changes as the concated one
            return f_NFA_S;
        }

        public static NFAWithSingleFinalState Union(NFA f_NFA, NFA s_NFA)
        {
            //construct new NFA with single final state for result
            NFAWithSingleFinalState united;

            //Convert these 2 input NFAS to NFAS with single final state if they have more than one final state
            NFAWithSingleFinalState f_NFA_S = NFA_Single_FinalState(f_NFA);
            NFAWithSingleFinalState s_NFA_S = NFA_Single_FinalState(s_NFA);

            // construct new initial state for NFA after union Operation and define new stateID
            State new_intialState = new State("q" + (f_NFA_S.States.Count + s_NFA_S.States.Count));

            //construct new final state for NFA after star Operation and define new stateID
            State new_finalState = new State("q" + (f_NFA_S.States.Count + s_NFA_S.States.Count) + 1);

            //connect first NFA's final state to new final state with lamda transition
            //check whether previous final states have lambda transition to other states if yes --> update transitions if no --> construct new one
            if (f_NFA_S.FinalState.transitions.ContainsKey(""))
                f_NFA_S.FinalState.transitions[""].Add(new_finalState);
            else
            {
                List<State> first_fa_new_transiton = new List<State>();
                first_fa_new_transiton.Add(new_finalState);
                f_NFA_S.FinalState.transitions.Add("", first_fa_new_transiton);
            }
            //connect second NFA's final state to new final state with lamda transition
            //check whether previous final states have lambda transition to other states if yes --> update transitions if no --> construct new one
            if (s_NFA_S.FinalState.transitions.ContainsKey(""))
                s_NFA_S.FinalState.transitions[""].Add(new_finalState);
            else
            {
                List<State> second_fa_new_transiton = new List<State>();
                second_fa_new_transiton.Add(new_finalState);
                s_NFA_S.FinalState.transitions.Add("", second_fa_new_transiton);
            }

            //connect new constructed initial state to first NFA's initial state and second NFA's initial state
            List<State> new_initial_state_transitions = new List<State>();
            new_initial_state_transitions.Add(f_NFA_S.InitialState);
            new_initial_state_transitions.Add(s_NFA_S.InitialState);
            new_intialState.transitions.Add("", new_initial_state_transitions);

            //add new final state and new initial state to second NFA before union operation
            s_NFA_S.States.Add(new_intialState);
            s_NFA_S.States.Add(new_finalState);

            //change second NFA's states IDs 
            for (int i = 0; i < s_NFA_S.States.Count; i++)
                s_NFA_S.States[i].state_ID = "q" + (i + f_NFA_S.States.Count).ToString();

            //add second NFA's states to first one
            for (int i = 0; i < s_NFA_S.States.Count; i++)
                f_NFA_S.States.Add(s_NFA_S.States[i]);

            //add second NFA's input symbols to the result or first NFA --> if common input symbols --> Do nothing else --> add new ones
            for (int i = 0; i < s_NFA_S.InputSymbols.inputs.Count; i++)
            {
                if (!f_NFA_S.InputSymbols.inputs.Contains(s_NFA_S.InputSymbols.inputs[i]))
                    f_NFA_S.InputSymbols.inputs.Add(s_NFA_S.InputSymbols.inputs[i].ToString());
            }

            united = new NFAWithSingleFinalState(f_NFA_S.States, f_NFA_S.InputSymbols.inputs, new_intialState, new_finalState);
            return united;
        }
        public static void Main(string[] args)
        {
            string operation = Console.ReadLine();
            switch (operation.ToLower())
            {
                case "star":
                    {
                        var json_text = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/star/in/FA.json");
                        NFA nfa = ConvertJsonToFA.ConvertJsonToNFA(json_text);
                        string result_path = @"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/star/in/FAR.json";
                        ConvertFAToJson.ConvertNFAToJson(Star(nfa), result_path);
                        break;
                    }
                case "concat":
                    {
                        var json_text_1 = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/concat/in/FA1.json");
                        var json_text_2 = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/concat/in/FA2.json");
                        string result_path = @"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/concat/in/FAR.json";
                        NFA nfa_1 = ConvertJsonToFA.ConvertJsonToNFA(json_text_1);
                        NFA nfa_2 = ConvertJsonToFA.ConvertJsonToNFA(json_text_2);
                        ConvertFAToJson.ConvertNFAToJson(Concat(nfa_1, nfa_2), result_path);
                        break;
                    }
                case "union":
                    {
                        var json_text_1 = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/union/in/FA1.json");
                        var json_text_2 = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/union/in/FA2.json");
                        string result_path = @"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase4-sample/union/in/FAR.json";
                        NFA nfa_1 = ConvertJsonToFA.ConvertJsonToNFA(json_text_1);
                        NFA nfa_2 = ConvertJsonToFA.ConvertJsonToNFA(json_text_2);
                        ConvertFAToJson.ConvertNFAToJson(Union(nfa_1, nfa_2), result_path);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
