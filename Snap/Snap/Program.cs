using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSSocketClient;
using Newtonsoft.Json;
using SnapCall;

namespace Snap
{
    class Program
    {
        enum State
        {
            Preflop,
            Preturn,
            PreRiver,
            PostRiver,
        }
        private static State currentState;

        private static Game game;
        private static readonly TcpListener tcpListener = new TcpListener(10);
        private static readonly List<StreamWriter> writers = new List<StreamWriter>();
        private static int numberOfPlayers = 0;
        private static int currentWriter = 0;


        private static void SwitchWriter()
        {
            currentWriter = (currentWriter + 1) % 2;
        }

        private static void Process(string data)
        {
            PTPHeader ptp = JsonConvert.DeserializeObject<PTPHeader>(data);
            switch (currentState)
            {
                case State.Preflop:
                    Preflop(ptp);
                    break;
                case State.Preturn:
                    Preturn(ptp);
                    break;
                case State.PreRiver:
                    Preriver(ptp);
                    break;
                case State.PostRiver:
                    PostRiver(ptp);
                    break;
            }

        }

        private static void PostRiver(PTPHeader ptp)
        {
            IncrementFlags(ptp);

            if (preflops_calls == 1 || preflops_checks == 2 || preflops_folds == 1)
            {
                Hand playerOneHand = new Hand();
                Hand playerTwoHand = new Hand();

                Card c00 = new Card(game.players[0].Hand[0].ConvertCardToString());
                Card c01 = new Card(game.players[0].Hand[1].ConvertCardToString());
                Card c10 = new Card(game.players[1].Hand[0].ConvertCardToString());
                Card c11 = new Card(game.players[1].Hand[1].ConvertCardToString());

                Card s1 = new Card(game.tableCards[0].ConvertCardToString());
                Card s2 = new Card(game.tableCards[1].ConvertCardToString());
                Card s3 = new Card(game.tableCards[2].ConvertCardToString());
                Card s4 = new Card(game.tableCards[3].ConvertCardToString());
                Card s5 = new Card(game.tableCards[4].ConvertCardToString());

                Card[] c0 = { c00, c01, s1, s2, s3, s4, s5 };
                playerOneHand.Cards = new List<Card>(c0);
                Card[] c1 = { c10, c11, s1, s2, s3, s4, s5 };
                playerTwoHand.Cards = new List<Card>(c1);

                playerOneHand.GetBestHandFromSeven();
                playerTwoHand.GetBestHandFromSeven();

                bool hasDealerWon = playerOneHand.GetStrength().CompareTo(playerTwoHand.GetStrength()) == 1;

                PTPHeader response = new PTPHeader(0, hasDealerWon, false, false, false, );
                string json = JsonConvert.SerializeObject(response);
                BroadcastMessage(json);
            }
            else
            {
                SwitchWriter();
                PTPHeader response = new PTPHeader(0, false, true, true, true, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);
            }
        }

        private static void Preriver(PTPHeader ptp)
        {
            IncrementFlags(ptp);

            if (preflops_calls == 1 || preflops_checks == 2 || preflops_folds == 1)
            {
                PTPHeader response = new PTPHeader(-1, false, false, false, false, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                SwitchWriter();
                response = new PTPHeader(-1, true, true, false, true, "");
                json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                preflops_calls = preflops_folds = preflops_checks = 0;
                currentState = State.PostRiver;
            }
            else
            {
                SwitchWriter();
                PTPHeader response = new PTPHeader(0, false, true, true, true, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);
            }
        }

        private static void Preturn(PTPHeader ptp)
        {
            IncrementFlags(ptp);

            if (preflops_calls == 1 || preflops_checks == 2 || preflops_folds == 1)
            {
                PTPHeader response = new PTPHeader(-1, false, false, false, false, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                SwitchWriter();
                response = new PTPHeader(-1, true, true, false, true, "");
                json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                preflops_calls = preflops_folds = preflops_checks = 0;
                currentState = State.PreRiver;
            }
            else
            {
                SwitchWriter();
                PTPHeader response = new PTPHeader(0, false, true, true, true, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);
            }
        }

        private static int preflops_checks = 0;
        private static int preflops_folds = 0;
        private static int preflops_calls = 0;
        private static void Preflop(PTPHeader ptp)
        {
            IncrementFlags(ptp);

            if (preflops_calls == 2 || preflops_checks == 1 || preflops_folds == 1)
            {
                PTPHeader response = new PTPHeader(-3, false, false, false, false, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                SwitchWriter();
                response = new PTPHeader(-3, true, true, false, true, "");
                json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);

                preflops_calls = preflops_folds = preflops_checks = 0;
                currentState = State.Preturn;
            }
            else
            {
                SwitchWriter();
                PTPHeader response = new PTPHeader(0, false, true, true, true, "");
                string json = JsonConvert.SerializeObject(response);
                WriteToCurrentWriter(json);
            }
        }

        private static void IncrementFlags(PTPHeader ptp)
        {
            if (ptp.call) preflops_calls++;
            if (ptp.check) preflops_checks++;
            if (ptp.fold) preflops_folds++;
        }

        private static void WriteToCurrentWriter(string json)
        {
            writers[currentWriter].WriteLine(json);
            writers[currentWriter].Flush();
        }

        private static void BroadcastMessage(string msg)
        {
            foreach (var writer in writers)
            {
                writer.WriteLine(msg);
                writer.Flush();
            }
        }

        private static void ListenerThread()
        {   
            Socket socketForClient = tcpListener.AcceptSocket();
            if (socketForClient.Connected)
            {
                numberOfPlayers++;
                Console.WriteLine("Client:" + socketForClient.RemoteEndPoint + " now connected to server.");
                NetworkStream networkStream = new NetworkStream(socketForClient);

                //add writer for this client to clients pool
                writers.Add(new StreamWriter(networkStream));

                StreamReader streamReader = new StreamReader(networkStream);
                while (true)
                {
                    try
                    {
                        string theString = streamReader.ReadLine();
                        Console.WriteLine("Message received by client:" + theString);
                        //BroadcastMessage(theString);
                        Process(theString);
                    }
                    catch
                    {
                        Console.Write("connection broken");
                        return;
                    }
                }
            }
            socketForClient.Close();
            Console.WriteLine("Press any key to exit from server program");
            Console.ReadKey();
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void Main()
        {
            tcpListener.Start();

            Console.WriteLine("This is Server program,");
            Console.WriteLine("IP: " + GetLocalIPAddress());
            for (int i = 0; i < 2; i++)
            {
                Thread newThread = new Thread(ListenerThread);
                newThread.Start();
            }
            while (numberOfPlayers != 2)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Both clients are nao intertwined. Commencing the Game.");
            game = new Game();
            game.cards = game.croupier.GenerateDeck();
            game.croupier.ShowCards(game.cards);
            game.players[0].Hand = game.croupier.DealHand(game.cards);
            game.players[1].Hand = game.croupier.DealHand(game.cards);
            game.tableCards = game.croupier.GetTableCards(game.cards);

            const int MONEY = 1000;

            PTPHandshake playerOneHandshake = new PTPHandshake(true, true, MONEY, game.players[0].GetAllSevenCards(game.tableCards));
            PTPHandshake playerTwoHandshake = new PTPHandshake(false, false, MONEY, game.players[1].GetAllSevenCards(game.tableCards));

            string serializedHandshake = JsonConvert.SerializeObject(playerOneHandshake);
            writers[0].WriteLine(serializedHandshake);
            writers[0].Flush();
            serializedHandshake = JsonConvert.SerializeObject(playerTwoHandshake);
            writers[1].WriteLine(serializedHandshake);
            writers[1].Flush();
        }
    }
}
