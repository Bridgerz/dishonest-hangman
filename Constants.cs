using System;


namespace dishonest_hangman
{
    public class GameState
    {
        public const int NEXTGUESS = 0;
        public const int ALREADYGUESSED = 1;
        public const int LOSE = 2;
        public const int WIN = 3;
    }
}