namespace FiniteStateAutomaton.Models;
public record GuiMenuPages
{
    public static readonly Dictionary<string, List<string>> MenuItems = new()
    {
        {
            "MainMenu",
            [
                "Load Automaton",
                "Print Automaton",
                "Check the word",
                "Load Automaton from file",
                "Save Automaton to file",
                "Load example automaton",
                "Delete epsilon transitions",
                "Convert NFA to DFA",
                "Exit"
            ]
        },
        {
            "ChooseAutomatonType", 
            [
                "DFA", 
                "NFA"
            ]
        },
        {
            "ChooseNFAtype", 
            [
                "NFA with epsilon transitions", 
                "NFA without epsilon transitions"
            ]
        },
        {
            "ExampleAutomaton", 
            [
                "DFA Example", 
                "NFA Example"
            ]
        }
    };
}