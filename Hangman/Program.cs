using System;
using System.Collections.Generic;
using System.Linq;
using TM.ProgrammingAdvanced;

namespace Hangman
{
    internal class Program
    {
        #region Hangmen ASCII

        public static string[] Drawings =
        {
            @"         
               
          
          
          
          
         ",
            @"         
               
          
          
          
          
=========",
            @"
      |
      |
      |
      |
      |
=========",
            @"  +---+
      |
      |
      |
      |
      |
=========",
            @"  +---+
  |   |
      |
      |
      |
      |
=========",
            @"  +---+
  |   |
  O   |
      |
      |
      |
=========",
            @"  +---+
  |   |
  O   |
  |   |
      |
      |
=========",
            @"  +---+
  |   |
  O   |
 /|   |
      |
      |
=========",
            @"  +---+
  |   |
  O   |
 /|\  |
      |
      |
=========",
            @"  +---+
  |   |
  O   |
 /|\  |
 /    |
      |
=========",
            @"  +---+
  |   |
  O   |
 /|\  |
 / \  |
      |
========="
        };

        #endregion

        public static string[] Words = Data.Words;

        private static void Main()
        {
            Console.WriteLine("----- HANGMAN -----");

            #region Game

            do
            {
                #region Init of vars

                string input;
                var s = State.Playing;
                Playing p;
                var r = new Random(Guid.NewGuid().GetHashCode());
                int hOrT = r.Next(0, 2), fails = 0;
                char[] wordToFind;
                do
                {
                    wordToFind = Words[r.Next(Words.Length)].ToLower().ToCharArray();
                } while (wordToFind.Length < 4 && wordToFind.Length > 14);
                var masked = new char[wordToFind.Length];
                var guessedWords = new List<string>();
                var guesses = new List<char>();
                var wrongGuesses = new List<char>();
                for (var i = 0; i < wordToFind.Length; i++) masked[i] = '_';
                var possibleWords = Words.Where(word => word.Length == wordToFind.Length).ToList();

                #endregion

                #region Heads or tails

                Console.WriteLine("To decide who is allowed to start, I'll flip a coin.");
                Console.WriteLine("Type H or T to choose the side.");
                input = Console.ReadLine()?.ToUpper();
                Console.Clear();
                if (hOrT == 0 && input == "H" || hOrT == 1 && input == "T")
                {
                    p = Playing.PlayerOne;
                    Console.WriteLine("You guessed correctly, you may start.");
                }
                else if (input == "H" || input == "T")
                {
                    p = Playing.PlayerTwo;
                    Console.WriteLine("Too bad, player two is allowed to begin.");
                }
                else
                {
                    p = Playing.PlayerTwo;
                    Console.WriteLine("You didn't enter a valid choice. Player two is going first now.");
                }

                Console.WriteLine("Press any key to start the game");
                Console.ReadLine();

                #endregion

                do
                {
                    char[] guess;
                    Console.Clear();
                    var found = false;

                    #region Possible words for AI

                    foreach (var wrongGuess in wrongGuesses)
                    {
                        foreach (var word in Words)
                        {
                            if (word.Contains(wrongGuess)) possibleWords.Remove(word);
                        }
                    }

                    for (var i = 0; i < masked.Length; i++)
                    {
                        if (masked[i] == '_') continue;
                        foreach (var word in possibleWords.ToList())
                        {
                            var temp = word.ToCharArray();
                            if (temp[i] != masked[i]) possibleWords.Remove(word);
                        }
                    }

                    #endregion

                    #region Console Writelines

                    Console.WriteLine(Drawings[fails]);
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("The word: " + new string(masked));
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("Guessed letters: " + string.Join(" ", guesses));
                    Console.WriteLine("Guessed words: " + string.Join(" ", guessedWords));
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(string.Join("\n", possibleWords));
                    Console.WriteLine("--------------------------");
                    Console.WriteLine(p == Playing.PlayerOne
                        ? "You're up, good luck."
                        : "Player two? What is your guess?");

                    #endregion

                    do
                    {
                        #region Guess checking
                        if (possibleWords.Count == 1 && p == Playing.PlayerTwo)
                        {
                            guess = possibleWords[0].ToCharArray();
                            break;
                        }
                        guess = Console.ReadLine()?.ToLower().ToCharArray();
                        if (guess.Length <= 0) continue;
                        if (guess.Length < 2)
                        {
                            if (!guesses.Contains(guess[0])) break;
                        }
                        else
                        {
                            if (!guessedWords.Contains(new string(guess))) break;
                        }

                        #endregion
                    } while (true);

                    if (guess.Length < 2)
                    {
                        #region Letter guess

                        for (var i = 0; i < wordToFind.Length; i++)
                            if (wordToFind[i] == guess[0])
                            {
                                found = true;
                                masked[i] = guess[0];
                            }

                        if (!found)
                        {
                            fails++;
                            wrongGuesses.Add(guess[0]);
                        }
                        guesses.Add(guess[0]);

                        #endregion
                    }
                    else
                    {
                        #region Word guess

                        if (new string(guess) == new string(wordToFind))
                        {
                            Console.Clear();
                            switch (p)
                            {
                                case Playing.PlayerOne:
                                    Console.WriteLine("Congrats, you guessed the right word!");
                                    s = State.Won;
                                    break;
                                case Playing.PlayerTwo:
                                    Console.WriteLine("Too bad, you lost...");
                                    s = State.Lost;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            guessedWords.Add(new string(guess));
                            fails++;
                        }

                        #endregion
                    }

                    if (fails == 10) s = State.Lost;
                    if (new string(masked) == new string(wordToFind) && p == Playing.PlayerOne) s = State.Won;
                    if (new string(masked) == new string(wordToFind) && p == Playing.PlayerTwo) s = State.Lost;
                    p = p == Playing.PlayerOne ? Playing.PlayerTwo : Playing.PlayerOne;
                } while (s == State.Playing);

                #region Game end

                Console.Clear();
                Console.WriteLine("The word was: " + new string(wordToFind));
                Console.WriteLine(s == State.Won ? "You won!" : "Better luck next time!");
                Console.WriteLine("Want to play another game? Type Y or N:");
                do
                {
                    input = Console.ReadLine()?.ToLower();
                    if (input == "y") break;
                    if (input == "n") break;
                } while (true);

                if (input != "n") continue;
                Console.WriteLine("Thanks for playing, see you next time!");
                break;

                #endregion

            } while (true);

            #endregion

            Console.ReadLine();
        }

        #region Enums

        private enum State
        {
            Playing,
            Won,
            Lost
        }

        private enum Playing
        {
            PlayerOne,
            PlayerTwo,
            Computer
        }

        #endregion
    }
}