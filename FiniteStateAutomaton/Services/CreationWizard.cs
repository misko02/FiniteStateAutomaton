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
}