﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApp
{
    /// <summary>
    /// Intended to operate on 5 card hands for the scoring of Poker,
    /// and some variations thereof. Throws exception on "hand" with
    /// wrong number of elements.
    /// https://www.pagat.com/poker/rules/ranking.html
    /// </summary>
    public class PokerFiveCard
    {
        private const int HAND_SIZE = 5;
        private const string ERR_HAND_SIZE = "Exception in PokerFiveCard: List<Card> hand param arg has wrong number of elements!";

        private void CheckArgSize(List<Card> hand)
        {
            if (hand == null)
                throw new ArgumentNullException(nameof(hand));
            if (hand.Count != HAND_SIZE)
                throw new ArgumentException(ERR_HAND_SIZE);
        }

        /// <summary>
        /// The highest type of straight flush, A-K-Q-J-10 of a suit, is known as a Royal Flush.
        /// </summary>
        public bool IsRoyalFlush(List<Card> hand)
        {
            CheckArgSize(hand);
            if (!IsFlush(hand)) 
                return false;
            //make copy and sort, do test for flush
            List<Card> local = GetSortedCopy(hand);
            List<int> flushValues = BuildRoyalFlushValues();
            for (int i = 0; i < local.Count; i++)
            {
                if (flushValues[i] != local[i].IntegerValue)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// A straight flush occurs where the difference between sorted
        /// card values is always 1, and all cards are of the same suit.
        /// </summary>
        public bool IsStraightFlush(List<Card> hand)
        {
            //TODO implement logic for ACE being counted as '1' as well.
            CheckArgSize(hand);
            if (!IsFlush(hand))
                return false;
            List<Card> sorted = GetSortedCopy(hand);
            //adjacent elements must be exactly 1 value from each other
            for (int i = 0; i < hand.Count-1; i++)
            {
                int diff = sorted[i+1].IntegerValue - sorted[i].IntegerValue;
                if (diff != 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// A flush occurs where all cards in the hand are of the same suit.
        /// </summary>
        public bool IsFlush(List<Card> hand)
        {
            CheckArgSize(hand);
            string currentSuit = hand[0].Suit;
            foreach (Card c in hand)
            {
                if (c.Suit != currentSuit)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns list of tuple int,int denoting the card value and it's frequency.
        /// </summary>
        public List<Tuple<int,int>> CountLikeValues(List<Card> hand)
        {
            List<Tuple<int, int>> countInfo = new();
            //if there is for instance, 4 of a kind, return that information.
            for (int i = 0; i < hand.Count; i++)
            {
                Card c = hand[i];
                for (int j = 0; j < countInfo.Count; j++)
                {
                    if (countInfo[j].Item1 == c.IntegerValue)
                    {
                        countInfo[j] = new Tuple<int, int>(c.IntegerValue, countInfo[j].Item2 + 1);
                    }
                }
                countInfo.Add(new Tuple<int, int>(c.IntegerValue, 1));
            }

            return countInfo;
        }

        /// <summary>
        /// Returns list of tuple Card,int denoting the card value and it's frequency.
        /// </summary>
        public List<List<Card>> GetCardPairs(List<Card> hand)
        {
            List<List<Card>> cardPairs = new();
            //if there is for instance, 4 of a kind, return that information.
            for (int i = 0; i < hand.Count; i++)
            {
                bool addedExisting = false;
                Card c = hand[i];
                for (int j = 0; j < cardPairs.Count; j++)
                {
                    if (cardPairs[j].Count > 0)
                    {
                        //test first element's integervalue against current card.
                        if (cardPairs[j][0].IntegerValue == c.IntegerValue)
                        {
                            cardPairs[j].Add(c);
                            addedExisting = true;
                        }
                    }
                }
                if (!addedExisting)
                    cardPairs.Add(new List<Card>() { c });
            }
            return cardPairs;
        }

        /// <summary>
        /// Can return a unique Card value Tuple, if no pairs exist in the hand.
        /// </summary>
        public Tuple<int, int> GetMaxLikeValue(List<Card> hand)
        {
            CheckArgSize(hand);
            var l = CountLikeValues(hand);
            int max = l.Max(x => x.Item2);
            foreach (var tuple in l)
            {
                if (tuple.Item1 == max)
                    return tuple;
            }

            return l[0];
        }
        private List<Card> GetSortedCopy(List<Card> hand)
        {
            List<Card> local = new List<Card>(hand);
            local.Sort();
            return local;
        }

        private List<int> BuildRoyalFlushValues()
        {
            List<int> values = new List<int>()
                {10, FaceValues.JackValue, FaceValues.QueenValue, FaceValues.KingValue, FaceValues.AceValue};
            //{FaceValues.AceValue, FaceValues.KingValue, FaceValues.QueenValue, FaceValues.JackValue, 10};
            return values;
        }

    }
}