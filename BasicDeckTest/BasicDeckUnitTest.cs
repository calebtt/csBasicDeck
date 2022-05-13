using System;
using System.Collections.Generic;
using System.Drawing;
using csBasicDeck;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BasicDeckTest
{
    [TestClass]
    public class BasicDeckUnitTest
    {
        private const int NumCardsInHand = 5;
        //shared instance between tests
        readonly PokerFiveCard pk = new(NumCardsInHand);
        readonly BasicDeck bd = new();

        [TestMethod]
        public void TestBasicDeckHands()
        {
            Console.WriteLine("Num Cards: " + bd.Size());
            int sz = bd.Size();
            bd.Shuffle();
            for (int i = 0; i < sz; i++)
            {
                Console.Write("[{0}] ", i);
                Console.WriteLine(bd.RemoveFromTop());
            }
            //Some quick testing...
            bd.BuildNewDeck();
            Action<List<Card>, string, string> PrintLambda = (List<Card> l, string msg, string expect) =>
            {
                Assert.IsTrue(l.Count.ToString() == expect, msg); // the string comp is just convenient
            };
            List<Card> handOne = bd.CreateHandFromTop(5);
            PrintLambda(handOne, "one", "5");
            List<Card> handTwo = bd.CreateHandFromTop(5);
            PrintLambda(handTwo, "two", "5");
            List<Card> handRest = bd.CreateHandFromTop(42);
            PrintLambda(handRest, "rest", "42");
            List<Card> handWoops = bd.CreateHandFromTop(-5);
            PrintLambda(handWoops, "woops", "0");
            bd.BuildNewDeck();
            List<Card> handOneBottom = bd.CreateHandFromBottom(5);
            PrintLambda(handOneBottom, "oneb", "5");
            List<Card> handTwoBottom = bd.CreateHandFromBottom(5);
            PrintLambda(handTwoBottom, "twob", "5");
            List<Card> handRestBottom = bd.CreateHandFromBottom(42);
            PrintLambda(handRestBottom, "restb", "42");
            List<Card> handWoopsBottom = bd.CreateHandFromBottom(-5);
            PrintLambda(handWoopsBottom, "woopsb", "0");
        }

        [TestMethod]
        public void TestIsRoyalFlush()
        {
            string testName = "In: " + nameof(TestIsRoyalFlush);
            //the odds of a royal flush occurring in randomly generated card decks is
            //649,739 : 1 against, source: https://en.wikipedia.org/wiki/Poker_probability#:~:text=Frequency%20of%205-card%20poker%20hands%20%20%20,693.1667%20%3A%201%20%207%20more%20rows%20
            //so we will be manually creating a royal flush to test
            List<Card> flushHand = BuildRoyalFlushHand();
            Assert.IsTrue(pk.IsRoyalFlush(flushHand), testName + "Assert: PokerFiveCard.IsRoyalFlush() returns true with known good flush hand in the correct order.");
            bd.Shuffle(flushHand); // randomized order of royal flush hand
            Assert.IsTrue(pk.IsRoyalFlush(flushHand), testName + "Assert: PokerFiveCard.IsRoyalFlush() returns true with known good flush hand in bad order.");
            //test a hand that is not a royal flush
            flushHand = new()
            {
                new Card() { CardColor = Color.Black, IntegerValue = 3, StringValue = "3", Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.KingValue, StringValue = FaceValues.KingName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.QueenValue, StringValue = FaceValues.QueenName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = 10, StringValue = "10", Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.AceValue, StringValue = FaceValues.AceName, Suit = SuitValues.ClubsName }
            };
            Assert.IsFalse(pk.IsRoyalFlush(flushHand), testName + "Assert: PokerFiveCard.IsRoyalFlush() returns false with bad flush hand.");
            //test a hand that is not a "royal" flush, but is a flush.
            flushHand = BuildStraightFlushHand();
            Assert.IsFalse(pk.IsRoyalFlush(flushHand), testName + "Assert: PokerFiveCard.IsRoyalFlush() returns false with bad *royal* flush hand.");
        }
        [TestMethod]
        public void TestIsStraightFlush()
        {
            string testName = "In: " + nameof(TestIsStraightFlush);
            //the odds of a straight flush occurring in randomly generated card decks is
            //72,192 : 1 against, source: https://en.wikipedia.org/wiki/Poker_probability#:~:text=Frequency%20of%205-card%20poker%20hands%20%20%20,693.1667%20%3A%201%20%207%20more%20rows%20
            //so we will be manually creating a straight flush to test, a royal flush is also a straight flush.
            List<Card> flushHand = BuildRoyalFlushHand();
            Assert.IsTrue(pk.IsStraightFlush(flushHand), testName + "Assert: PokerFiveCard.IsStraightFlush() returns true with known good flush hand in the correct order.");
            //shuffle and test again
            bd.Shuffle(flushHand);
            Assert.IsTrue(pk.IsStraightFlush(flushHand), testName + "Assert: PokerFiveCard.IsStraightFlush() returns true with known good flush hand in bad order.");
            //and test a normal straight flush
            flushHand = BuildStraightFlushHand();
            Assert.IsTrue(pk.IsStraightFlush(flushHand), testName + "Assert: PokerFiveCard.IsStraightFlush() returns true with known good flush hand in the correct order.");
            //test not a flush
            flushHand = new()
            {
                new Card() { CardColor = Color.Red, IntegerValue = 8, StringValue = "8", Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = 10, StringValue = "10", Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.JackValue, StringValue = FaceValues.JackName, Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.QueenValue, StringValue = FaceValues.QueenName, Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.KingValue, StringValue = FaceValues.KingName, Suit = SuitValues.DiamondsName }
            };
            Assert.IsFalse(pk.IsStraightFlush(flushHand), testName + "Assert: PokerFiveCard.IsStraightFlush() returns false with bad flush hand.");
        }

        [TestMethod]
        public void TestIsFlush()
        {
            string testName = "In: " + nameof(TestIsFlush);
            List<Card> hand = BuildRoyalFlushHand();
            Assert.IsTrue(pk.IsFlush(hand), testName + "Assert: PokerFiveCard.IsFlush() returns true with known good flush hand in the correct order.");
            bd.Shuffle(hand);
            Assert.IsTrue(pk.IsFlush(hand), testName + "Assert: PokerFiveCard.IsFlush() returns true with known good flush hand in bad order.");
            hand = BuildNotAFlushHand();
            Assert.IsFalse(pk.IsFlush(hand), testName + "Assert: PokerFiveCard.IsFlush() returns false with bad flush hand.");
        }

        //Returns a 5 card royal flush in sorted order.
        List<Card> BuildRoyalFlushHand()
        {
            return new()
            {
                new Card() { CardColor = Color.Black, IntegerValue = 10, StringValue = "10", Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.JackValue, StringValue = FaceValues.JackName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.QueenValue, StringValue = FaceValues.QueenName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.KingValue, StringValue = FaceValues.KingName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.AceValue, StringValue = FaceValues.AceName, Suit = SuitValues.ClubsName }
            };
        }

        //Returns a 5 card straight flush in sorted order.
        List<Card> BuildStraightFlushHand()
        {
            return new()
            {
                new Card() { CardColor = Color.Red, IntegerValue = 9, StringValue = "9", Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = 10, StringValue = "10", Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.JackValue, StringValue = FaceValues.JackName, Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.QueenValue, StringValue = FaceValues.QueenName, Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Red, IntegerValue = FaceValues.KingValue, StringValue = FaceValues.KingName, Suit = SuitValues.DiamondsName }
            };
        }

        /// <summary>
        /// Builds and returns a List of Card that is not a flush at all.
        /// </summary>
        List<Card> BuildNotAFlushHand()
        {
            return new()
            {
                new Card() { CardColor = Color.Red, IntegerValue = 3, StringValue = "3", Suit = SuitValues.DiamondsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.KingValue, StringValue = FaceValues.KingName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.QueenValue, StringValue = FaceValues.QueenName, Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = 10, StringValue = "10", Suit = SuitValues.ClubsName },
                new Card() { CardColor = Color.Black, IntegerValue = FaceValues.AceValue, StringValue = FaceValues.AceName, Suit = SuitValues.ClubsName }
            };
        }
    }
}
