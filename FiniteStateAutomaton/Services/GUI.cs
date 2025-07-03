using FiniteStateAutomaton.Enums;
using FiniteStateAutomaton.Models;

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
        ///     Cursor position in menu
        /// </summary>
        private int _cursorPosition = 0;
        /// <summary>
        ///     Width of a table column
        /// </summary>
        private readonly int _frameWidth = 9;
        /// <summary>
        /// Center of the console window width
        /// </summary>
        private int WidthCenter => Console.WindowWidth / 3;
        /// <summary>
        ///   Center of the console window height
        ///  </summary>
        private int HeightCenter => Console.WindowHeight / 2;
        /// <summary>
        ///     Is GUI running?
        /// </summary>
        public bool IsRunning { get; private set; } = true;

        Automaton? _automaton = null;
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
                { Pages.ChooseAutomatonType, ChooseAutomatonType },
                { Pages.ChooseNFAtype, ChooseNFAtype },
                { Pages.DefineStates, LoadStates },
                { Pages.DefineAlphabet, LoadAlphabet },
                { Pages.DefineInitialState, LoadInitialStates },
                { Pages.DefineFinalStates, LoadFinalStates },
                { Pages.DefineTransitions, LoadTransitions },
                { Pages.PrintAutomaton, () => PrintAutomaton(_automaton) },
                { Pages.CheckWord, CheckWord },
                { Pages.LoadAutomatonFromFile, LoadAutomatonFromFile },
                { Pages.SaveAutomatonToFile, SaveAutomatonToFile },
                { Pages.LoadExampleAutomaton, LoadExampleAutomaton },
                { Pages.ExampleAutomatonsMenu, ExampleAutomatonsMenu }
            };
            pagesDictionary[_page]();
            int?option = PrintMenu(MenuPages.MenuItems[_page]);

            if (!option.HasValue)
            {
                Console.WriteLine("Invalid option. Please try again.");
                return;
            }

            switch (option.Value)
            {
                case 0:
                    var automatonTypes = new List<Type?> { typeof(DFA), typeof(NFA) };
                    var automatonType = automatonTypes[PrintMenu(automatonTypes.Select(x => x.Name).ToList()) ?? 0];
                    _automaton = LoadAutomaton(automatonType);
                    break;
                case 1:
                    if (_automaton is null)
                    {
                        Console.WriteLine("No automaton loaded.");
                        break;
                    }

                    try
                    {
                        PrintAutomaton(_automaton);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Automaton couldn't be printed.");
                        Console.WriteLine($"Exception: {e.Message}");
                    }

                    break;
                case 2:
                    if (_automaton is null)
                    {
                        Console.WriteLine("No automaton loaded.");
                        break;
                    }

                    Console.WriteLine("Enter the word to check: ");
                    var word = Console.ReadLine() ?? string.Empty;
                    Console.WriteLine(_automaton.Accepts(word)
                        ? "Automaton accepts this word"
                        : "Automaton doesn't accept this word");
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
                        _automaton = new NFA(filename);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("automaton couldn't be loaded.");
                        Console.WriteLine($"Exception: {e.Message}");
                        break;
                    }

                    Console.WriteLine("Automaton successfully loaded.");
                    break;
                case 4:
                    if (_automaton is null)
                    {
                        Console.WriteLine("No automaton loaded.");
                        break;
                    }

                    Console.WriteLine("Enter the filename: ");
                    var destination = Console.ReadLine() ?? "filename.txt";
                    try
                    {
                        _automaton.SaveToFile(destination);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("There was some problem during saving automaton.");
                        Console.WriteLine($"Exception: {e.Message}");
                        break;
                    }

                    Console.WriteLine("Automaton successfully saved.");
                    break;
                case 5:
                    var choice = PrintMenu(exampleAutomatons);
                    _automaton = choice switch
                    {
                        0 => new DFA("ZipCodeAutomaton.txt"),
                        1 => new NFA("binary.txt"),
                        2 => new NFA("numberParser.txt"),
                        _ => _automaton
                    } ?? new DFA();
                    Console.WriteLine("EXAMPLE AUTOMATON LOADED");
                    break;
                case 6:
                    if (_automaton is null)
                    {
                        Console.WriteLine("No automaton loaded.");
                        break;
                    }

                    if (_automaton is not NFA nfaAutomaton)
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
                    if (_automaton is not NFA nfa)
                    {
                        Console.WriteLine("Automaton is not NFA.");
                        break;
                    }

                    _automaton = new DFA(nfa);
                    Console.WriteLine("Automaton successfully converted to DFA");
                    break;
                case 8:
                    IsRunning = false;
                    return;

            }
        }
        // Consider moving validation of page to separate method to avoid code duplication
        private void ExampleAutomatonsMenu()
        {
            if (_page != Pages.ExampleAutomatonsMenu)
            {
                Console.Clear();
                _page = Pages.ExampleAutomatonsMenu;
                _cursorPosition = 0;
            }
            PrintMenu(MenuPages.MenuItems[_page]);
        }

        private void LoadExampleAutomaton()
        {
            if (_page != Pages.LoadExampleAutomaton)
            {
                Console.Clear();
                _page = Pages.LoadExampleAutomaton;
                _cursorPosition = 0;
            }
            PrintMenu(MenuPages.MenuItems[_page]);
        }

        private void SaveAutomatonToFile()
        {
            throw new NotImplementedException();
        }

        private void LoadAutomatonFromFile()
        {
            throw new NotImplementedException();
        }

        private void CheckWord()
        {
            throw new NotImplementedException();
        }

        private void ChooseNFAtype()
        {
            if (_page != Pages.ChooseNFAtype)
            {
                Console.Clear();
                _page = Pages.ChooseNFAtype;
                _cursorPosition = 0;
            }
            PrintMenu(MenuPages.MenuItems[_page]);
        }

        private void ChooseAutomatonType()
        {
            if (_page != Pages.ChooseAutomatonType)
            {
                Console.Clear();
                _page = Pages.ChooseAutomatonType;
                _cursorPosition = 0;
            }
            PrintMenu(MenuPages.MenuItems[_page]);
        }

        private void MainMenu()
        {
            if (_page != Pages.MainMenu)
            {
                Console.Clear();
                _page = Pages.MainMenu;
                _cursorPosition = 0;
            }
            PrintMenu(MenuPages.MenuItems[_page]);
        }

        /// <summary>
        ///     Printing a menu with options
        /// </summary>
        private int? PrintMenu(List<string> options)
        {
            int? option = 0;
            while (option is not null)
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
                option = HandleMenuInput(options); 
                if(option is not null)
                {
                    return option;
                }
            }
            return option;
        }
        
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

        /// <summary>
        ///     Loading automaton from user input
        /// </summary>
        /// <returns>New automaton</returns>
        public Automaton? LoadAutomaton(Type? type)
        {
            // Ugly code, consider a factory pattern
            if (type is null)
            {
                Console.WriteLine("Automaton type is not specified.");
                return null;
            }
            switch (type.Name)
            {
                case "DFA":
                    _automaton = new DFA(); break;
                case "NFA":
                    _automaton = new NFA();
                    List<string> options = ["With epsilon transitions", "Without epsilon transitions"];
                    if (PrintMenu(options) == 0) _automaton.Sigma.Add(NFA.Epsilon);
                    break;
                default:
                    Console.WriteLine("Invalid automaton type selected.");
                    return null;
            }
            LoadStates();
            LoadAlphabet();
            LoadInitialStates();
            LoadFinalStates();
            LoadTransitions();
            PrintAutomaton(_automaton);
            return _automaton;
        }
        
        /// <summary>
        /// Method to load transitions from user input
        /// </summary>

        private void LoadTransitions()
        {
            if (_automaton is null)
            {
                Console.WriteLine("Automaton is not loaded.");
                return;
            }
            Console.Clear();
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
                        LoadTransitions();
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
                        _automaton.AddTransition(state, "q" + toState, symbol);
                    }
                    catch (Exception e)
                    {
                        Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                        Console.WriteLine($"Exception occurred: {e.Message}");
                    }
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Method to load the initial state from user input
        /// </summary>
        private void LoadInitialStates()
        {
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

        private void LoadStates()
        {
            if (_automaton is null)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                Console.WriteLine("Automaton is not loaded.");
                Console.ReadKey();
                return;
            }
            Console.Clear();
            Console.SetCursorPosition(WidthCenter, HeightCenter);
            Console.Write("Enter number of states: ");
            if (!int.TryParse(Console.ReadLine(), out var numberOfStates) || numberOfStates < 0)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter + 1);
                Console.WriteLine("Invalid number of states");
                Console.ReadKey();
                LoadStates();
            }
            for (var i = 0; i < numberOfStates; i++)
            {
                Console.SetCursorPosition(WidthCenter, HeightCenter);
                Console.Write($"Label of state number {i+1}: ");
                var state = Console.ReadLine();
                _automaton.AddState("q" + state);
                Console.Clear();
            }   
        }
        

        /// <summary>
        ///     Print automaton in tabular form
        /// </summary>
        public void PrintAutomaton(Automaton? automaton)
        {
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