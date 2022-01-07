using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csConsoleApp
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Some setup and testing...");
            BasicDeck bd = new();
            Console.WriteLine("Num Cards: " + bd.Size());
            int sz = bd.Size();
            bd.Shuffle();
            for (int i = 0; i < sz; i++)
            {
                Console.Write("[{0}] ",i);
                Console.WriteLine(bd.RemoveFromTop());
            }
            //now we build a deck and assign 2 hands from it
            bd.BuildNewDeck();
            //a hand is it's own deck..
            BasicDeck handOne = new (bd.CreateHandFromTop(5));
            BasicDeck handTwo = new (bd.CreateHandFromTop(5));
            //can copy the cards in the hand for operations.
            List<Card> handOneCards = handOne.GetCopyOfCards();
            List<Card> handTwoCards = handTwo.GetCopyOfCards();
            Console.WriteLine();
            Console.WriteLine("Sample draw for hand one has the cards:");
            handOneCards.PrintToConsole();
            Console.WriteLine();
            Console.WriteLine("Sample draw for hand two has the cards:");
            handTwoCards.PrintToConsole();
            //now for some card game type logic
            Console.WriteLine();
            PrintWithColor("Beginning game of Five Card Flush. Flush of any kind wins the game instantly, most of a kind takes the round.");
            PrintWithColor("10 rounds max, two of a kind takes that total value for scoring and so on. Total score wins if no flush encountered.");
            PokerFiveCard pk = new PokerFiveCard();
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;
            for (int i = 0; i < 10; i++)
            {
                //if there is a flush type detected, notify user and return
                bool winOne = ComputeFlushAndPrint(pk, handOneCards, "one");
                bool winTwo = ComputeFlushAndPrint(pk,handTwoCards, "two");
                if (winOne || winTwo)
                {
                    if (winOne && winTwo)
                    {
                        Console.WriteLine("TIE! Player one and two tied.");
                        return;
                    }
                    else if (winOne)
                    {
                        Console.WriteLine("Winner! Player one.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Winner! Player two.");
                        return;
                    }
                }
                //count pairs
                var handOnePairs = pk.GetCardPairs(handOneCards);
                var handTwoPairs = pk.GetCardPairs(handTwoCards);
                ComputeScoreAndPrint("one", handOnePairs, ref firstPlayerScore);
                ComputeScoreAndPrint("two", handTwoPairs, ref secondPlayerScore);
                //rebuild deck and shuffle before dealing again
                bd.BuildNewDeck();
                bd.Shuffle();
                handOne = new(bd.CreateHandFromTop(5));
                handTwo = new(bd.CreateHandFromTop(5));
                handOneCards = handOne.GetCopyOfCards();
                handTwoCards = handTwo.GetCopyOfCards();
            }
            //compare scores to see who won
            if (firstPlayerScore == secondPlayerScore)
            {
                Console.WriteLine("TIED: duplicate score of {0}", firstPlayerScore);
                return;
            }
            else if (firstPlayerScore > secondPlayerScore)
            {
                Console.WriteLine("Winner! Player one. Score: {0}", firstPlayerScore);
                return;
            }
            else
            {
                Console.WriteLine("Winner! Player two. Score: {0}", secondPlayerScore);
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

        public static void PrintWithColor(string message)
        {
            ConsoleColor cur = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = cur;
        }
    }
}
