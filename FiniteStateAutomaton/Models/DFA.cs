namespace FiniteStateAutomaton;

/// <summary>
///     Class representing Deterministic Finite Automaton
/// </summary>
internal class DFA : Automaton
{
    private readonly Dictionary<(string, string), HashSet<string>> _delta = [];

    /// <summary>
    ///     Default constructor
    /// </summary>
    public DFA()
    {
        States = ["q0"];
        Sigma = [];
        Delta = [];
        InitialState = "q0";
        FinalStates = [];
    }

    /// <summary>
    ///     Constructor creating DFA from file
    /// </summary>
    /// <param name="filename">filename of file we want to load automaton from</param>
    public DFA(string filename)
    {
        var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        var path = Path.Combine(projectDirectory, "Automatons", filename);
        if (!File.Exists(path))
        {
            Console.WriteLine("File doesn't exist");
            return;
        }

        var lines = File.ReadAllLines(path);
        States = [];
        Sigma = [];
        Delta = [];
        States.Add("q" + lines[0][0]);
        InitialState = "q" + lines[0][0];
        foreach (var line in lines)
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
                Sigma.Add(symbol);
                AddTransition("q" + initialState, "q" + targetState, symbol);
            }
    }

    /// <summary>
    ///     Constuctor creating DFA from NFA
    /// </summary>
    /// <param name="nfa"></param>
    public DFA(NFA nfa)
    {
        if (nfa.HasEpsilonTransitions) nfa = nfa.RemoveEpsilonTransitions();
        States = nfa.States;
        Sigma = nfa.Sigma;
        InitialState = nfa.InitialState;
        FinalStates = nfa.FinalStates;
        Delta = [];
        foreach (var state in nfa.States)
        foreach (var symbol in nfa.Sigma)
        {
            var targetStates = nfa.GetNextState(state, symbol);
            if (targetStates.Count == 0)
            {
                Delta[(state, symbol)] = new HashSet<string>();
                continue;
            }

            var newState = targetStates.Count > 1
                ? "q" + string.Join(",", targetStates.Select(q => q[1]))
                : "q" + targetStates.First()[1];
            if (!States.Contains(newState))
            {
                AddState(newState);
                if (targetStates.Any(IsFinalState)) MarkAsFinal(newState);
            }

            AddTransition(state, newState, symbol);
        }
    }

    /// <summary>
    ///     Transitions function for Deterministic Finite Automaton, as key pair (state, symbol), as value target state
    /// </summary>
    public override Dictionary<(string, string), HashSet<string>> Delta
    {
        get => _delta;
        protected set
        {
            foreach (var key in value.Keys)
            {
                if (!States.Contains(key.Item1))
                    throw new ArgumentException("Initial state doesn't exist in automaton's set of states");

                if (!Sigma.Contains(key.Item2))
                    throw new ArgumentException("Symbol doesn't exist in automaton's alphabet");
                if (value[key].Count > 1)
                    throw new ArgumentException("Deterministic automatons has only one initial state for each symbol");
                if (!States.Contains(value[key].First()))
                    throw new ArgumentException("Destination state doesn't exist in automaton's set of states");
            }
        }
    }

    /// <summary>
    ///     Function checking if automaton accepts given word
    /// </summary>
    /// <param name="word">Checked word</param>
    /// <returns> Whether automaton accepts word or not</returns>
    public override bool Accepts(string word)
    {
        var currentState = InitialState;
        foreach (var symbol in word)
        {
            if (!Sigma.Contains(symbol.ToString())) return false;
            currentState = Delta[(currentState, symbol.ToString())].First();
        }

        return IsFinalState(currentState);
    }
}