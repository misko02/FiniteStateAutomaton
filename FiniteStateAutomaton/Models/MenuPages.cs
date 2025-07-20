using FiniteStateAutomaton.Enums;
using static FiniteStateAutomaton.Enums.Pages;

namespace FiniteStateAutomaton.Models;
internal record MenuPages
{
    public static readonly Dictionary<Pages, List<string>> MenuItems = new()
    {
        {
            MainMenu,
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
            ChooseAutomatonType, 
            [
                "DFA", 
                "NFA"
            ]
        },
        {
            ChooseNFAtype,  
            [
                "NFA with epsilon transitions", 
                "NFA without epsilon transitions"
            ]
        },
        {
            ExampleAutomatonsMenu,
            [
                "Zip Code automaton checker",       // Example of DFA automaton
                "Binary string checker",            // Example of NFA automaton
                "Floating point number checker"     //Example of automaton with epsilon transitions
            ]
        }
    };
}