using System;
using System.Collections.Generic;

namespace Hangman
{
    class Program
    {
        #region Hangmen ASCII
        public static string[] Drawings = {
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
        enum State { Playing, Won, Lost };
        enum Playing { PlayerOne, PlayerTwo }
        public static string[] Words = TM.ProgrammingAdvanced.Data.Words;

        static void Main(string[] args)
        {
            Console.WriteLine("----- HANGMAN -----");

            #region Game
            do
            {
                string input;
                State s = State.Playing;
                Playing p;
                Random r = new Random(Guid.NewGuid().GetHashCode());
                int hOrT = r.Next(0, 2), fails = 0;
                Char[] wordToFind = Words[r.Next(Words.Length)].ToLower().ToCharArray();
                Char[] masked = new Char[wordToFind.Length];
                List<String> guessedWords = new List<String>();
                List<Char> guesses = new List<Char>();

                for (int i = 0; i < wordToFind.Length; i++)
                {
                    masked[i] = '_';
                }

                #region Heads or tails
                Console.WriteLine("To decide who is allowed to start, I'll flip a coin.");
                Console.WriteLine("Type H or T to choose the side.");
                input = Console.ReadLine().ToUpper();
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
                    Char[] guess;
                    //Console.Clear();
                    Boolean found = false;
                    Console.WriteLine(Drawings[fails]);
                    Console.WriteLine((p == Playing.PlayerOne) ? "You're up, good luck." : "Player two? What is your guess?");
                    Console.WriteLine(masked);
                    Console.WriteLine("Guessed letters: " + String.Join(" ", guesses));
                    Console.WriteLine("----------");
                    guess = Console.ReadLine().ToLower().ToCharArray();
                    if (guess.Length < 2)
                    {
                        for (int i = 0; i < wordToFind.Length; i++)
                        {
                            if (wordToFind[i] == guess[0])
                            {
                                found = true;
                                masked[i] = guess[0];
                            }
                        }
                        if (!found)
                        {
                            fails++;
                        }
                        guesses.Add(guess[0]);
                    } else
                    {
                        Console.Write(guess.ToString() + " " + wordToFind.ToString());
                        if(guess == wordToFind)
                        {
                            Console.WriteLine((p == Playing.PlayerOne) ? "Congrats, you guessed the right word!" : "Too bad, player two won.");
                        } else
                        {
                            guessedWords.Add(guess.ToString());
                        }
                    }
                } while (fails < 11);
                break;
            } while (true);
            #endregion
            Console.ReadLine();
        }
    }
}
