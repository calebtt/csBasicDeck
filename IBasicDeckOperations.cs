using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApp
{
    public interface IBasicDeckOperations
    {
        public void Shuffle();
        public Card RemoveFromTop();
        public Card RemoveFromBottom();
        public void AddToTop(Card c);
        public void AddToBottom(Card c);
        public void BuildNewDeck();
        public int Size();
        public List<Card> CreateHandFromTop(int size);
        public List<Card> CreateHandFromBottom(int size);
        public List<Card> GetCopyOfCards();

    }
}
