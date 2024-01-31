using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using TLA_Library;

class program
{
    static void DFS_Visit_Recursive(DFA dfa, NewState n, List<NewState> dfs_visit)
    {
        n.Visit = true;
        for (int i = 0; i < dfa.InputSymbols.inputs.Count; i++)
            if (n.transitions[dfa.InputSymbols.inputs[i]].Visit == false)
            {
                dfs_visit.Add(n.transitions[dfa.InputSymbols.inputs[i]]);
                DFS_Visit_Recursive(dfa, n.transitions[dfa.InputSymbols.inputs[i]], dfs_visit);
            }
    }
    public static DFA SimplificationDFA(DFA dfa)
    {
        List<NewState> dfs_visit = new List<NewState>();
        dfs_visit.Add(dfa.InitialState);
        DFS_Visit_Recursive(dfa, dfa.InitialState, dfs_visit);
        dfa.States = dfs_visit;
        List<NewState> new_finil_state = new List<NewState>();
        for (int i = 0; i < dfa.FinalStates.Count; i++)
        {
            if (dfs_visit.Contains(dfa.FinalStates[i]))
                new_finil_state.Add(dfa.FinalStates[i]);
        }
        dfa.FinalStates = new_finil_state;
        List<NewState> finil = new List<NewState>();
        List<NewState> nonfinil = new List<NewState>();
        for (int i = 0; i < dfa.States.Count; i++)
        {
            if (dfa.FinalStates.Contains(dfa.States[i]))
            {
                finil.Add(dfa.States[i]);
            }
            else
                nonfinil.Add(dfa.States[i]);
        }

        List<List<NewState>> equal_state_k = new List<List<NewState>>();
        equal_state_k.Add(finil);
        equal_state_k.Add(nonfinil);
        while (true)
        {
            List<List<NewState>> equal_state_k_next = new List<List<NewState>>();
            for (int i = 0; i < equal_state_k.Count; i++)
            {
                Dictionary<string, List<NewState>> product_equal_states = new Dictionary<string, List<NewState>>();
                for (int j = 0; j < equal_state_k[i].Count; j++)
                {
                    string x = "";
                    if (dfa.InputSymbols.inputs.Count == 2)
                    {
                        for (int k = 0; k < equal_state_k.Count; k++)
                        {
                            if (equal_state_k[k].Contains(equal_state_k[i][j].transitions[dfa.InputSymbols.inputs[0]]))
                            {
                                x += equal_state_k[k][0].state_ID;
                                break;
                            }
                        }
                        for (int k = 0; k < equal_state_k.Count; k++)
                        {
                            if (equal_state_k[k].Contains(equal_state_k[i][j].transitions[dfa.InputSymbols.inputs[1]]))
                            {
                                x += equal_state_k[k][0].state_ID;
                                break;
                            }
                        }
                    }
                    else if (dfa.InputSymbols.inputs.Count == 1)
                    {
                        for (int k = 0; k < equal_state_k.Count; k++)
                        {
                            if (equal_state_k[k].Contains(equal_state_k[i][j].transitions[dfa.InputSymbols.inputs[0]]))
                            {
                                x += equal_state_k[k][0].state_ID;
                                break;
                            }
                        }
                    }
                    if (product_equal_states.ContainsKey(x))
                    {
                        product_equal_states[x].Add(equal_state_k[i][j]);
                    }
                    else
                    {
                        List<NewState> temp = new List<NewState>();
                        temp.Add(equal_state_k[i][j]);
                        product_equal_states.Add(x, temp);
                    }
                }
                foreach (var tmp in product_equal_states.Values)
                {
                    equal_state_k_next.Add(tmp);
                }

            }

            if (equal_state_k_next.Count == equal_state_k.Count)
                break;
            equal_state_k = equal_state_k_next;
        }
        for (int i = 0; i < equal_state_k.Count; i++)
        {
            equal_state_k[i] = equal_state_k[i].OrderBy(x => x.state_ID.Replace("q", "")).ToList();
        }
        equal_state_k = equal_state_k.OrderBy(x => x[0].state_ID.Replace("q", "")).ToList();
        List<NewState> new_dfa_states = new List<NewState>();
        for (int i = 0; i < equal_state_k.Count; i++)
        {
            string x = "";
            for (int j = 0; j < equal_state_k[i].Count; j++)
            {
                x += equal_state_k[i][j].state_ID;
            }
            new_dfa_states.Add(new NewState(x));
        }
        List<NewState> new_dfa_finil_states = new List<NewState>();
        for (int i = 0; i < equal_state_k.Count; i++)
        {
            if (dfa.FinalStates.Contains(equal_state_k[i][0]))
            {
                new_dfa_finil_states.Add(new_dfa_states[i]);
            }
        }
        NewState new_dfa_initial_state = null;
        for (int i = 0; i < equal_state_k.Count; i++)
        {
            if (equal_state_k[i].Contains(dfa.InitialState))
            {
                new_dfa_initial_state = new_dfa_states[i];
                break;
            }
        }
        DFA new_dfa = new DFA(0, new_dfa_states, dfa.InputSymbols, new_dfa_initial_state, new_dfa_finil_states);
        if (dfa.InputSymbols.inputs.Count == 2)
        {
            for (int i = 0; i < equal_state_k.Count; i++)
            {
                for (int k = 0; k < equal_state_k.Count; k++)
                {
                    if (equal_state_k[k].Contains(equal_state_k[i][0].transitions[dfa.InputSymbols.inputs[0]]))
                    {
                        new_dfa_states[i].transitions[dfa.InputSymbols.inputs[0]] = new_dfa_states[k];
                        break;
                    }
                }
                for (int k = 0; k < equal_state_k.Count; k++)
                {
                    if (equal_state_k[k].Contains(equal_state_k[i][0].transitions[dfa.InputSymbols.inputs[1]]))
                    {
                        new_dfa_states[i].transitions[dfa.InputSymbols.inputs[1]] = new_dfa_states[k];
                        break;
                    }
                }
            }
        }
        else if (dfa.InputSymbols.inputs.Count == 1)
        {
            for (int i = 0; i < equal_state_k.Count; i++)
            {
                for (int k = 0; k < equal_state_k.Count; k++)
                {
                    if (equal_state_k[k].Contains(equal_state_k[i][0].transitions[dfa.InputSymbols.inputs[0]]))
                    {
                        new_dfa_states[i].transitions[dfa.InputSymbols.inputs[0]] = new_dfa_states[k];
                        break;
                    }
                }
            }
        }
        return new_dfa;
    }
    public static void Main(string[] args)
    {
        var text = File.ReadAllText(@"C:/Users/ASUS/Desktop/TLA01-Projects-main/samples/phase2-sample/in/input1.json");
        DFA dfa = ConvertJsonToFA.ConvertJsonToDFA(text);

        DFA result = SimplificationDFA(dfa);
        ConvertFAToJson.ConvertDFAToJson(result);
    }
}
