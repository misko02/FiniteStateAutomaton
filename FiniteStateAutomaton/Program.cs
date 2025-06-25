List<string> menuOptions = [
    "Load Automaton",
    "Print Automaton", 
    "Check the word", 
    "Load Automaton from file", 
    "Save Automaton to file",
    "Load example automaton",
    "Delete epsilon transitions",
    "Convert NFA to DFA",
    "Exit" 
];

var gui = new GUI();
Automaton automaton = new DFA();

while (gui.IsRunning)
{

    var option = gui.PrintMenu(menuOptions);

    if(!option.HasValue)
    {
        Console.WriteLine("Invalid option. Please try again.");
        continue;
    }
    switch (option.Value)
    {
        case 0:
            var automatonTypes = new List<Type> { typeof(DFA), typeof(NFA) };
            var automatonType = automatonTypes[gui.PrintMenu(automatonTypes.Select(x => x.Name).ToList()) ?? 0];
            automaton = gui.LoadAutomaton(automatonType);
            break;
        case 1:
            if (automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                break;
            }
            gui.PrintAutomaton(automaton);
            break;
        case 2:
            if (automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                break;
            }
            Console.WriteLine("Enter the word to check: ");
            var word = Console.ReadLine() ?? string.Empty;
            Console.WriteLine(automaton.Accepts(word) ? "Automaton accepts this word" : "Automaton doesn't accept this word");
            break;
        case 3:
            Console.WriteLine("Enter the filename: ");
            var filename = Console.ReadLine();
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Filename cannot be empty.");
                break;
            }

            try
            {
                automaton = new NFA(filename);
            }
            catch(Exception e)
            {
                Console.WriteLine("automaton couldn't be loaded.");
                Console.WriteLine($"Exception: {e.Message}");
                break;
            }
            Console.WriteLine("Automaton successfully loaded.");
            break;
        case 4:
            if (automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                break;
            }
            Console.WriteLine("Enter the filename: ");
            var destination = Console.ReadLine() ?? "filename.txt";
            automaton.SaveToFile(destination);
            Console.WriteLine("Automaton successfully saved.");
            break;
        case 5:
            List<string> exampleAutomatons = [
                "Code automaton checker",
                "Binary string checker",
                "Floating point number checker"
            ];
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
            if (automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                break;
            }
            if (automaton is not NFA nfaAutomaton)
            {
                Console.WriteLine("DFA Automaton doesn't have epsilon transitions");
                break;
            }
            try
            {
                nfaAutomaton.RemoveEpsilonTransitions();
                Console.WriteLine("Epsilon transitions deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting epsilon transitions: {e.Message}");
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
    }
Console.ReadKey();
}