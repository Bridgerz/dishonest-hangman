using System;


namespace dishonest_hangman
{
    class Program
    {
        static void Main(string[] args)
        {
            var Game = new Game();

            Console.Clear();
            Console.WriteLine("Welcome To Hangman!");
            Console.WriteLine("Press Enter to start.");
            Console.ReadLine();

            Game.Start();
        }
    }
}