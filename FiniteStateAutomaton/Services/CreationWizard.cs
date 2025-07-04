namespace FiniteStateAutomaton.Services;

public class CreationWizard
{
    protected CreationWizard() { }
    public static CreationWizard Wizard { get; } = new CreationWizard();
    
    private Automaton _currentAutomaton = null!;
    
    internal Automaton LoadAutomaton()
    {
        throw new NotImplementedException();
    }
    private void ChooseAutomatonType() 
    {
        throw new NotImplementedException();
    }
    private void DefineStates()
    {
        throw new NotImplementedException();
    }
    private void DefineAlphabet()
    {
        throw new NotImplementedException();
    }
    private void DefineInitialState()
    {
        throw new NotImplementedException();
    }
    private void DefineFinalStates()
    {
        throw new NotImplementedException();
    }
    private void DefineTransitions()
    {
        throw new NotImplementedException();
    }
    private void SaveAutomatonToFile()
    {
        throw new NotImplementedException();
    }
    private void LoadAutomatonFromFile()
    {
        throw new NotImplementedException();
    }
    private void LoadExampleAutomaton()
    {
        throw new NotImplementedException();
    }
}