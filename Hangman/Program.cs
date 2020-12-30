using System;
using System.Collections.Generic;
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
                string input;
                var s = State.Playing;
                Playing p;
                var r = new Random(Guid.NewGuid().GetHashCode());
                int hOrT = r.Next(0, 2), fails = 0;
                var wordToFind = Words[r.Next(Words.Length)].ToLower().ToCharArray();
                var masked = new char[wordToFind.Length];
                var guessedWords = new List<string>();
                var guesses = new List<char>();

                for (var i = 0; i < wordToFind.Length; i++) masked[i] = '_';

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

                #endregion

                do
                {
                    char[] guess;
                    Console.Clear();
                    var found = false;
                    Console.WriteLine(Drawings[fails]);
                    Console.WriteLine("----------");
                    Console.WriteLine(masked);
                    Console.WriteLine("----------");
                    Console.WriteLine("Guessed letters: " + string.Join(" ", guesses));
                    Console.WriteLine("Guessed words: " + string.Join(" ", guessedWords));
                    //Console.WriteLine(new String(wordToFind));
                    Console.WriteLine("----------");
                    Console.WriteLine(p == Playing.PlayerOne
                        ? "You're up, good luck."
                        : "Player two? What is your guess?");
                    do
                    {
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
                    } while (true);

                    if (guess.Length < 2)
                    {
                        for (var i = 0; i < wordToFind.Length; i++)
                            if (wordToFind[i] == guess[0])
                            {
                                found = true;
                                masked[i] = guess[0];
                            }

                        if (!found) fails++;
                        guesses.Add(guess[0]);
                    }
                    else
                    {
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
                    }

                    if (fails == 10) s = State.Lost;
                    if (new string(masked) == new string(wordToFind) && p == Playing.PlayerOne) s = State.Won;
                    if (new string(masked) == new string(wordToFind) && p == Playing.PlayerTwo) s = State.Lost;
                    p = p == Playing.PlayerOne ? Playing.PlayerTwo : Playing.PlayerOne;
                } while (s == State.Playing);
                Console.Clear();
                Console.WriteLine(s == State.Won ? "You won!" : "Better luck next time!");
                break;
            } while (true);

            #endregion

            Console.ReadLine();
        }

        private enum State
        {
            Playing,
            Won,
            Lost
        }

        private enum Playing
        {
            PlayerOne,
            PlayerTwo
        }
    }
}