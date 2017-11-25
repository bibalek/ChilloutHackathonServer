using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSocketClient
{
    
    public class Player
    {
        public Player()
        {
            Hand = new List<LogicCard>();
        }

        public bool IsSB { get; set; }
        public bool IsBB { get; set; }
        public bool IsDealer { get; set; }
        public int AccountBalance { get; set; }
        public List<LogicCard> Hand { get; set; }
        //public List<Card> Hand { get; set; }

        public string GetAllSevenCards(List<LogicCard> tableCards)
        {
            string tableCardsSymbols = "";
            for(int cardIndex = 0; cardIndex < 5; cardIndex++)
            {
                tableCardsSymbols += tableCards[cardIndex].ConvertCardToString() + ";";
            }
            return this.Hand[0].ConvertCardToString() + ";" + this.Hand[1].ConvertCardToString() + ";" + tableCardsSymbols;
        }

    }





    public class Deal
    {
        
    }
}
