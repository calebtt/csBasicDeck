using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApp
{
    public class Card : IComparable
    {
        private const string ERR_COMPARE_NULL = "Exception in Card.CompareTo(): null argument.";
        public string Suit { get; set; } = new string("ERROR");
        public Color CardColor { get; set; } = Color.Empty;
        public string StringValue { get; set; } = new string("ERROR");
        public int IntegerValue { get; set; }
        public Card() { }
        public Card(Card rhs)
        {
            this.Suit = rhs.Suit;
            this.CardColor = rhs.CardColor;
            this.StringValue = rhs.StringValue;
            this.IntegerValue = rhs.IntegerValue;
        }
        public override string ToString()
        {
            return StringValue + " of " + Suit;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), ERR_COMPARE_NULL);
            Card lc = obj as Card;
            if (this.IntegerValue < lc.IntegerValue)
                return -1;
            if (this.IntegerValue == lc.IntegerValue)
                return 0;
            return 1;
        }

        public static bool operator<(Card lhs, Card rhs)
        {
            return lhs.IntegerValue < rhs.IntegerValue;
        }
        public static bool operator>(Card lhs, Card rhs)
        {
            return lhs.IntegerValue > rhs.IntegerValue;
        }
    }
}
