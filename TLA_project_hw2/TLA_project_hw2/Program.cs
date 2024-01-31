using System;
using System.Linq;
using System.Collections.Generic;
class program
{
    //class alphabet_input
    //{
    //    public char letter;
    //    public alphabet_input(string x)
    //    {
    //        letter = x[0];
    //    }
    //}
    //class alphabet_stack
    //{
    //    public char letter;
    //    public alphabet_stack(string x)
    //    {
    //        letter = x[0];
    //    }
    //}
    class state
    {
        public string name_state;
        public Dictionary<string, List<Tuple<state, string>>> transition = new Dictionary<string, List<Tuple<state, string>>>();
        public state(string x)
        {
            name_state = x;
        }
    }
    class PDA
    {
        public List<state> states = new List<state>();
        public List<char> alaphabet_inputs = new List<char>();
        public List<char> alaphabet_stacks = new List<char>();
        public List<state> final_states = new List<state>();
        public PDA(string[] ss, string[] ai, string[] aas, string[] fs)
        {
            for (int i = 0; i < ss.Length; i++)
            {
                state temp = new state(ss[i]);
                states.Add(temp);
                if (fs.Contains(ss[i]))
                {
                    final_states.Add(temp);
                }
            }
            for (int i = 0; i < ai.Length; i++)
            {
                alaphabet_inputs.Add(ai[i][0]);
            }
            alaphabet_inputs.Add('#');
            for (int i = 0; i < aas.Length; i++)
            {
                alaphabet_stacks.Add(aas[i][0]);
            }
            alaphabet_stacks.Add('#');
            alaphabet_stacks.Add('$');
        }
    }
    static bool accepted(string string_input, state s, Stack<char> stack, int i, PDA pda)
    {
        if (i == string_input.Length)
        {
            if (pda.final_states.Contains(s))
                return true;
        }
        if (stack.Count == 0)
            return false;
        Stack<char> st1 = new Stack<char>();
        Stack<char> st2 = new Stack<char>();
        Stack<char> st3 = new Stack<char>();
        Stack<char> st4 = new Stack<char>();
        List<char> tempp = new List<char>();
        int n = stack.Count;
        for (int j = 0; j <n; j++)
        {
            tempp.Add(stack.Pop());
        }
        for (int j = n - 1; j >= 0; j--)
        {
            st1.Push(tempp[j]);
            st2.Push(tempp[j]);
            st3.Push(tempp[j]);
            st4.Push(tempp[j]);
            //st_temp.Push(tempp[j]);
        }
        char temp = tempp[0];
        string s1 = "#," + temp;
        string s2 = "#,#";

        string s3 = "";
        string s4 = "";
        if (i < string_input.Length)
        {
            s3 = string_input[i] + ",#";
            s4 = string_input[i] + "," + temp;
        }
        bool b = false;
        if (s.transition.ContainsKey(s1))
        {
            st1.Pop();
            for (int j = 0; j < s.transition[s1].Count; j++)
            {
                if (s.transition[s1][j].Item2 != "#")
                {
                    for(int k = s.transition[s1][j].Item2.Length-1; k>=0; k--)
                    {
                        st1.Push(s.transition[s1][j].Item2[k]);
                    }
                }
                b = accepted(string_input, s.transition[s1][j].Item1, st1, i, pda);
                st1.Clear();
                for (int k = tempp.Count - 1; k >= 1; k--)
                {
                    st1.Push(tempp[k]);
                }
                if (b)
                    return true;
            }
        }
        if (s.transition.ContainsKey(s2))
        {
            for (int j = 0; j < s.transition[s2].Count; j++)
            {
                if (s.transition[s2][j].Item2 != "#")
                {
                    for (int k = s.transition[s2][j].Item2.Length - 1; k >= 0; k--)
                    {
                        st2.Push(s.transition[s2][j].Item2[k]);
                    }
                }
                b = accepted(string_input, s.transition[s2][j].Item1, st2, i, pda);
                st2.Clear();
                for (int k = tempp.Count - 1; k >= 0; k--)
                {
                    st2.Push(tempp[k]);
                }
                if (b)
                    return true;
            }
        }
        if (s.transition.ContainsKey(s3))
        {
            for (int j = 0; j < s.transition[s3].Count; j++)
            {
                if (s.transition[s3][j].Item2 != "#")
                {
                    for (int k = s.transition[s3][j].Item2.Length - 1; k >= 0; k--)
                    {
                        st3.Push(s.transition[s3][j].Item2[k]);
                    }
                }
                b = accepted(string_input, s.transition[s3][j].Item1, st3, i + 1, pda);
                st3.Clear();
                for (int k = tempp.Count - 1; k >= 0; k--)
                {
                    st3.Push(tempp[k]);
                }
                if (b)
                    return true;
            }
        }
        if (s.transition.ContainsKey(s4))
        {
            st4.Pop();
            for (int j = 0; j < s.transition[s4].Count; j++)
            {
                if (s.transition[s4][j].Item2 != "#")
                {
                    for (int k = s.transition[s4][j].Item2.Length - 1; k >= 0; k--)
                    {
                        st4.Push(s.transition[s4][j].Item2[k]);
                    }
                }
                b = accepted(string_input, s.transition[s4][j].Item1, st4, i + 1, pda);
                st4.Clear();
                for (int k = tempp.Count - 1; k >= 1; k--)
                {
                    st4.Push(tempp[k]);
                }
                if (b)
                    return true;
            }
        }
        return false;
    }
    static void Main()
    {
        string[] states = Console.ReadLine().Trim('{').Trim('}').Split(',');
        string[] alphabet_input = Console.ReadLine().Trim('{').Trim('}').Split(',');
        string[] alphabet_stack = Console.ReadLine().Trim('{').Trim('}').Split(',');
        string[] final_states = Console.ReadLine().Trim('{').Trim('}').Split(',');
        int n = int.Parse(Console.ReadLine());
        PDA pda = new PDA(states, alphabet_input, alphabet_stack, final_states);
        for (int i = 0; i < n; i++)
        {
            string[] input = Console.ReadLine().Split("),(");
            string[] one = input[0].Trim('(').Split(',');
            string[] tow = input[1].Trim(')').Split(',');
            int find1 = -1;
            int find2 = -1;
            for (int j = 0; j < pda.states.Count; j++)
            {
                if (pda.states[j].name_state == one[0])
                {
                    find1 = j;
                }
                if (pda.states[j].name_state == tow[1])
                {
                    find2 = j;
                }
            }
            if (pda.states[find1].transition.ContainsKey(one[1] + "," + one[2]))
            {
                pda.states[find1].transition[one[1] + "," + one[2]].Add(new Tuple<state, string>(pda.states[find2], tow[0]));
            }
            else
            {
                List<Tuple<state, string>> temp = new List<Tuple<state, string>>();
                temp.Add(new Tuple<state, string>(pda.states[find2], tow[0]));
                pda.states[find1].transition.Add(one[1] + "," + one[2], temp);
            }
        }
        string string_inpur = Console.ReadLine();
        Stack<char> stack = new Stack<char>();
        stack.Push('$');
        string_inpur = string_inpur.Replace("#", "");
        if (accepted(string_inpur, pda.states[0], stack, 0, pda))
            Console.WriteLine("Accepted");
        else
            Console.WriteLine("Rejected");
    }
}
