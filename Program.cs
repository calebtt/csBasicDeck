using System;
using System.Collections.Generic;
using System.Text;

namespace csBasicDeck
{
    internal class Program
    {
        private static void InitialTesting(BasicDeck bd, out BasicDeck handOne, out BasicDeck handTwo)
        {
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
            handOne = new(bd.CreateHandFromTop(5));
            handTwo = new(bd.CreateHandFromTop(5));
            Console.WriteLine("\nSample draw for hand one has the cards:");
            handOne.Cards.PrintToConsole();
            Console.WriteLine("\nSample draw for hand two has the cards:");
            handTwo.Cards.PrintToConsole();
        }
        public static void Main(string[] args)
        {
            BasicDeck bd = new();
            BasicDeck handOne;
            BasicDeck handTwo;
            InitialTesting(bd, out handOne, out handTwo);

            //now for some card game type logic
            PrintWithColor("\nBeginning game of Five Card Flush. Flush of any kind wins the game instantly, most of a kind takes the round.");
            PrintWithColor("10 rounds max, two of a kind takes that total value for scoring and so on. Total score wins if no flush encountered.");
            PokerFiveCard pk = new PokerFiveCard();
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;
            for (int i = 0; i < 10; i++)
            {
                //if there is a flush type detected, notify user and return
                bool winOne = ComputeFlushAndPrint(pk, handOne.Cards, "one");
                bool winTwo = ComputeFlushAndPrint(pk,handTwo.Cards, "two");
                if (winOne || winTwo)
                {
                    PrintWithColor("\nFlush!", ConsoleColor.Red);
                    if (winOne && winTwo)
                    {
                        PrintWithColor("TIE! Player one and two tied.");
                        return;
                    }
                    else if (winOne)
                    {
                        PrintWithColor("Winner! Player one.");
                        return;
                    }
                    else
                    {
                        PrintWithColor("Winner! Player two.");
                        return;
                    }
                }
                //count pairs
                var handOnePairs = pk.GetCardPairs(handOne.Cards);
                var handTwoPairs = pk.GetCardPairs(handTwo.Cards);
                ComputeScoreAndPrint("one", handOnePairs, ref firstPlayerScore);
                ComputeScoreAndPrint("two", handTwoPairs, ref secondPlayerScore);
                //rebuild deck and shuffle before dealing again
                bd.BuildNewDeck();
                bd.Shuffle();
                handOne = new(bd.CreateHandFromTop(5));
                handTwo = new(bd.CreateHandFromTop(5));
                List<Card> handOneCards = handOne.GetCopyOfCards();
                List<Card> handTwoCards = handTwo.GetCopyOfCards();
            }
            //compare scores to see who won
            if (firstPlayerScore == secondPlayerScore)
            {
                StringBuilder sb = new();
                sb.AppendFormat("TIED: duplicate score of {0}", firstPlayerScore);
                PrintWithColor(sb.ToString(), ConsoleColor.DarkYellow);
                return;
            }
            else if (firstPlayerScore > secondPlayerScore)
            {
                StringBuilder sb = new();
                sb.AppendFormat("Winner! Player one. Score: {0}", firstPlayerScore);
                PrintWithColor(sb.ToString(), ConsoleColor.DarkYellow);
                return;
            }
            else
            {
                StringBuilder sb = new();
                sb.AppendFormat("Winner! Player two. Score: {0}", secondPlayerScore);
                PrintWithColor(sb.ToString(), ConsoleColor.DarkYellow);
                return;
            }
        }

        public static bool ComputeFlushAndPrint(PokerFiveCard pk, List<Card> hand, string player)
        {
            if (TestFlushes(pk, hand))
            {
                string msg = "\nFlush of " + hand[0].Suit + "  on hand " + player + " !";
                PrintWithColor(msg);
                Console.WriteLine("Cards in hand: ");
                hand.PrintToConsole();
                return true;
            }

            return false;
        }
        public static void ComputeScoreAndPrint(string player, List<List<Card>> dupes, ref int currentScore)
        {
            foreach (var p in dupes)
            {
                if (p.Count > 1)
                {
                    currentScore += p.Count * p[0].IntegerValue; // adding score
                    Console.WriteLine("Hand {2} has {0} {1}'s of a kind.", p.Count, p[0].StringValue, player);
                }
            }
        }
        public static bool TestFlushes(PokerFiveCard pk, List<Card> handCards)
        {
            if (pk.IsRoyalFlush(handCards))
                return true;
            if (pk.IsStraightFlush(handCards))
                return true;
            if (pk.IsFlush(handCards))
                return true;
            return false;
        }

        public static void PrintWithColor(string message, ConsoleColor c = ConsoleColor.Green)
        {
            ConsoleColor cur = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(message);
            Console.ForegroundColor = cur;
        }
    }
}
