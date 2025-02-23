namespace FiniteStateAutomaton;

/// <summary>
/// Class representing Non-deterministic Finite Automaton
/// </summary>
internal class NFA: Automaton
{

    /// <summary>
    /// Constant value representing epsilon transition
    /// </summary>
    public const string Epsilon = "e";

    /// <summary>
    /// Checks if automaton has epsilon transitions
    /// </summary>
    public bool HasEpsilonTransitions => Sigma.Contains(Epsilon);

    /// <summary>
    /// Default constructor
    /// </summary>
    public NFA()
    {
        States = [];
        Sigma = [];
        Delta = [];
    }
    /// <summary>
    /// Constructor creating NFA from file
    /// </summary>
    /// <param name="filename">nazwa pliku</param>
    public NFA(string filename)
    {
        string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        string path = Path.Combine(projectDirectory, "Automatons", filename);
        if (!File.Exists(path))
        {
            Console.WriteLine("File doesn't exist");
            return;
        }
        string[] lines = File.ReadAllLines(path);
        States = [];
        Sigma = [];
        Delta = [];
        States.Add("q" + lines[0][0]);
        InitialState = "q" + lines[0][0];
        foreach (string line in lines)
        {
            if (line.Length == 1)
            {
                MarkAsFinal("q" + line[0]);
            }
            else
            {
                var parts = line.Split(" ");
                var initialState = parts[0];
                var targetState = parts[1];
                var symbol = parts[2];
                if (!States.Contains("q" + initialState))
                    AddState("q" + initialState);
                if (!States.Contains("q" + targetState))
                    AddState("q" + targetState);
                if(symbol == "<eps>")
                {
                    AddEpsilonTransition("q" + initialState, "q" + targetState);
                }
                else
                {
                    Sigma.Add(symbol);
                    AddTransition("q" + initialState, "q" + targetState, symbol);
                }
            }
        }
    }

    /// <summary>
    /// Adds epsilon transition
    /// </summary>
    /// <param name="initialState">initial state</param>
    /// <param name="destinationState">destination State</param>
    /// <exception cref="ArgumentException">Wrong argument</exception>
    public void AddEpsilonTransition(string initialState, string destinationState)
    {
        if(!States.Contains(initialState))
        {
            throw new ArgumentException("Initial state doesn't exist in automaton's set of states");
        }
        if (!States.Contains(destinationState))
        {
            throw new ArgumentException("Destination state doesn't exist in automaton's set of states");
        }
        if (!Delta.ContainsKey((initialState, Epsilon)))
        {
            Delta[(initialState, Epsilon)] = new HashSet<string>();
        }
        if(!Sigma.Contains(Epsilon))
        {
            Sigma.Add(Epsilon);
        }
        Delta[(initialState, Epsilon)].Add(destinationState);
    }
    /// <summary>
    /// Calculates epsilon closure for state
    /// </summary>
    /// <param name="state">State we want to calculate epsilon state from</param>
    /// <returns> Set of states that's included in epsilon closure of state </returns>
    public HashSet<string> GetEpsilonClosure(string state)
    {
        var closure = new HashSet<string> { state };
        var stack = new Stack<string>();
        stack.Push(state);
        while (stack.Count > 0)
        {
            var currentState = stack.Pop();
            if (Delta.ContainsKey((currentState, Epsilon)))
            {
                foreach (var nextState in Delta[(currentState, Epsilon)])
                {
                    if (!closure.Contains(nextState))
                    {
                        closure.Add(nextState);
                        stack.Push(nextState);
                    }
                }
            }
        }
        return closure;
    }
    /// <summary>
    /// Calculate epsilon closure for set of states
    /// </summary>
    /// <param name="stateSet"> Set of states we want to get epsilon closure </param>
    /// <returns>Set of states that;s included in epsilon closure of states set</returns>
    public HashSet<string> GetEpsilonClosure(HashSet<string> stateSet) {
        var closure = new HashSet<string>();
        foreach (var state in stateSet)
        {
            closure.UnionWith(GetEpsilonClosure(state));
        }
        return closure;
    }
    /// <summary>
    /// Removes epsilon transitions from NFA
    /// </summary>
    public NFA RemoveEpsilonTransitions(){
        var nfaWithoutEpsilon = new NFA();
        nfaWithoutEpsilon.States = States;
        nfaWithoutEpsilon.InitialState = InitialState;
        nfaWithoutEpsilon.FinalStates = FinalStates;
        nfaWithoutEpsilon.Sigma = Sigma.Where(i => i != Epsilon).ToHashSet();
        nfaWithoutEpsilon.Delta = new Dictionary<(string, string), HashSet<string>>();
        
        var epsilonClosures = new Dictionary<string,HashSet<string>>();
        foreach (var state in nfaWithoutEpsilon.States)
        {
            epsilonClosures[state] = GetEpsilonClosure(state);
        }
        foreach (var state in nfaWithoutEpsilon.States)
        {
            foreach (var symbol in nfaWithoutEpsilon.Sigma)
            {

                var nextStates = new HashSet<string>();
                foreach (var epsilonState in epsilonClosures[state])
                {
                    if (Delta.ContainsKey((epsilonState, symbol)))
                    {
                        nextStates.UnionWith(Delta[(epsilonState, symbol)]);
                    }
                }
                foreach(var nextState in nextStates)
                {
                    foreach (var nextEpsilonState in GetEpsilonClosure(nextState))
                    {
                        nfaWithoutEpsilon.AddTransition(state, nextEpsilonState, symbol);
                    }
                }
            }
        }


        return nfaWithoutEpsilon;
    }

    /// <summary>
    /// Function checking if automaton accepts given word
    /// </summary>
    /// <param name="word"> Checked word </param>
    /// <returns>Whether word is accepted by automaton or not </returns>
    public override bool Accepts(string word)
    {
        var currentStates = GetEpsilonClosure(InitialState) ;
        foreach (var symbol in word)
        {
            if (!Sigma.Contains(symbol.ToString()))
            {
                return false;
            }

            var nextStates = new HashSet<string>();
            foreach (var state in currentStates)
            {
                if (Delta.ContainsKey((state, symbol.ToString())))
                {
                    nextStates.UnionWith(Delta[(state, symbol.ToString())]);
                }
            }

            currentStates = nextStates;
        }
        if (currentStates.Any(state => FinalStates.Contains(state)))
        {
            return true;
        }
        return false;
    }
}
