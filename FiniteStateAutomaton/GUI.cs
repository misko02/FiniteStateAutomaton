namespace FiniteStateAutomaton
{
    internal class GUI
    {
        /// <summary>
        /// Width of table column
        /// </summary>
        private const int FrameWidth = 9;
        /// <summary>
        /// Cursor position in menu
        /// </summary>
        private int _cursorPosition;
        /// <summary>
        /// Is GUI running
        /// </summary>
        public bool IsRunning { get; set; } = true;

        /// <summary>
        /// Printing menu with options
        /// </summary>
        public int? PrintMenu(List<string> options)
        {
            Console.Clear();
            for (var i = 0; i < options.Count;i++)
            {
                if (_cursorPosition == i)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine($"{i+1}. {options[i]}");
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
        /// Loading automaton from user input
        /// </summary>
        /// <returns>New automaton</returns>
        public Automaton LoadAutomaton(Type type)
        {
            Automaton automaton;
            if (type == typeof(DFA))
            {
               automaton = new DFA();
            }
            else if (type == typeof(NFA))
            {
                automaton = new NFA();
                List<string> options = ["With epsilon transitions", "Without epsilon transitions"];
                if (PrintMenu(options) == 0)
                {
                    automaton.Sigma.Add(NFA.Epsilon);
                }
            }
            else
            {
                return null;
            }
            //Load states
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter number of states: ");
            int numberOfStates = int.Parse(Console.ReadLine());
            for (int i = 0; i < numberOfStates; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                Console.Write($"Label of state number {i + 1}: ");
                var state = Console.ReadLine();
                automaton.AddState("q"+state);
                Console.Clear();
            }
            //load alphabet
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter number of symbols of alphabet: ");
            int numberOfSymbols = int.Parse(Console.ReadLine());
            Console.Clear();
            for (int i = 0; i < numberOfSymbols; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                Console.Write($"Enter label of symbol number {i + 1}: ");
                string symbol = Console.ReadLine() ?? string.Empty;
                automaton.Sigma.Add(symbol);
                Console.Clear();
            }
            //load initial state
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter initial state number: ");
            string initialState = Console.ReadLine();
            if (!automaton.States.Contains("q"+initialState))
            {
                Console.WriteLine("Initial state doesn't exist in the set of states");
                Console.ReadKey();
                return null;
            }
            automaton.MarkAsInitial("q" + initialState);
            //load final states
            Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
            Console.Write("Enter number of final states: ");
            int numberOfFinalStates = int.Parse(Console.ReadLine());
            for (int i = 0; i < numberOfFinalStates; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                Console.Write($"Enter label of final state number {i + 1}: ");
                string state = Console.ReadLine();
                if (!automaton.States.Contains("q" + state))
                {
                    Console.WriteLine("Final state doesn't exist in the set of states");
                    Console.ReadKey();
                    return null;
                }
                automaton.MarkAsFinal("q" + state);
                Console.Clear();
            }
            //load transitions
           foreach(var state in automaton.States)
            {
                foreach (var symbol in automaton.Sigma)
                {

                    Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                    int numberOfTransitions;
                    if (type == typeof(DFA))
                        numberOfTransitions = 1;
                    else
                    {
                        Console.Write($"Enter number of transitions of state {state} after symbol {symbol}: ");
                        numberOfTransitions = int.Parse(Console.ReadLine());
                    }
                    for (var i = 0; i < numberOfTransitions; i++)
                    {
                        Console.Clear();
                        Console.SetCursorPosition(Console.WindowWidth / 3, Console.WindowHeight / 2);
                        Console.Write($"Enter {i+1} destination states after transition from state {state} and symbol {symbol}: ");
                        string toState = Console.ReadLine();
                        if (!automaton.States.Contains("q" + toState))
                        {
                            Console.WriteLine("Destination state doesn't exist in the set of states");
                            Console.ReadKey();
                            return null;
                        }
                        automaton.AddTransition(state, "q"+toState, symbol);
                        Console.Clear();
                    }
                }
            }
            PrintAutomaton(automaton);
            return automaton;
        }
        /// <summary>
        /// Print automaton in tabular form
        /// </summary>
        /// <param name="dfa"> automat skonczenie stanowy</param>
        public void PrintAutomaton(Automaton automaton)
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 , 0);
            Console.WriteLine(automaton.GetType().Name);

            //top of table
            Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth )/4;
            Console.Write("||" + "=".Repeat(FrameWidth) + "||");
            Console.Write(("=".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
            Console.Write("||" + " ".Repeat(FrameWidth) + "||");
            Console.Write((" ".Repeat(FrameWidth)+"||").Repeat(automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
            Console.Write("||" + "State".CenterString(FrameWidth) + "||");
            foreach (var symbol in automaton.Sigma)
            {
                Console.Write(symbol.CenterString(FrameWidth)+"||");
            }
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
            Console.Write("||" + " ".Repeat(FrameWidth) + "||");
            Console.Write((" ".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
            Console.WriteLine();

            Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
            Console.Write("||" + "=".Repeat(FrameWidth) + "||");
            Console.Write(("=".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
            Console.WriteLine();
            // Main table
            foreach(var state in automaton.States)
            {
                Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
                Console.Write("||" + " ".Repeat(FrameWidth) + "||");
                Console.Write((" ".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
                var graphicalState = state;
                if(state==automaton.InitialState && automaton.FinalStates.Contains(state))
                {
                    graphicalState = $"->_{state}_";
                }
                else if (state == automaton.InitialState)
                {
                    graphicalState = $"-> {state} ";
                }
                else if (automaton.FinalStates.Contains(state))
                {
                    graphicalState = $"_{state}_";
                }
                Console.Write($"||{graphicalState.CenterString(FrameWidth)}||");
                foreach (var symbol in automaton.Sigma)
                {
                    if (automaton is DFA)
                    {
                        Console.Write((automaton.GetNextState(state,symbol).FirstOrDefault()??"-").CenterString(FrameWidth) + "||");
                    }
                    else
                    {
                        var destinations = automaton.GetNextState(state, symbol).Count > 0
                            ? "{" + string.Join(",", automaton.Delta[(state, symbol)]) + "}"
                            :"{-}";
                        Console.Write(destinations.CenterString(FrameWidth) + "||");
                    }
                }
                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
                Console.Write("||" + " ".Repeat(FrameWidth) + "||");
                Console.Write((" ".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
                Console.WriteLine();

                Console.CursorLeft = (Console.WindowWidth - automaton.Sigma.Count * FrameWidth) / 4;
                Console.Write("||"+ "=".Repeat(FrameWidth) + "||");
                Console.Write(("=".Repeat(FrameWidth) + "||").Repeat(automaton.Sigma.Count));
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
        /// Repeat string n times
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
        /// Centerize string in given width
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
