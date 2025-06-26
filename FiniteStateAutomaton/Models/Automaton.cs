namespace FiniteStateAutomaton;

internal abstract class Automaton
{
    private Dictionary<(string, string), HashSet<string>> _delta = [];
    private HashSet<string> _finalStates = [];
    private string _initialState = "";

    /// <summary>
    ///     Automaton states
    /// </summary>
    public virtual HashSet<string> States { get; protected set; }

    /// <summary>
    ///     Number of states in automaton
    /// </summary>
    public virtual int NumberOfStates => States.Count;

    /// <summary>
    ///     Alphabet of automaton
    /// </summary>
    public virtual HashSet<string> Sigma { get; protected set; }

    /// <summary>
    ///     Function of transition, as key pair (state, symbol), as set of target states
    /// </summary>
    public virtual Dictionary<(string, string), HashSet<string>> Delta
    {
        get => _delta;
        protected set
        {
            foreach (var key in value.Keys)
            {
                if (!States.Contains(key.Item1))
                    throw new ArgumentException("State doesn't exist in set of automaton's states");

                if (!Sigma.Contains(key.Item2))
                    throw new ArgumentException("Symbol doesn't exist in automaton's alphabet");

                foreach (var val in value[key])
                    if (!States.Contains(val))
                        throw new ArgumentException("State doesn't exist in set of automaton's states");
            }

            _delta = value;
        }
    }

    /// <summary>
    ///     Initial state of automaton
    /// </summary>
    public virtual string InitialState
    {
        get => _initialState;
        protected set
        {
            if (!States.Contains(value))
                throw new ArgumentException("State doesn't exist in set of automaton's states");

            _initialState = value;
        }
    }

    /// <summary>
    ///     Set of final states
    /// </summary>
    public virtual HashSet<string> FinalStates
    {
        get => _finalStates;
        protected set
        {
            foreach (var state in value)
                if (!States.Contains(state))
                    throw new ArgumentException($"State {state} doesn't exist in set of automaton's states");

            _finalStates = value;
        }
    }

    /// <summary>
    ///     Adds new state to automaton
    /// </summary>
    /// <param name="state"> State we want to add </param>
    /// <param name="transitions"> Transition function of the new state </param>
    /// <returns> Newly added state </returns>
    /// <exception cref="ArgumentException">State doesn't exist in set of automaton's states</exception>
    public string AddState(string? state = null, Dictionary<(string, string), HashSet<string>>? transitions = null)
    {
        // if state is not given, we assign it default value q{number of states}
        state ??= $"q{NumberOfStates}";
        States.Add(state);
        // If transition function is not given, we add transition for each symbol to empty set of states
        if (transitions is null)
            foreach (var symbol in Sigma)
                Delta[(state, symbol)] = [];
        else
            foreach (var key in transitions.Keys)
                try
                {
                    foreach (var val in transitions[key])
                    {
                        if (!States.Contains(val))
                            throw new ArgumentException("State doesn't exist in set of automaton's states");
                        AddTransition(key.Item1, val, key.Item2);
                    }
                }
                catch
                {
                    States.Remove(state);
                    throw;
                }

        return state;
    }

    /// <summary>
    ///     Marks state as initial
    /// </summary>
    /// <param name="state"> State from automaton's set of states we want to mark as initial</param>
    public void MarkAsInitial(string state)
    {
        if (!States.Contains(state)) AddState(state);
        InitialState = state;
    }

    /// <summary>
    ///     Marks state as final
    /// </summary>
    /// <param name="state"> State we want to mark as final </param>
    public void MarkAsFinal(string state)
    {
        if (!States.Contains(state)) AddState(state);

        FinalStates.Add(state);
    }

    /// <summary>
    ///     Adds a new transition to the automaton
    /// </summary>
    /// <param name="from"> initial state </param>
    /// <param name="to"> set of destination states </param>
    /// <param name="symbol"> transition's symbol </param>
    /// <exception cref="ArgumentException"> Wrong argument </exception>
    public void AddTransition(string from, string to, string symbol)
    {
        if (!States.Contains(from))
            throw new ArgumentException("Initial state doesn't exist in set of automaton's states");

        if (!Sigma.Contains(symbol))
            throw new ArgumentException("Symbol doesn't exist in automaton's alphabet");
        if (!States.Contains(to))
            throw new ArgumentException("Destination state doesn't exist in set of automaton's states");
        if (!Delta.ContainsKey((from, symbol))) Delta[(from, symbol)] = new HashSet<string>();
        Delta[(from, symbol)].Add(to);
    }

    /// <summary>
    ///     Returns set of states after transition
    /// </summary>
    /// <param name="from"> State we're coming from </param>
    /// <param name="symbol"> Transition's symbol </param>
    /// <returns> Set of states after transition</returns>
    /// <exception cref="ArgumentException"> Wrong argument </exception>
    public HashSet<string> GetNextState(string from, string symbol)
    {
        if (!States.Contains(from)) throw new ArgumentException("State doesn't exist in set of automaton's states");

        if (!Sigma.Contains(symbol)) throw new ArgumentException("Symbol nie istnieje w zbiorze alfabetu");
        if (!Delta.ContainsKey((from, symbol))) return new HashSet<string>();
        return Delta[(from, symbol)];
    }

    /// <summary>
    ///     Returns whether given state is final
    /// </summary>
    /// <param name="state"> Checked state </param>
    /// <returns> True if state is final, otherwise false</returns>
    public bool IsFinalState(string state)
    {
        return FinalStates.Contains(state);
    }

    /// <summary>
    ///     Saves automaton to file
    /// </summary>
    /// <param name="filename"> Name of newly created file </param>
    public void SaveToFile(string filename)
    {
        var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        var path = Path.Combine(projectDirectory, "Automatons", filename);
        using var sw = File.CreateText(path);
        sw.WriteLine(InitialState[1]);
        foreach (var state in FinalStates) sw.WriteLine(state[1]);
        foreach (var key in Delta.Keys)
        foreach (var val in Delta[key])
            sw.WriteLine($"{key.Item1[1]} {val[1]} {key.Item2}");
    }

    /// <summary>
    ///     Checks if given word is accepted by automaton
    /// </summary>
    /// <param name="word"> word we want to check </param>
    /// <returns>True if word is accepted by automaton, false otherwise </returns>
    public abstract bool Accepts(string word);
}