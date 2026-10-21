using System;

namespace MealyCommentDetector
{
    // 1. States matching your diagram exactly
    public enum State
    {
        Q0,    // Start state (waiting for '/')
        Q1,    // Saw first '/' (waiting for '*')
        Q2,    // Inside comment body
        Q3,    // Saw '*' inside comment (checking for '/')
        QTrap  // Trap / Error state
    }

    public class MealyMachine
    {
        /// <summary>
        /// Transition function: (CurrentState, InputChar) -> (NextState, Output)
        /// Exactly matches the arrows in your diagram!
        /// </summary>
        public static (State NextState, int Output) Transition(State currentState, char input)
        {
            // Any character that isn't '/' or '*' is treated as 'a'
            char symbol = (input == '/' || input == '*') ? input : 'a';

            switch (currentState)
            {
                case State.Q0:
                    if (symbol == '/')
                        return (State.Q1, 0);     // q0 -- / / 0 --> q1
                    else
                        return (State.QTrap, 0);  // q0 -- a, * / 0 --> qtrap

                case State.Q1:
                    if (symbol == '*')
                        return (State.Q2, 0);     // q1 -- * / 0 --> q2
                    else
                        return (State.QTrap, 0);  // q1 -- a, / / 0 --> qtrap

                case State.Q2:
                    if (symbol == '*')
                        return (State.Q3, 0);     // q2 -- * / 0 --> q3
                    else
                        return (State.Q2, 0);     // q2 -- a, / / 0 --> q2 (self-loop)

                case State.Q3:
                    if (symbol == '/')
                        return (State.Q0, 1);     // q3 -- / / 1 --> q0 (Comment closed! Output 1)
                    else if (symbol == '*')
                        return (State.Q3, 0);     // q3 -- * / 0 --> q3 (self-loop for /***)
                    else
                        return (State.Q2, 0);     // q3 -- a / 0 --> q2

                case State.QTrap:
                default:
                    return (State.QTrap, 0);      // qtrap -- a, *, / / 0 --> qtrap
            }
        }

        /// <summary>
        /// Traces the Mealy Machine step-by-step for a given string.
        /// </summary>
        public static void ProcessString(string input)
        {
            Console.WriteLine($"\nInput string: \"{input}\"");
            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"{"Step",-6} | {"Current",-8} | {"Input",-6} | {"Next State",-10} | {"Output",-6}");
            Console.WriteLine(new string('-', 55));

            State currentState = State.Q0;
            string outputSequence = "";
            int lastOutput = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                var (nextState, output) = Transition(currentState, ch);

                Console.WriteLine($"{i + 1,-6} | {currentState,-8} | {ch,-6} | {nextState,-10} | {output,-6}");

                outputSequence += output.ToString();
                lastOutput = output;
                currentState = nextState;
            }

            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"Output Sequence : {outputSequence}");

            // In a Mealy machine, string is accepted if the last output is 1
            if (lastOutput == 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Result          : ACCEPTED (Valid C-Style Comment)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Result          : REJECTED (Invalid or Incomplete)");
            }
            Console.ResetColor();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   C-Style Comment Recognizer (Mealy Machine)    ");
            Console.WriteLine("=================================================");

            // 1. Run all test cases from your slide
            string[] testCases = new string[]
            {
                // Accepted:
                "/*a*/",
                "/**/",
                "/***/",
                "/*aaa*aaa*/",
                "/*a/a*/",

                // Rejected:
                "/**",
                "/**/a/*aa*/",
                "aaa/**/aa",
                "/*/",
                "/**a/",
                "//aaaa"
            };

            Console.WriteLine("\n--- RUNNING SLIDE TEST CASES ---");
            foreach (var test in testCases)
            {
                MealyMachine.ProcessString(test);
            }

            // 2. Interactive testing
            Console.WriteLine("\n=================================================");
            Console.WriteLine("Type your own comment to test (press Enter empty to quit):");
            while (true)
            {
                Console.Write("\nEnter string: ");
                string? userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput))
                    break;

                MealyMachine.ProcessString(userInput);
            }
        }
    }
}