using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace csBasicDeck
{
    internal class Program
    {
        /// <summary> Struct to hold some program settings.
        /// Can be extended or passed into the program from another source,
        /// if desired. </summary>
        struct ProgSettings
        {
            //some program settings
            public const int NumRoundsFiveCard = 10;
            public const string FirstPlayerName = "[Player One]";
            public const string SecondPlayerName = "[Player Two]";
            public const int NumCardsInHand = 5;
            public const ConsoleColor TieColor = ConsoleColor.DarkCyan;
            public const ConsoleColor FlushColor = ConsoleColor.Red;
            public const ConsoleColor FlushTieColor = ConsoleColor.Magenta;
            public const ConsoleColor WinColor = ConsoleColor.DarkYellow;
        }
        /// <summary> Stores some info about the win type.
        /// Such as is it a tie, or a flush or both or neither. </summary>
        struct WinInfo
        {
            /// <summary> The type of flush the winner gained. </summary>
            public enum FlushType
            {
                None,
                StraightFlush,
                RoyalFlush,
                RegularFlush
            }
            /// <summary> If two or more of the players have identical high scores, returns true. </summary>
            public bool IsTie
            {
                get
                {
                    //find high score
                    int maxScore = PlayerScores.Max();
                    var maxScoreCount = PlayerScores.FindAll(sc => sc == maxScore);
                    return (maxScoreCount.Count > 1);
                }
            }
            /// <summary> If one or more of the players achieved a flush, returns true. </summary>
            public bool IsFlush
            {
                get
                {
                    foreach (FlushType ft in PlayerFlushes)
                    {
                        if (ft != FlushType.None)
                            return true;
                    }
                    return false;
                }
            }
            public List<int> PlayerScores { get; }
            public List<FlushType> PlayerFlushes { get; }
            public WinInfo(List<FlushType> playerFlushes, List<int> playerScores)
            {
                PlayerFlushes = playerFlushes;
                PlayerScores = playerScores;
            }
        }

        //
        // Entry-point for basic deck example
        //
        public static void Main(string[] args)
        {
            InitialTesting();

            while (true)
            {
                //show game message
                PrintWithColor(
                    "\nBeginning game of Five Card Flush. Flush of any kind wins the game instantly, most of a kind takes the round.");
                PrintWithColor(
                    "10 rounds max, two of a kind takes that total value for scoring and so on. Total score wins if no flush encountered.");
                //read key to begin
                Console.WriteLine("Press any key to begin... (Q to exit)");
                var exitKey = Console.ReadKey();
                if (Char.ToUpper(exitKey.KeyChar) == 'Q')
                    return;
                //now for some card game type logic, a loop of draws
                WinInfo winnerInfo = RunRoundLoop();
                //scoring info
                int firstPlayerScore = winnerInfo.PlayerScores[0];
                int secondPlayerScore = winnerInfo.PlayerScores[1];
                if (winnerInfo.IsFlush)
                {
                    PrintFlushWinInfo(winnerInfo.PlayerFlushes[0], winnerInfo.PlayerFlushes[1]);
                    continue;
                }

                //compare scores to see who won
                StringBuilder sb = new();
                if (firstPlayerScore == secondPlayerScore)
                {
                    sb.AppendFormat("TIED: duplicate score of {0}", firstPlayerScore);
                    PrintWithColor(sb.ToString(), ProgSettings.TieColor);
                    continue;
                }

                //else
                string winner = firstPlayerScore > secondPlayerScore
                    ? ProgSettings.FirstPlayerName
                    : ProgSettings.SecondPlayerName;
                int winningScore = firstPlayerScore > secondPlayerScore ? firstPlayerScore : secondPlayerScore;
                sb.AppendFormat("Winner! {0}. Score: {1}", winner, winningScore);
                PrintWithColor(sb.ToString(), ProgSettings.WinColor);
            }
        }
        /// <summary> Builds a new deck for each hand, printing the cards in the deck for inspection.
        /// Performs a sample drawing of cards from the deck into a 5 card hand. </summary>
        /// <param name="bd">Existing deck of cards, will be replaced with a new deck after the test</param>
        /// <param name="handOne">Deck to store the hand cards in for player1</param>
        /// <param name="handTwo">Deck to store the hand cards in for player2</param>
        private static void InitialTesting()
        {
            BasicDeck bd = new();
            BasicDeck handOne;
            BasicDeck handTwo;
            Console.WriteLine("Some setup and testing...");
            Console.WriteLine("Num Cards: " + bd.Size());
            int sz = bd.Size();
            bd.Shuffle();
            for (int i = 0; i < sz; i++)
            {
                Console.Write("[{0}] ", i);
                Console.WriteLine(bd.RemoveFromTop());
            }
            //now we build a deck and assign 2 hands from it
            bd.BuildNewDeck();
            //a hand is it's own deck..
            handOne = new(bd.CreateHandFromTop(ProgSettings.NumCardsInHand));
            handTwo = new(bd.CreateHandFromTop(ProgSettings.NumCardsInHand));
            Console.WriteLine("\nSample draw for hand one has the cards:");
            handOne.Cards.PrintToConsole();
            Console.WriteLine("\nSample draw for hand two has the cards:");
            handTwo.Cards.PrintToConsole();
        }
        /// <summary> Runs a match of the game, ProgSettings.NumRoundsFiveCard rounds,
        /// returns information about the win and scores. Program specific logic for two players. </summary>
        /// <param name="handOne">Hand one cards</param>
        /// <param name="handTwo">Hand two cards</param>
        /// <param name="bd">Deck object for drawing from</param>
        /// <returns>WinInfo struct with relevant info</returns>
        private static WinInfo RunRoundLoop()
        {
            BasicDeck bd = new(); // deck management object, creation, shuffling, drawing, etc.
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;
            PokerFiveCard pk = new PokerFiveCard(ProgSettings.NumCardsInHand); // game logic obj
            //ten round loop as of now
            for (int i = 0; i < ProgSettings.NumRoundsFiveCard; i++)
            {
                //rebuild deck and shuffle before dealing
                bd.BuildNewDeck();
                BasicDeck handOne = new(bd.CreateHandFromTop(ProgSettings.NumCardsInHand));
                BasicDeck handTwo = new(bd.CreateHandFromTop(ProgSettings.NumCardsInHand));
                //if there is a flush type detected, notify user and return
                WinInfo.FlushType flushWinOne = TestFlushes(pk, handOne.Cards);
                WinInfo.FlushType flushWinTwo = TestFlushes(pk, handTwo.Cards);
                if (flushWinOne != WinInfo.FlushType.None || flushWinTwo != WinInfo.FlushType.None)
                {
                    return new WinInfo(new() {flushWinOne, flushWinTwo}, 
                        new(){firstPlayerScore, secondPlayerScore});
                }
                //count pairs
                var handOnePairs = pk.GetCardPairs(handOne.Cards);
                var handTwoPairs = pk.GetCardPairs(handTwo.Cards);
                ComputeScoreAndPrint(ProgSettings.FirstPlayerName, handOnePairs, ref firstPlayerScore, i+1);
                ComputeScoreAndPrint(ProgSettings.SecondPlayerName, handTwoPairs, ref secondPlayerScore, i+1);
            }

            return new WinInfo(new() {WinInfo.FlushType.None, WinInfo.FlushType.None},
                new(){firstPlayerScore, secondPlayerScore});
        }
        /// <summary> Prints to the console info about the flush win (if present) with colored text.
        /// Program specific logic, two players only. </summary>
        private static void PrintFlushWinInfo(WinInfo.FlushType flushWinOne, WinInfo.FlushType flushWinTwo)
        {
            if (flushWinOne != WinInfo.FlushType.None || flushWinTwo != WinInfo.FlushType.None)
            {
                PrintWithColor("\nFlush!", ProgSettings.FlushColor);
                //handle both hands winning flush case
                if (flushWinOne != WinInfo.FlushType.None && flushWinTwo != WinInfo.FlushType.None)
                {
                    PrintWithColor($"TIE! {ProgSettings.FirstPlayerName} and {ProgSettings.SecondPlayerName} tied.", ProgSettings.FlushTieColor);
                    return;
                }
                //handle single hand winning flush case
                string winner = flushWinOne != WinInfo.FlushType.None ? ProgSettings.FirstPlayerName : ProgSettings.SecondPlayerName;
                PrintWithColor($"Winner! {winner}.", ProgSettings.FlushColor);
            }
        }

        /// <summary> Helper func for printing (to console) the suit of the flush and the player name. </summary>
        /// <param name="hand">Deck with hand cards</param>
        /// <param name="playerName">Name of player associated with hand</param>
        public static void PrintFlushSuit(List<Card> hand, string playerName)
        {
            string msg = "\nFlush of " + hand[0].Suit + "  on hand " + playerName + " !";
            PrintWithColor(msg);
            Console.WriteLine("Cards in hand: ");
            hand.PrintToConsole();
        }
        /// <summary> Computes the score of the hand (pairs) and updates the running total,
        /// prints a message if more than one of a kind is found. </summary>
        /// <param name="player">Name of player running the test for</param>
        /// <param name="dupes">List of List-Card showing the duplicates (pairs) in the hand</param>
        /// <param name="currentScore">ref param for running score total</param>
        /// <param name="currentRound">current round hand number for the match</param>
        private static void ComputeScoreAndPrint(string player, List<List<Card>> dupes, ref int currentScore, int currentRound)
        {
            foreach (var p in dupes)
            {
                if (p.Count > 1)
                {
                    currentScore += p.Count * p[0].IntegerValue; // adding score
                    Console.WriteLine("Round {3} {2} has Hand {0} {1}'s of a kind.", p.Count, p[0].StringValue, player, currentRound);
                }
            }
        }
        /// <summary> Tests a hand of cards successively for most-rare (highest value) to least-rare
        /// flush variations. </summary>
        /// <param name="pk">A PokerFiveCard logic object</param>
        /// <param name="handCards">Hand of cards (5 cards!)</param>
        /// <returns>FlushType struct denoting straight, royal, or regular flush (or none)</returns>
        private static WinInfo.FlushType TestFlushes(PokerFiveCard pk, List<Card> handCards)
        {
            if (pk.IsRoyalFlush(handCards))
                return WinInfo.FlushType.RoyalFlush;
            if (pk.IsStraightFlush(handCards))
                return WinInfo.FlushType.StraightFlush;
            if (pk.IsFlush(handCards))
                return WinInfo.FlushType.RegularFlush;
            return WinInfo.FlushType.None;
        }
        /// <summary> Utility function used to conveniently print colored messages to the console. </summary>
        /// <param name="message">Message to print</param>
        /// <param name="c">optional color to print with</param>
        private static void PrintWithColor(string message, ConsoleColor c = ConsoleColor.Green)
        {
            ConsoleColor cur = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(message);
            Console.ForegroundColor = cur;
        }
    }
}
