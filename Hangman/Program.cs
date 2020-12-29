using System;

namespace Hangman
{
    class Program
    {
        enum State { Playing, Won, Lost };
        enum Playing { PlayerOne, PlayerTwo }
        public static string[] Words = TM.ProgrammingAdvanced.Data.Words;

        static void Main(string[] args)
        {
            Console.WriteLine("----- HANGMAN -----");

            #region Game
            do
            {
                State s = State.Playing;
                Random r = new Random();
                Char[] wordToFind = Words[r.Next(Words.Length)].ToCharArray();
                Char[] masked = new Char[wordToFind.Length];
                for(int i = 0; i < wordToFind.Length; i++)
                {
                    masked[i] = '_';
                }

                break;
            } while (true);
            #endregion
            Console.ReadLine();
        }
    }
}
