using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csConsoleApp
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
