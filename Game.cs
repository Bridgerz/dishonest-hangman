using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dishonest_hangman
{
    public class Game
    {
        private List<String> Words;
        private String Word;
        private List<String> SimWords;
        private char[] CorrectGuesses;
        private List<char> IncorrectGuesses;
        private int GameState;

        public Game()
        {
            this.Words = new List<String>();
            this.SimWords = new List<String>();
            this.IncorrectGuesses = new List<char>();
            this.GameState = Const.NEXTGUESS;

            // Initialize Words List with all words with valid letters
            var arr = System.IO.File.ReadAllLines("./words");
            foreach (var word in arr)
            {
                if (!word.ToCharArray().Contains('\'') && Regex.IsMatch(word, "[a-zA-Z]"))
                    Words.Add(word.ToLower());
            }
        }

        public void Start()
        {
            // Get Random Word
            var rand = new Random();
            this.Word = Words[rand.Next(this.Words.Count)];

            this.Word = "xylophonist";

            this.CorrectGuesses = new char[this.Word.Length];
            for (int i = 0; i < this.CorrectGuesses.Length; i++)
                this.CorrectGuesses[i] = '_';

            this.SimWords = this.Words.Where(x => x.Length == this.Word.Length).ToList();

            while (!this.TakeTurn()) { }

            // End Game
            if (this.IncorrectGuesses.Count >= Const.MAXGUESSES)
            {
                this.GameState = Const.LOSE;
                return;
            }
            this.GameState = Const.WIN;
            this.PrintStatus();
        }

        private bool TakeTurn()
        {
            this.PrintStatus();

            var guess = this.GetGuess();
            if (this.CorrectGuesses.Contains(guess) || this.IncorrectGuesses.Contains(guess))
            {
                this.GameState = Const.ALREADYGUESSED;
                return false;
            }
            this.GameState = Const.NEXTGUESS;

            this.TryCheat(guess);

            // end if all characters have been guessed or max guesses has been reached
            return this.CorrectGuesses.Count(letter => letter != '_') == this.Word.ToList().Count || this.IncorrectGuesses.Count >= Const.MAXGUESSES;
        }

        private bool TryCheat(char guess)
        {
            if (this.SimWords.Count == 1)
                return this.Cornered(guess);

            var (countWith, charIndex) = this.MostSubWordsWith(guess);
            var wordsWithout = this.SimWords.Where(word => !word.Contains(guess));

            // must give character with more than 1 occurance
            if (countWith == 0 && wordsWithout.Count() == 0)
            {
                this.Word = this.SimWords[0];
                for (int i = 0; i < this.Word.Length; i++)
                {
                    if (this.Word[i] == guess)
                    {
                        this.SimWords = this.SimWords.Where(word => word[i] == guess).ToList();
                        this.CorrectGuesses[i] = guess;
                    }
                }
                return false;
            }

            // more "sub" words if don't accept guessed letter
            if (countWith <= wordsWithout.Count())
            {
                this.IncorrectGuesses.Add(guess);
                this.SimWords = wordsWithout.ToList();
                this.Word = this.SimWords[0];
                return true;
            }

            // more "sub words if accept guessed letter
            this.CorrectGuesses[charIndex] = guess;
            this.SimWords = this.SimWords.Where(word =>
                (word.ElementAt(charIndex) == guess) &&
                (word.ToCharArray().Count(letter => letter == guess) == 1)
            ).ToList();
            this.Word = this.SimWords[0];
            return false;
        }

        private (int, int) MostSubWordsWith(char guess)
        {
            var countWith = 0;
            var index = 0;
            for (int i = 0; i < this.Word.Length; i++)
            {
                // guessed letter can not be at this index
                if (this.CorrectGuesses[i] != '_')
                    continue;
                // query words with guess character at index i, that only contain 1 occurance of character.
                //      (giving a character with more than one occurance gives player 2 more advantage)
                var curCountWith = this.SimWords.Where(word =>
                    (word.ElementAt(i) == guess) &&
                    (word.ToCharArray().Count(letter => letter == guess) == 1)
                ).Count();
                if (curCountWith > countWith)
                {
                    countWith = curCountWith;
                    index = i;
                }
            }
            return (countWith, index);
        }

        private char GetGuess()
        {
            var guess = ' ';
            while (guess == ' ')
            {
                var input = Console.ReadLine();
                //check input is a single character
                if (input != "" && input != null && Regex.IsMatch(input, "[a-zA-Z]"))
                {
                    guess = Char.ToLower(input[0]);
                    break;
                }
            }
            return guess;
        }

        private bool Cornered(char guess)
        {
            if (!this.Word.Contains(guess))
                return true;

            for (int i = 0; i < this.Word.Length; i++)
            {
                if (Word[i] == guess)
                    this.CorrectGuesses[i] = guess;
            }
            return false;
        }

        private void PrintStatus()
        {
            //Console.Clear();
            Console.WriteLine("The first word to guess is: " + this.Word);
            Console.WriteLine("Number of similar words: " + this.SimWords.Count());
            Console.Write("Guessed Letters: ");

            foreach (var letter in this.IncorrectGuesses)
                Console.Write(" " + letter);

            Console.WriteLine();
            Console.WriteLine("Remaining Incorrect Guesses: " + (Const.MAXGUESSES - this.IncorrectGuesses.Count));

            foreach (var letter in this.CorrectGuesses)
                Console.Write(letter + " ");


            Console.WriteLine();
            Console.WriteLine();
            switch (this.GameState)
            {
                case Const.NEXTGUESS:
                    Console.WriteLine("Please provide your next guess...");
                    break;
                case Const.ALREADYGUESSED:
                    Console.WriteLine("You have already guessed this letter...");
                    break;
                case Const.LOSE:
                    Console.WriteLine("Better luck next time!");
                    break;
                case Const.WIN:
                    Console.WriteLine("You Win!");
                    break;
            }
        }
    }
}