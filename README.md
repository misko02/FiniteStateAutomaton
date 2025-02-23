# User Manual
## File structure
```
.Automatons - File folder with predefined automatons
	├── ZipCodeAutomaton.txt - Automaton accepting Polish zip codes - Example of DFA
	├── BinaryAutomaton.txt - Automaton accepting binary strings with substrings "01" and "10" - Example of NFA
	├── numberParser.txt - Automaton accepting floating point numbers with or without leading sign - Example of NFA with epsilon transitions
- Automaton.cs - Acepting class representing finite automaton
- DFA.cs - Class representing deterministic automaton
- NFA.cs - Class representing non-deterministic automaton
- GUI.cs - Class representing GUI
- Program.cs - Main file of the program
- GlobalUsings.cs - Global usings
- README.md - User manual
```
## Running of program
```
dotnet run
```

## Program description
<p> After running the program, a window with a graphical interface and a visible menu will appear. The menu contains the following options: </p>

### Load automaton
<p> Opens a dialog window allowing to create a new automaton using a graphical interface. </p>

_You can find more instructions about loading automaton below in the **"Loading automaton"** section_
### Display automaton
Display the automaton in a tabular form.
### Check word
Allows you to check if a given word is accepted by the automaton.
### Load automaton from file
<p>Allows you to load automaton from txt file </p>

*__Warning:__*<br>
You have to provide the full name of the file ***with extension***

### Save automaton to file
<p> Allows you to save the automaton to a text file. </p>

*__Warning:__*<br>
You have to provide the full name of the file ***with extension***

### Load example automaton
<p> Loads a predefined automatons from a example pool </p>

#### Automatons:
- ZipCodeAutomaton.txt - Automaton accepting Polish zip codes - Example of DFA
- BinaryAutomaton.txt - Automaton accepting binary strings with substrings "01" and "10" - Example of NFA
- numberParser.txt - Automaton accepting floating point numbers with or without leading sign - Example of NFA with epsilon transitions

### Remove epsilon transitions
<p> Removes epsilon transitions from the automaton, if they exist.</p>

### Transform NFA to DFA
<p> Transforms a non-deterministic automaton into a deterministic one. <br>
If the automaton has epsilon transitions, they will be removed. </p>

### Close
<p> Close the program. </p>

## Loading automaton
### Automaton's type
<p> After selecting the "Load automaton" option, a dialog window with a graphical interface will appear. </p>
<p> On the beginning we have to choose the type of automaton we want to create (DFA for deterministic automaton, NFA for non-deterministic automaton). <br>
If we choose non-deterministic automaton, we will also be able to specify if we want the automaton to have epsilon transitions. </p>

### Loading deterministic automaton
1. First step will be to specify the number of states we want to add to the automaton
	* We are giving the number of states, that we want to add to the automaton
	* For each state we will be able to specify its label (label means only the suffix of the state, we don't have to add "q", it will be automatically added).)
2. The second step will be to specify the number of characters that our alphabet will have
	* On the beginning we declare the number of characters that our alphabet will have
	* After that, for each character of the alphabet, we will be able to specify its label
3. The third step will be to specify the initial state of the automaton
	* On the beginning we declare the initial state of the automaton
		* _Warning:_ We have to give only the label of the state, additionally, the state must be in the set of states declared in the first step
	* Next, we specify the number of accepting states
		* For each accepting state, we specify its label
			* _Warning:_ Again, accepting states must be in the set of states declared in the first step
4. The last step will be to specify the delta function
	* For each state and character of the alphabet, we specify the state to which the automaton goes after reading the given character
		* _Warning:_ All states must be in the set of states declared in the first step
5. After loading all information, the automaton will be saved and displayed in tabular form.

### Loading non-deterministic automaton
Procedure of loading non-deterministic automaton is very similar to loading deterministic automaton, with some differences, which will be described below. <br>
Additionally, the procedure may slightly differ depending on whether we have chosen an automaton with epsilon transitions or without.

1. The first step will be to specify the number of states in the automaton
	* This point is no different from loading states in a deterministic automaton
2. The second step will be to specify the characters of the alphabet
	* If we have chosen an NFA with epsilon transitions, the epsilon character will be automatically added to the alphabet, without the need to declare it
3. The third step will be to specify the special states
	* This point is also no different from loading states in a deterministic automaton
4. The last step will be to specify delta function
	* This step is the most different from loading a deterministic automaton
		* First of all, for each state and character of the alphabet, we can specify the number of states to which the automaton goes after reading the given character
		* Then, we choose the states to which the automaton goes after reading the given character
			* _Warning:_ All states must be in the set of states declared in the first step, and characters in the set of characters declared in the second step
			* _Warning 2:_ In the case of choosing an NFA with epsilon transitions, for each state and character of the alphabet, we can additionally specify the number of states to which the automaton goes after reading the epsilon character
			* _Warning 3:_ If we want the automaton to go to no state after a given character (i.e. go to an empty set of states), we enter "0")
5. After loading all information, the automaton will be saved and displayed in tabular form. Notice that now states are defined as sets.
## Additional notes

If you have problems running the program, try updating the .NET Core SDK to the latest version.\
```
dotnet tool update --global dotnet-ef
```
And try to restore the project using the command:\
```
dotnet restore
```
Additionally, to avoid problems with displaying tables, it is recommended to run the program in the "Terminal" program in full-screen mode. <br>
If problems with displaying tables still occur, you can change the `FrameWidth` constant in the `GUI.cs` class. <br>
Additionally, it may be helpful to change the font size in the terminal. (ctrl + mouse scroll) <br>
nie rozmiaru czcionki w terminalu. (ctrl + scroll myszki) <br>