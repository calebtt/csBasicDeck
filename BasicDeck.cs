using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace csBasicDeck
{
    public class BasicDeck : IBasicDeckOperations
    {
        private const int ShuffleRepeats = 20;
        private List<Card> flatCardArray;
        private readonly Random rn = new(); // new C# language feature (new() without type)
        /// <summary> Property for accessing the card array.  </summary>
        public List<Card> Cards
        {
            get { return flatCardArray; }
        }

        /// <summary> Constructor, builds a new deck of cards and shuffles it. </summary>
        public BasicDeck()
        {
            BuildNewDeck();
        }
        /// <summary> Constructor, creates a deck from an existing list of cards. </summary>
        /// <param name="hand">list of cards</param>
        public BasicDeck(List<Card> hand)
        {
            flatCardArray = hand;
        }
        public void BuildNewDeck()
        {
            flatCardArray = new List<Card>();
            Dictionary<int, string> fvList = BuildFaceValueMap();
            foreach (var elem in fvList)
            {
                Card c = new()
                {
                    CardColor = Color.Black, IntegerValue = elem.Key, StringValue = elem.Value, Suit = SuitValues.ClubsName
                };
                Card d = new()
                {
                    CardColor = Color.Red, IntegerValue = elem.Key, StringValue = elem.Value, Suit = SuitValues.DiamondsName
                };
                Card h = new()
                {
                    CardColor = Color.Red, IntegerValue = elem.Key, StringValue = elem.Value, Suit = SuitValues.HeartsName
                };
                Card s = new()
                {
                    CardColor = Color.Black, IntegerValue = elem.Key, StringValue = elem.Value, Suit = SuitValues.SpadesName
                };
                flatCardArray.Add(c);
                flatCardArray.Add(d);
                flatCardArray.Add(h);
                flatCardArray.Add(s);
            }

            Shuffle();
        }

        /// <summary>
        /// Builds and returns a Dictionary that maps a card's Integer value to a string representation.
        /// </summary>
        public Dictionary<int, string> BuildFaceValueMap()
        {
            Dictionary<int, string> suitMap = new();
            for (int i = FaceValues.ValueCardMin; i < FaceValues.ValueCardMax+1; i++)
            {
                suitMap.Add(i, i.ToString());
            }
            suitMap.Add(FaceValues.JackValue, FaceValues.JackName);
            suitMap.Add(FaceValues.QueenValue, FaceValues.QueenName);
            suitMap.Add(FaceValues.KingValue, FaceValues.KingName);
            suitMap.Add(FaceValues.AceValue, FaceValues.AceName);
            return suitMap;
        }
        public void Shuffle()
        {
            const int minShuffle = 0;
            Card tempCard;
            for (int j = 0; j < ShuffleRepeats; j++)
            {
                for (int i = 0; i < flatCardArray.Count; i++)
                {
                    int shuffle = rn.Next(minShuffle, flatCardArray.Count);
                    tempCard = flatCardArray.ElementAt(shuffle);
                    flatCardArray.RemoveAt(shuffle);
                    flatCardArray.Add(tempCard);
                }
            }
        }
        public void Shuffle(List<Card> hand)
        {
            const int minShuffle = 0;
            for (int j = 0; j < ShuffleRepeats; j++)
            {
                for (int i = 0; i < hand.Count; i++)
                {
                    int shuffle = rn.Next(minShuffle, hand.Count);
                    Card c = new (hand.ElementAt(shuffle));
                    hand.RemoveAt(shuffle);
                    hand.Add(c);
                }
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (Card c in flatCardArray)
            {
                sb.Append(c.ToString() + " ");
            }
            //sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public Card RemoveFromTop()
        {
            Card card = new (flatCardArray.First());
            flatCardArray.RemoveAt(0);
            return card;
        }

        public Card RemoveFromBottom()
        {
            throw new NotImplementedException();
        }

        public void AddToTop(Card c)
        {
            flatCardArray = flatCardArray.Prepend(c) as List<Card>;
        }

        public void AddToBottom(Card c)
        {
            flatCardArray.Add(c);
        }

        public int Size()
        {
            return flatCardArray.Count;
        }

        /// <summary>
        /// Returns a sub-list of the internal List of Card,
        /// starting at the "beginning" or "top" of the deck.
        /// </summary>
        /// <param name="size">num elements in hand</param>
        /// <returns>Empty list on invalid size.</returns>
        public List<Card> CreateHandFromTop(int size)
        {
            List<Card> temp = new();
            if (size <= flatCardArray.Count && size >0)
            {
                temp = flatCardArray.GetRange(0, size);
                flatCardArray.RemoveRange(0,size);
            }
            return temp;
        }

        /// <summary>
        /// Returns a sub-list of the internal List of Card,
        /// starting at the "end" or "bottom" of the deck.
        /// </summary>
        /// <param name="size">num elements in hand</param>
        /// <returns>Empty list on invalid size.</returns>
        public List<Card> CreateHandFromBottom(int size)
        {
            List<Card> temp = new();
            if (size <= flatCardArray.Count && size > 0)
            {
                int firstIndex = flatCardArray.Count - size;
                temp = flatCardArray.GetRange(firstIndex, size);
                flatCardArray.RemoveRange(firstIndex, size);
            }
            return temp;
        }

        public List<Card> GetCopyOfCards()
        {
            return new List<Card>(flatCardArray);
        }
    }
}
