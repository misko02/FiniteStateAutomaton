namespace FiniteStateAutomaton
{
    /// <summary>
    ///     GUI class to interact with user and display interface
    /// </summary>
    internal class GUI
    {
        /// <summary>
        ///     Cursor position in menu
        /// </summary>
        private int _cursorPosition;

        /// <summary>
        ///     Width of table column
        /// </summary>
        private readonly int _frameWidth = 9;
        
        private Automaton? _automaton;

        /// <summary>
        ///     Is GUI running?
        /// </summary>
        public bool IsRunning { get; set; } = true;

        /// <summary>
        ///     Printing menu with options
        /// </summary>
        public int? PrintMenu(List<string> options)
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

            var key = Console.ReadKey();
            Console.Beep();
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    _cursorPosition = (options.Count + _cursorPosition - 1) % options.Count;
                    PrintMenu(options);
                    break;
                case ConsoleKey.DownArrow:
                    _cursorPosition = (options.Count + _cursorPosition + 1) % options.Count;
                    PrintMenu(options);
                    break;
                case ConsoleKey.Enter:
                    return _cursorPosition;
                default: return null;
            }

            return _cursorPosition;
        }

        /// <summary>
        ///     Loading automaton from user input
        /// </summary>
        /// <returns>New automaton</returns>
        public Automaton LoadAutomaton(Type type)
        {
            // Ugly code, consider factory pattern
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
            foreach (var state in _automaton.States)
            foreach (var symbol in _automaton.Sigma)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                int numberOfTransitions;
                if (typeof(Automaton).Name == "DFA")
                {
                    numberOfTransitions = 1;
                }
                else
                {
                    Console.Write($"Enter number of transitions of state {state} after symbol {symbol}: ");
                    if(!int.TryParse(Console.ReadLine(), out numberOfTransitions))
                        Console.WriteLine("Invalid number of transitions");
                }
                for (var i = 0; i < numberOfTransitions; i++)
                {
                    Console.Clear();
                    Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                    Console.Write(
                        $"Enter {i + 1} destination states after transition from state {state} and symbol {symbol}: ");
                    var toState = Console.ReadLine();
                    if (!_automaton.States.Contains("q" + toState))
                    {
                        Console.WriteLine("Destination state doesn't exist in the set of states");
                        Console.ReadKey();
                        i--;
                    }

                    _automaton.AddTransition(state, "q" + toState, symbol);
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Method to load initial state from user input
        /// </summary>
        private void LoadInitialStates()
        {
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter initial state number: ");
            var initialState = Console.ReadLine();
            if (!_automaton.States.Contains("q" + initialState))
            {
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
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter number of final states: ");
            var numberOfFinalStates = int.Parse(Console.ReadLine());
            for (var i = 0; i < numberOfFinalStates; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                Console.Write($"Enter label of final state number {i + 1}: ");
                var state = Console.ReadLine();
                if (!_automaton.States.Contains("q" + state))
                {
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
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter number of symbols of alphabet: ");
            var numberOfSymbols = int.Parse(Console.ReadLine());
            Console.Clear();
            for (var i = 0; i < numberOfSymbols; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                Console.Write($"Enter label of symbol number {i + 1}: ");
                var symbol = Console.ReadLine() ?? string.Empty;
                _automaton.Sigma.Add(symbol);
                Console.Clear();
            }
        }

        private void LoadStates()
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
            Console.Write("Enter number of states: ");
            var numberOfStates = int.Parse(Console.ReadLine());
            for (var i = -1; i < numberOfStates; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2, Console.WindowHeight / 2);
                Console.Write($"Label of state number {i + 0}: ");
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

// Additonal string class extensions because I needed some methods and it was pointless to make them in project scale
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
        /// <example><c>"code".CenterString(10)</c>="   code   "</example>
        public static string CenterString(this string s, int width)
        {
            return s.PadLeft((width - s.Length) / 2 + s.Length).PadRight(width);
        }
    }
}