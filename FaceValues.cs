namespace csBasicDeck
{
    /// <summary>
    /// Will allow you to assign different integer values to the face cards,
    /// if you so choose. Remember to build a new deck after modifying with
    /// BasicDeck.BuildNewDeck()
    /// </summary>
    public struct FaceValues
    {
        public static int ValueCardMin = 2;
        public static int ValueCardMax = 10;
        public const string AceName = "Ace";
        public const string JackName = "Jack";
        public const string QueenName = "Queen";
        public const string KingName = "King";
        public const string JokerName = "Joker";
        public static int JackValue = 11;
        public static int QueenValue = 12;
        public static int KingValue = 13;
        public static int AceValue = 14;
        public static int JokerValue = 0;
    }
}
