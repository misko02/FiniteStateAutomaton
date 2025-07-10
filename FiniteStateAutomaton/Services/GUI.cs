using FiniteStateAutomaton.Enums;
using FiniteStateAutomaton.Models;
using FiniteStateAutomaton.Services;

namespace FiniteStateAutomaton
{
    /// <summary>
    ///     GUI class to interact with user and display interface
    /// </summary>
    internal class GUI
    {
        /// <summary>
        /// Current Page of GUI
        /// </summary>
        private Pages _page = Pages.MainMenu;
        /// <summary>
        /// Stack of previous pages to allow going back in menu
        /// </summary>
        private Stack<Pages> _previousPages = new();
        /// <summary>
        ///     Cursor position in menu
        /// </summary>
        private int _cursorPosition;
        /// <summary>
        ///     Width of a table column
        /// </summary>
        private readonly int _frameWidth = 9;
        /// <summary>
        /// Center of the console window width
        /// </summary>
        private static int WidthCenter => Console.WindowWidth / 3;
        /// <summary>
        ///   Center of the console window height
        ///  </summary>
        private static int HeightCenter => Console.WindowHeight / 2;
        /// <summary>
        ///     Is GUI running?
        /// </summary>
        public bool IsRunning { get; private set; } = true;

        private Automaton? _automaton;
        /// <summary>
        /// Constructor of GUI class
        /// </summary>
        public GUI()
        {
            Console.Title = "Finite State Automaton";
            Console.CursorVisible = false;
        }
        public void Run()
        {
            var pagesDictionary = new Dictionary<Pages, Action>
            {
                { Pages.MainMenu, MainMenu },
                { Pages.ChooseAutomatonType, () => ChooseAutomatonType()},
                { Pages.ChooseNFAtype, () => ChooseNFAtype()},
                { Pages.DefineStates, () => LoadStates() },
                { Pages.DefineAlphabet, LoadAlphabet },
                { Pages.DefineInitialState, LoadInitialStates },
                { Pages.DefineFinalStates, LoadFinalStates },
                { Pages.DefineTransitions, LoadTransitions },
                { Pages.PrintAutomaton, () => PrintAutomaton(_automaton) },
                { Pages.CheckWord, CheckWord },
                { Pages.LoadAutomatonFromFile, LoadAutomatonFromFile },
                { Pages.SaveAutomatonToFile, SaveAutomatonToFile },
                { Pages.ExampleAutomatonsMenu, ExampleAutomatonsMenu }
            };
            pagesDictionary[_page]();
            Console.ReadKey();  
        }
        /// <summary>
        ///     Printing a menu with options
        /// </summary>
        private int PrintMenu(List<string> options)
        {
            while (true)
            {
                Console.Clear();
                for (var i = 0; i < options.Count; i++)
                {
                    if (_cursorPosition == i)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{i + 1}. {options[i]}");
                    Console.ResetColor();
                }
                var option = HandleMenuInput(options); 
                if(option.HasValue)
                {
                    return option.Value;
                }
            }
        }
        // Consider moving validation of page to separate method to avoid code duplication
        private int? HandleMenuInput(List<string> options)
        {   
            var key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    _cursorPosition = (_cursorPosition - 1 + options.Count) % options.Count;
                    return null;
                case ConsoleKey.DownArrow:
                    _cursorPosition = (_cursorPosition + 1) % options.Count;
                    return null;
                case ConsoleKey.Enter:
                    return _cursorPosition;
                default:
                    return null;
            }
        }

        private void LoadPage(Pages page)
        {
            if (_page != page)
            {
                Console.Clear();
                _previousPages.Push(_page);
                _page = page;
                _cursorPosition = 0;
            }
        }
        
        private void ExampleAutomatonsMenu()
        {
            LoadPage(Pages.ExampleAutomatonsMenu);
            var option = PrintMenu(MenuPages.MenuItems[_page]);
            switch (option)
            {
                case 0:
                    _automaton = CreationWizard.Wizard.LoadExampleAutomaton("ZipCodeAutomaton.txt"); // Example DFA
                    PrintAutomaton(_automaton);
                    break;
                case 1:
                    _automaton = CreationWizard.Wizard.LoadExampleAutomaton("BinaryAutomaton.txt"); // Example NFA
                    PrintAutomaton(_automaton);
                    break;
                case 2:
                    _automaton = CreationWizard.Wizard.LoadExampleAutomaton("NumberParser.txt"); // Example NFA with epsilon transitions
                    PrintAutomaton(_automaton);
                    break;
                default:
                    MainMenu();
                    break;
            }
        }

        private void SaveAutomatonToFile()
        {
            // I don't want to add Exit neither to have error in case of no automaton loaded, so I'll have to use stack to remember previous pages. Consider.
           LoadPage(Pages.SaveAutomatonToFile); 
            Console.WriteLine("Enter the filename: ");
            var filename = Console.ReadLine() ?? "filename.txt";
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Filename cannot be empty.");
                return;
            }
            try
            {
                _automaton?.SaveToFile(filename);
                Console.WriteLine("Automaton successfully saved.");
            }
            catch (Exception e)
            {
                Console.WriteLine("There was some problem during saving automaton.");
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        private void LoadAutomatonFromFile()
        {
            LoadPage(Pages.LoadAutomatonFromFile);
            Console.WriteLine("Enter the filename: ");
            var filename = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("Filename cannot be empty.");
                return;
            }
            try
            {
                _automaton = new NFA(filename);
                Console.WriteLine("Automaton successfully loaded.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Automaton couldn't be loaded.");
                Console.WriteLine($"Exception: {e.Message}");
            }
        }

        private void CheckWord()
        {
           LoadPage(Pages.CheckWord); 
            if (_automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                return;
            }
            Console.WriteLine("Enter the word to check: ");
            var word = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(word))
            {
                Console.WriteLine("Word cannot be empty.");
                return;
            }
            Console.WriteLine(_automaton.Accepts(word)
                ? "Automaton accepts this word"
                : "Automaton doesn't accept this word");
        }

        private string ChooseNFAtype()
        {
           LoadPage(Pages.ChooseNFAtype); 
            var option = PrintMenu(MenuPages.MenuItems[_page]);
            return option switch
            {
                0 => "Epsilon NFA",
                1 => "NFA",
                _ => "Undefined"
            };
        }

        private string ChooseAutomatonType()
        {
            LoadPage(Pages.ChooseAutomatonType);
            var option = PrintMenu(MenuPages.MenuItems[_page]);
            
            switch (option)
            {
                case 0:
                    return "DFA";
                case 1: {
                    return ChooseNFAtype();
                }
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    return "Undefined";
            }
            
        }

        private void MainMenu()
        {
            LoadPage(Pages.MainMenu); 
            var option = PrintMenu(MenuPages.MenuItems[_page]);
            switch (option)
            {
                case 0: 
                    LoadAutomaton();
                    break;
                case 1:
                    PrintAutomaton(_automaton);
                    break;
                case 2:
                    CheckWord();
                    break;
                case 3:
                    LoadAutomatonFromFile();
                    break;
                case 4:
                    SaveAutomatonToFile();
                    break;
                case 5:
                    ExampleAutomatonsMenu();
                    break;
                case 6:
                    if (_automaton is NFA nfa)
                        nfa.RemoveEpsilonTransitions();
                    else
                        Console.WriteLine("DFA automaton doesn't have epsilon transitions");
                    break;
                case 7:
                    if (_automaton is NFA nfaAutomaton)
                        _automaton = new DFA(nfaAutomaton);
                    else
                        Console.WriteLine("Automaton is not NFA.");
                    break;
                case 8:
                    IsRunning = false;
                    return;
            }
            _page = Pages.MainMenu; 
        }


        /// <summary>
        ///     Loading automaton from user input
        /// </summary>
        /// <returns>New automaton</returns>
        private void LoadAutomaton()
        {
            CreationWizard wizard = CreationWizard.Wizard;
            var type = ChooseAutomatonType();
            wizard.ChooseAutomatonType(type);
            List<string> states = LoadStates();
            wizard.DefineStates(states);
            LoadAlphabet();
            LoadInitialStates();
            LoadFinalStates();
            LoadTransitions();
            PrintAutomaton(_automaton);
        }
        
        /// <summary>
        /// Method to load transitions from user input
        /// </summary>

        private void LoadTransitions()
        {
            LoadPage(Pages.DefineTransitions);
            if (_automaton is null)
            {
                Console.WriteLine("Automaton is not loaded.");
                return;
            }
            Console.Clear();
            List<Tuple<string,string,string>> transitions = new List<Tuple<string, string, string>>();
            foreach (var state in _automaton.States)
            foreach (var symbol in _automaton.Sigma)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                int numberOfTransitions;
                if (_automaton is DFA)
                {
                    numberOfTransitions = 1;
                }
                else
                {
                    Console.Write($"Enter number of transitions of state {state} after symbol {symbol}: ");
                    if (!int.TryParse(Console.ReadLine(), out numberOfTransitions))
                    {
                        Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                        Console.WriteLine("Invalid number of transitions");
                        Console.ReadKey();
                    }
                }
                for (var i = 0; i < numberOfTransitions; i++)
                {
                    Console.Clear();
                    Console.SetCursorPosition(WidthCenter, HeightCenter);
                    Console.Write(
                        $"Enter {i + 1} destination states after transition from state {state} and symbol {symbol}: ");
                    var toState = Console.ReadLine();
                    if (!_automaton.States.Contains("q" + toState))
                    {
                        Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                        Console.WriteLine("Destination state doesn't exist in the set of states");
                        Console.ReadKey();
                        i--;
                    }
                    
                    try
                    {
                        transitions.Add(new Tuple<string, string, string>(state, symbol, toState));
                    }
                    catch (Exception e)
                    {
                        Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                        Console.WriteLine($"Exception occurred: {e.Message}");
                    }
                    Console.Clear();
                }
            }
            CreationWizard.Wizard.DefineTransitions(transitions);
        }

        /// <summary>
        /// Method to load the initial state from user input
        /// </summary>
        private void LoadInitialStates()
        {
            LoadPage(Pages.DefineInitialState);
            if (_automaton is null)
            {
                Console.Clear();
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                Console.WriteLine("Automaton is not loaded.");
                return;
            }
            Console.Clear();
            Console.SetCursorPosition(WidthCenter, HeightCenter);
            Console.Write("Enter initial state label: ");
            var initialState = Console.ReadLine();
            if (string.IsNullOrEmpty(initialState))
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                Console.WriteLine("Invalid initial state");
                Console.ReadKey();
                LoadInitialStates();
            }
            if (!_automaton.States.Contains("q" + initialState))
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter+1);
                Console.WriteLine("Initial state doesn't exist in the set of states");
                Console.ReadKey();
                LoadInitialStates();
            }

            _automaton.MarkAsInitial("q" + initialState);
        }
        /// <summary>
        /// Method to load final states from user input
        /// </summary>
        private void LoadFinalStates()
        {
            LoadPage(Pages.DefineFinalStates);
            if (_automaton is null)
            {
                Console.Clear();
                Console.SetCursorPosition(WidthCenter, HeightCenter+1);
                Console.WriteLine("Automaton is not loaded.");
                return;
            }
            Console.Clear();
            Console.SetCursorPosition(WidthCenter, HeightCenter);
            Console.Write("Enter number of final states: ");
            if (!int.TryParse(Console.ReadLine(), out var numberOfFinalStates) || numberOfFinalStates <= 0)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                Console.WriteLine("Invalid number of final states");
                Console.ReadKey();
                LoadFinalStates();
            }
            for (var i = 0; i < numberOfFinalStates; i++)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                Console.Write($"Enter label of final state number {i + 1}: ");
                var state = Console.ReadLine();
                if (!_automaton.States.Contains("q" + state))
                {
                    Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                    Console.WriteLine("Final state doesn't exist in the set of states");
                    Console.ReadKey();
                    LoadFinalStates();
                }

                _automaton.MarkAsFinal("q" + state);
                Console.Clear();
            }
        }

        /// <summary>
        /// Method to load alphabet from user input
        /// </summary>
        private void LoadAlphabet()
        {
            LoadPage(Pages.DefineAlphabet);
            if (_automaton is null)
            {
                Console.WriteLine("Automaton is not loaded.");
                Console.ReadKey();
                return;
            }
            Console.Clear();
            Console.SetCursorPosition(WidthCenter, HeightCenter);
            Console.Write("Enter number of symbols of alphabet: ");
            if (!int.TryParse(Console.ReadLine(), out var numberOfSymbols) || numberOfSymbols <= 0)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                Console.WriteLine("Invalid number of symbols");
                Console.ReadKey();
                LoadAlphabet();
            }
            Console.Clear();
            for (var i = 0; i < numberOfSymbols; i++)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                Console.Write($"Enter label of symbol number {i + 1}: ");
                var symbol = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrEmpty(symbol))
                {
                    Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                    Console.WriteLine("Symbol cannot be empty");
                    Console.ReadKey();
                    i--;
                    continue;
                }
                _automaton.Sigma.Add(symbol);
                Console.Clear();
            }
        }

        private List<string> LoadStates()
        {
            LoadPage(Pages.DefineStates);
            if (_automaton is null)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                Console.WriteLine("Automaton is not loaded.");
                Console.ReadKey();
                return new List<string>(); // I will implement this later, when I need to return states
            }
            Console.Clear();
            Console.SetCursorPosition(WidthCenter, HeightCenter);
            Console.Write("Enter number of states: ");
            if (!int.TryParse(Console.ReadLine(), out var numberOfStates) || numberOfStates < 0)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                Console.WriteLine("Invalid number of states");
                Console.ReadKey();
            }
            for (var i = 0; i < numberOfStates; i++)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                Console.Write($"Label of state number {i+1}: ");
                var state = Console.ReadLine();
                _automaton.AddState("q" + state);
                Console.Clear();
            }

            return new List<string>(); // I'll implement this later, when I need to return states
        }
        

        /// <summary>
        ///     Print automaton in tabular form
        /// </summary>
        private void PrintAutomaton(Automaton? automaton)
        {
            LoadPage(Pages.PrintAutomaton);
            if (automaton is null)
            {
                Console.WriteLine("No automaton loaded.");
                return;
            }
            _automaton = automaton;
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, 0);
            Console.WriteLine(_automaton.GetType().Name);

            //top of table
            Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
            Console.Write("||" + "=".Repeat(_frameWidth) + "||");
            Console.Write(("=".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
            Console.Write("||" + " ".Repeat(_frameWidth) + "||");
            Console.Write((" ".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
            Console.Write("||" + "State".CenterString(_frameWidth) + "||");
            foreach (var symbol in _automaton.Sigma) Console.Write(symbol.CenterString(_frameWidth) + "||");
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
            Console.Write("||" + " ".Repeat(_frameWidth) + "||");
            Console.Write((" ".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
            Console.Write("||" + "=".Repeat(_frameWidth) + "||");
            Console.Write(("=".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
            Console.WriteLine();
            // Main table
            foreach (var state in _automaton.States)
            {
                Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
                Console.Write("||" + " ".Repeat(_frameWidth) + "||");
                Console.Write((" ".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
                var graphicalState = state;
                if (state == _automaton.InitialState && _automaton.FinalStates.Contains(state))
                    graphicalState = $"->_{state}_";
                else if (state == _automaton.InitialState)
                    graphicalState = $"-> {state} ";
                else if (_automaton.FinalStates.Contains(state)) graphicalState = $"_{state}_";
                Console.Write($"||{graphicalState.CenterString(_frameWidth)}||");
                foreach (var symbol in _automaton.Sigma)
                    if (_automaton is DFA)
                    {
                        Console.Write(
                            (_automaton.GetNextState(state, symbol).FirstOrDefault() ?? "-").CenterString(_frameWidth) +
                            "||");
                    }
                    else
                    {
                        var destinations = _automaton.GetNextState(state, symbol).Count > 0
                            ? "{" + string.Join(",", _automaton.Delta[(state, symbol)]) + "}"
                            : "{-}";
                        Console.Write(destinations.CenterString(_frameWidth) + "||");
                    }

                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
                Console.Write("||" + " ".Repeat(_frameWidth) + "||");
                Console.Write((" ".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - _automaton.Sigma.Count * _frameWidth) / 4;
                Console.Write("||" + "=".Repeat(_frameWidth) + "||");
                Console.Write(("=".Repeat(_frameWidth) + "||").Repeat(_automaton.Sigma.Count));
                Console.WriteLine();
            }
        }
    }
}

// Additional string class extensions because I needed some methods, and it was pointless to make them in a project scale
namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Repeat string n times
        /// </summary>
        /// <param name="s">string that should be repeated</param>
        /// <param name="n">amount of repetition</param>
        /// <returns>new string with n-times repetitions</returns>
        /// <example><c>"code".Repeat(3)</c>="codecodecode"</example>
        public static string Repeat(this string s, int n)
        {
            return new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
        }

        /// <summary>
        ///     Centerize string in given width
        /// </summary>
        /// <param name="s">String we want to center</param>
        /// <param name="width">Width of string</param>
        /// <returns>String with padded both sides with space</returns>
        /// <example><c>"code".CenterString(10) = "‎ ‎ ‎ code‎ ‎ ‎ "</c> </example>
        public static string CenterString(this string s, int width)
        {
            return s.PadLeft((width - s.Length) / 2 + s.Length).PadRight(width);
        }
    }
}