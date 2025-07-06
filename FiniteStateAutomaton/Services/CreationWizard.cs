using FiniteStateAutomaton.Models;

namespace FiniteStateAutomaton.Services;

public class CreationWizard
{
    private CreationWizard() { }
    public static CreationWizard Wizard { get; } = new();
    
    private Automaton _currentAutomaton = null!;
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
        throw new NotImplementedException();
    }
    public void DefineStates(List<string> states)
    {
        throw new NotImplementedException();
    }
    public void DefineAlphabet()
    {
        throw new NotImplementedException();
    }
    public void DefineInitialState()
    {
        throw new NotImplementedException();
    }
    public void DefineFinalStates()
    {
        throw new NotImplementedException();
    }
    public void DefineTransitions()
    {
        throw new NotImplementedException();
    }
    public void SaveAutomatonToFile()
    {
        throw new NotImplementedException();
    }
    public void LoadAutomatonFromFile()
    {
        throw new NotImplementedException();
    }
    public void LoadExampleAutomaton()
    {
        throw new NotImplementedException();
    }
}