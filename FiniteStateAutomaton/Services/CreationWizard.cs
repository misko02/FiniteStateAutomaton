using FiniteStateAutomaton.Models;

namespace FiniteStateAutomaton.Services;

internal class CreationWizard
{
    private CreationWizard() { }
    public static CreationWizard Wizard { get; } = new();
    
    private Automaton _currentAutomaton = null!;
    
    public Automaton CurrentAutomaton
    {
        get
        {   
            if (_currentAutomaton == null)
            {
                throw new InvalidOperationException("No automaton type has been chosen.");
            }
            if (_currentAutomaton.States.Count == 0)
            {
                throw new InvalidOperationException("Automaton has no states defined.");
            }
            if (_currentAutomaton.Sigma.Count == 0)
            {
                throw new InvalidOperationException("Automaton has no alphabet defined.");
            }
            if (string.IsNullOrEmpty(_currentAutomaton.InitialState))
            {
                throw new InvalidOperationException("Automaton has no initial state defined.");
            }
            if (_currentAutomaton.FinalStates.Count == 0)
            {
                throw new InvalidOperationException("Automaton has no final states defined.");
            }
            if (_currentAutomaton.Delta.Count == 0)
            {
                throw new InvalidOperationException("Automaton has no transitions defined.");
            }
            return _currentAutomaton;
        }
    }
    public void ChooseAutomatonType(string type) 
    {
        switch (type)
        {
            case "DFA": _currentAutomaton = new DFA(); break;
            case "Epsilon NFA": _currentAutomaton = new NFA(); _currentAutomaton.Sigma.Add("Epsilon"); break;
            case "NFA": _currentAutomaton = new NFA(); break;
            default:
                Console.WriteLine("Invalid automaton type.");
                return;
        }
    }
    internal void DefineStates(List<string> states)
    {
        if (_currentAutomaton == null)
        {
            throw new InvalidOperationException("No automaton type has been chosen.");
        }
        foreach (var state in states)
        {
            _currentAutomaton.States.Add(state);
        }
    }
    internal void DefineAlphabet(List<string> alphabet)
    {
        foreach (var symbol in alphabet)
        {
            _currentAutomaton.Sigma.Add(symbol);
        }
    }
    internal void DefineInitialState(string initialState)
    {
        if (!_currentAutomaton.States.Contains(initialState))
        {
            throw new ArgumentException("Initial state must be one of the defined states.");
        }
        _currentAutomaton.MarkAsInitial(initialState);
    }
    internal void DefineFinalStates(List<string> finalStates)
    {
        foreach (var state in finalStates)
        {
            if (!_currentAutomaton.States.Contains(state))
            {
                throw new ArgumentException($"Final state '{state}' must be one of the defined states.");
            }
            _currentAutomaton.FinalStates.Add(state);
        }
    }
    
    internal void DefineTransitions(List<Tuple<string, string, string>> transitions)
    {
        foreach (var transition in transitions)
        {
            var fromState = transition.Item1;
            var symbol = transition.Item2;
            var toState = transition.Item3;
            try
            {
                _currentAutomaton.AddTransition(fromState, toState, symbol);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    internal void SaveAutomatonToFile(string filepath, Automaton automaton)
    {
        automaton.SaveToFile(filepath);
    }

    internal Automaton LoadAutomatonFromFile(string filepath, string type)
    {
        if (!File.Exists(filepath))
        {
            throw new FileNotFoundException("Automaton file not found.", filepath);
        }

        try
        {
            switch (type)
            {
                case "DFA":
                    return new DFA(filepath);
                case "Epsilon NFA":
                    return new NFA(filepath) { Sigma = { "Epsilon" } };
                case "NFA":
                    return new NFA(filepath);
                default:
                    throw new ArgumentException("Invalid automaton type specified.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    internal Automaton LoadExampleAutomaton(string filename)
    {
        switch(filename)
        {
            case "ZipCodeAutomaton.txt": return new DFA(filename);
            case "BinaryAutomaton.txt": return new NFA(filename);
            case "NumberParser.txt": return new NFA(filename);
            default:
                try
                {
                    return new NFA(filename);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found: " + filename);
                }
                return null!;   //for sure to change
        }
    }
}