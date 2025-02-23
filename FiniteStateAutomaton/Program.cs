List<string> menuOptions = [
    "Load Automaton",
    "Print Automaton", 
    "Check the word", 
    "Load Automaton from file", 
    "Save Automaton to file",
    "Load example automaton",
    "Delete epsilon transitions",
    "Convert NFA to DFA",
    "Exit" ];

var gui = new GUI();
Automaton automaton = new DFA();

while (gui.IsRunning)
{

    var option = gui.PrintMenu(menuOptions);

    if (option.HasValue)
    {
        switch (option.Value)
        {
            case 0:
                var automatonTypes = new List<Type> { typeof(DFA), typeof(NFA) };
                var automatonType = automatonTypes[gui.PrintMenu(automatonTypes.Select(x => x.Name).ToList()) ?? 0];
                automaton = gui.LoadAutomaton(automatonType);
                break;
            case 1:
                gui.PrintAutomaton(automaton);
                break;
            case 2:
                Console.WriteLine("Enter the word to check: ");
                var word = Console.ReadLine();
                Console.WriteLine(automaton.Accepts(word) ? "Automaton accepts this word" : "Automaton doesn't accept this word");
                break;
            case 3:
                Console.WriteLine("Enter the filename: ");
                var filename = Console.ReadLine();
                var loadedAutomaton = new NFA(filename);
                if (automaton is not null)
                {
                    Console.WriteLine("Automaton succesfully loaded.");
                    automaton = loadedAutomaton;
                }
                break;
            case 4:
                Console.WriteLine("Enter the filename: ");
                var destination = Console.ReadLine();
                automaton.SaveToFile(destination);
                Console.WriteLine("Automaton succesfully saved.");
                break;
            case 5:
                List<string> exampleAutomatons = new List<string> {
                    "Code automaton checker",
                    "Binary string checker",
                    "Floating point number checker"
                };
                var choice = gui.PrintMenu(exampleAutomatons);
                automaton = choice switch
                {
                    0 => new DFA("ZipCodeAutomaton.txt"),
                    1 => new NFA("binary.txt"),
                    2 => new NFA("numberParser.txt"),
                    _ => automaton
                } ?? new DFA();
                Console.WriteLine("EXAMPLE AUTOMATON LOADED");
                break;
            case 6:
                if (automaton is NFA nfaWithEpsilon)
                {
                    if (nfaWithEpsilon.HasEpsilonTransitions)
                    {
                        automaton = nfaWithEpsilon.RemoveEpsilonTransitions();
                        Console.WriteLine("Epsilon transitions removed.");
                    }
                    else
                    {
                        Console.WriteLine("Automaton doesn't have epsilon transitions");
                    }
                }
                else
                {
                    Console.WriteLine("DFA Automaton doesn't have epsilon transitions");
                }
                break;
            case 7:
                if (automaton is NFA nfa)
                {
                    automaton = new DFA(nfa);
                    Console.WriteLine("Automaton succesfully converted to DFA");
                }
                else
                {
                    Console.WriteLine("Automaton is not NFA.");
                }
                break;
            case 8:
                gui.IsRunning = false;
                return;
            default:
                break;
        }
    }
    else
    {
        Console.WriteLine("Invalid option");
    }
Console.ReadKey();
}