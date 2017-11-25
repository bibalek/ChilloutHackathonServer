using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSocketClient
{
    public class Croupier
    {
        public List<LogicCard> GenerateDeck()
        {
            List<LogicCard> cardsToReturn = new List<LogicCard>();
            foreach (Colors color in Enum.GetValues(typeof(Colors)))
            {
                foreach (Figures figure in Enum.GetValues(typeof(Figures)))
                {
                    LogicCard cartToAdd = new LogicCard(figure, color);
                    cardsToReturn.Add(cartToAdd);
                }
            }
            var rnd = new Random();
            var shuffledCards = cardsToReturn.OrderBy(card => rnd.Next()).ToList();
            return shuffledCards;
        }

        public void ShowCards(List<LogicCard> cards)
        {
            foreach (LogicCard card in cards)
            {
                Console.WriteLine(card.figure + " " + card.color);
            }
        }

        public List<LogicCard> DealHand(List<LogicCard> cards)
        {
            List<LogicCard> handToGive = new List<LogicCard>();

            LogicCard first = GetOneCardFromDeck(cards);
            LogicCard second = GetOneCardFromDeck(cards);

            handToGive.Add(first);
            handToGive.Add(second);
            return handToGive;
        }

        public LogicCard GetOneCardFromDeck(List<LogicCard> cards)
        {
            LogicCard card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }

        public List<LogicCard> GetTableCards(List<LogicCard> cards)
        {
            List<LogicCard> cardsToReturn = new List<LogicCard>();
            for(int cardIndex = 0; cardIndex < 5; cardIndex++)
            {
                cardsToReturn.Add(GetOneCardFromDeck(cards));
            }
            return cardsToReturn;
        }

        public List<LogicCard> DealFlop(List<LogicCard> cards)
        {
            List<LogicCard> flop = new List<LogicCard>();
            //without burned one
            LogicCard first = GetOneCardFromDeck(cards);
            LogicCard second = GetOneCardFromDeck(cards);
            LogicCard third = GetOneCardFromDeck(cards);

            flop.Add(first);
            flop.Add(second);
            flop.Add(third);
            return flop;
        }

        public List<LogicCard> DealTurn(List<LogicCard> cards)
        {
            List<LogicCard> turn = new List<LogicCard>();

            LogicCard first = GetOneCardFromDeck(cards);
            turn.Add(first);
            return turn;
        }

        public List<LogicCard> DealRiver(List<LogicCard> cards)
        {
            List<LogicCard> river = new List<LogicCard>();

            LogicCard first = GetOneCardFromDeck(cards);
            river.Add(first);
            return river;
        }
    }
}
