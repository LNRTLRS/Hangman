using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
        
        private static readonly string[] Words = File.ReadAllLines("../../../wordlist.txt");

        private static void Main()
        {
            string intro = @" __    __  ______  __    __  ______  __       __  ______  __    __ 
|  \  |  \/      \|  \  |  \/      \|  \     /  \/      \|  \  |  \
| ▓▓  | ▓▓  ▓▓▓▓▓▓\ ▓▓\ | ▓▓  ▓▓▓▓▓▓\ ▓▓\   /  ▓▓  ▓▓▓▓▓▓\ ▓▓\ | ▓▓
| ▓▓__| ▓▓ ▓▓__| ▓▓ ▓▓▓\| ▓▓ ▓▓ __\▓▓ ▓▓▓\ /  ▓▓▓ ▓▓__| ▓▓ ▓▓▓\| ▓▓
| ▓▓    ▓▓ ▓▓    ▓▓ ▓▓▓▓\ ▓▓ ▓▓|    \ ▓▓▓▓\  ▓▓▓▓ ▓▓    ▓▓ ▓▓▓▓\ ▓▓
| ▓▓▓▓▓▓▓▓ ▓▓▓▓▓▓▓▓ ▓▓\▓▓ ▓▓ ▓▓ \▓▓▓▓ ▓▓\▓▓ ▓▓ ▓▓ ▓▓▓▓▓▓▓▓ ▓▓\▓▓ ▓▓
| ▓▓  | ▓▓ ▓▓  | ▓▓ ▓▓ \▓▓▓▓ ▓▓__| ▓▓ ▓▓ \▓▓▓| ▓▓ ▓▓  | ▓▓ ▓▓ \▓▓▓▓
| ▓▓  | ▓▓ ▓▓  | ▓▓ ▓▓  \▓▓▓\▓▓    ▓▓ ▓▓  \▓ | ▓▓ ▓▓  | ▓▓ ▓▓  \▓▓▓
 \▓▓   \▓▓\▓▓   \▓▓\▓▓   \▓▓ \▓▓▓▓▓▓ \▓▓      \▓▓\▓▓   \▓▓\▓▓   \▓▓";
            Console.WriteLine(intro + "\n");

            #region Game

            var scores = new Dictionary<Playing, int>()
            {
                {Playing.Human, 0},
                {Playing.Computer, 0}
            };

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
                } while (wordToFind.Length < 4 || wordToFind.Length > 14);
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
                    p = Playing.Human;
                    Console.WriteLine("You guessed correctly, you may start.");
                }
                else if (input == "H" || input == "T")
                {
                    p = Playing.Computer;
                    Console.WriteLine("Too bad, the computer is allowed to begin.");
                }
                else
                {
                    p = Playing.Computer;
                    Console.WriteLine("You didn't enter a valid choice. The computer is going first now.");
                }

                Console.WriteLine("Press any key to start the game");
                Console.ReadLine();

                #endregion

                do
                {
                    char[] guess;
                    var charCount = new Dictionary<char, int>();
                    var orderedCount = new Dictionary<char, int>();
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

                    foreach (var word in possibleWords)
                    {
                        foreach (var c in word)
                        {
                            if (guesses.Contains(c)) continue;
                            if (charCount.ContainsKey(c))
                            {
                                charCount[c]++;
                            }
                            else
                            {
                                charCount.Add(c, 1);
                            }
                        }
                    }


                    #endregion

                    #region Console Writelines

                    Console.WriteLine(Drawings[fails]);
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("The word: " + new string(masked) + " (" + masked.Length + " characters)");
                    Console.WriteLine("--------------------------");
                    Console.WriteLine("Guessed letters: " + string.Join(" ", guesses));
                    Console.WriteLine("Guessed words: " + string.Join(" ", guessedWords));
                    Console.WriteLine("--------------------------");
                    foreach (var (key, value) in charCount.OrderByDescending(key => key.Value))
                    {
                        orderedCount.Add(key, value);
                    }
                    Console.WriteLine(p == Playing.Human
                        ? "You're up, good luck."
                        : "The computer is thinking...");

                    #endregion

                    do
                    {
                        #region Guess checking

                        if (p == Playing.Computer)
                        {
                            if (possibleWords.Count == 1)
                            {
                                guess = possibleWords[0].ToCharArray();
                                Thread.Sleep(1500);
                                break;
                            }
                            var t = orderedCount.FirstOrDefault().Key.ToString();
                            guess = t.ToCharArray();
                            Thread.Sleep(1500);
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
                                scores[p]++;
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
                            scores[p] += 5;
                            switch (p)
                            {
                                case Playing.Human:
                                    Console.WriteLine("Congrats, you guessed the right word!");
                                    s = State.Won;
                                    break;
                                case Playing.Computer:
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
                    if (new string(masked) == new string(wordToFind) && p == Playing.Human) s = State.Won;
                    if (new string(masked) == new string(wordToFind) && p == Playing.Computer) s = State.Lost;
                    p = p == Playing.Human ? Playing.Computer : Playing.Human;
                } while (s == State.Playing);

                #region Game end

                Console.Clear();
                Console.WriteLine("The word was: " + new string(wordToFind));
                Console.WriteLine(s == State.Won ? "You won!" : "Better luck next time!");
                Console.WriteLine("You scored " + scores[Playing.Human] + " points so far, while the computer scored " + scores[Playing.Computer] + " points.");
                Console.WriteLine("Want to play another game? Type Y or N:");
                do
                {
                    input = Console.ReadLine()?.ToLower();
                    if (input == "y")
                    {
                        Console.Clear();
                        break;
                    }
                    if (input == "n") break;
                } while (true);

                if (input != "n") continue;
                Console.Clear();
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
            Human,
            Computer
        }

        #endregion
    }
}