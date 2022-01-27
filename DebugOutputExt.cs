using System;
using System.Collections.Generic;

namespace csBasicDeck
{
    public static class DebugOutputExt
    {
        public static void PrintToConsole(this List<Card> hand)
        {
            foreach (var card in hand)
            {
                Console.Write("[{0}] ", card.ToString());
            }
        }
    }
}
