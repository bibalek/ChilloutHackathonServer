using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSocketClient
{
    public enum Figures { Two , Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }
    public enum Colors { C, S, D, H }

    public class LogicCard
    {
        public LogicCard() { }

        public LogicCard(Figures figure, Colors color)
        {
            this.figure = figure;
            this.color = color;
        }

        public Colors color { get; set; }
        public Figures figure { get; set; }

        public string ConvertCardToString()
        {
            string card = "";
            string figureToAdd = "";
            string colorToAdd = "";
            switch(this.figure)
            {
                case Figures.Two:
                    figureToAdd = "2";
                    break;
                case Figures.Three:
                    figureToAdd = "3";
                    break;
                case Figures.Four:
                    figureToAdd = "4";
                    break;
                case Figures.Five:
                    figureToAdd = "5";
                    break;
                case Figures.Six:
                    figureToAdd = "6";
                    break;
                case Figures.Seven:
                    figureToAdd = "7";
                    break;
                case Figures.Eight:
                    figureToAdd = "8";
                    break;
                case Figures.Nine:
                    figureToAdd = "9";
                    break;
                case Figures.Ten:
                    figureToAdd = "T";
                    break;
                case Figures.Jack:
                    figureToAdd = "J";
                    break;
                case Figures.Queen:
                    figureToAdd = "Q";
                    break;
                case Figures.King:
                    figureToAdd = "K";
                    break;
                case Figures.Ace:
                    figureToAdd = "A";
                    break;
                default:
                    figureToAdd = "X";
                    break;
            }

            switch(this.color)
            {
                case Colors.C:
                    colorToAdd = "C";
                    break;
                case Colors.S:
                    colorToAdd = "S";
                    break;
                case Colors.D:
                    colorToAdd = "D";
                    break;
                case Colors.H:
                    colorToAdd = "H";
                    break;
                default:
                    colorToAdd = "X";
                    break;
            }

            return figureToAdd + colorToAdd;
        }

        public void ConvertStringToCard(string card)
        {
            switch(card[0])
            {
                case '2':
                    this.figure = Figures.Two;
                    break;
                case '3':
                    this.figure = Figures.Three;
                    break;
                case '4':
                    this.figure = Figures.Four;
                    break;
                case '5':
                    this.figure = Figures.Five;
                    break;
                case '6':
                    this.figure = Figures.Six;
                    break;
                case '7':
                    this.figure = Figures.Seven;
                    break;
                case '8':
                    this.figure = Figures.Eight;
                    break;
                case '9':
                    this.figure = Figures.Nine;
                    break;
                case 'T':
                    this.figure = Figures.Ten;
                    break;
                case 'J':
                    this.figure = Figures.Jack;
                    break;
                case 'Q':
                    this.figure = Figures.Queen;
                    break;
                case 'K':
                    this.figure = Figures.King;
                    break;
                case 'A':
                    this.figure = Figures.Ace;
                    break;
                default:
                    this.figure = Figures.Two;
                    break;
            }

            switch(card[1])
            {
                case 'S':
                    this.color = Colors.S;
                    break;
                case 'D':
                    this.color = Colors.D;
                    break;
                case 'H':
                    this.color = Colors.H;
                    break;
                case 'C':
                    this.color = Colors.C;
                    break;
            }
        }
    }

}
