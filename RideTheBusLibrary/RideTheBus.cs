/* Programmers: Colin Manliclic, Zina Long
 * Date:        April 9, 2021
 * Purpose:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RideTheBusLibrary
{
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateClient(CallbackInfo info);
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IRideTheBus
    {
        [OperationContract]
        Card Draw();
        int NumCards { [OperationContract] get; }
        int Winstreak { [OperationContract] get; }
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();
        [OperationContract]
        void PlayBlackRed(Card current, string color);
        [OperationContract]
        void PlayHighLow(Card current, Card last, string choice);
        [OperationContract]
        void PlayInOut(Card next, Card current, Card last, string choice);
        [OperationContract]
        void PlayFaceNotFace(Card current, string choice);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RideTheBus : IRideTheBus
    {
        /*------------------------ Member Variables ------------------------*/
        // Cards
        private List<Card> cards = null;    
        private int cardIdx;
        private Card CurrentCard;
        private Card LastCard;

        // Players
        private Dictionary<int, ICallback> callbacks = null;
        private int nextClientId;                               
        private int clientIndex;
        private int winstreak;
        private bool gameOver = false;

        /*-------------------------- Constructors --------------------------*/

        public RideTheBus()
        {
            cards = new List<Card>();
            nextClientId = 1;
            clientIndex = 0;
            callbacks = new Dictionary<int, ICallback>();
            Populate();
        }

        /*------------------ Public properties and methods -----------------*/
        // Returns a copy of the next Card in the cards collection
        public Card Draw()
        {
            if (cardIdx + 1 >= cards.Count)
            {
                gameOver = true;
            }
            
            if(CurrentCard != null)
            {
                LastCard = CurrentCard;
            }

            Card card = cards[cardIdx++];
            CurrentCard = card;

            NextPlayer();
            updateAllClients();

            return card;
        }

        // Lets the client read the number of cards remaining in the shoe
        public int NumCards
        {
            get
            {
                return cards.Count - cardIdx;
            }
        }

        public int Winstreak
        {
            get
            {
                return winstreak;
            }
        }

        // ServiceContract method that lets the client "register" for callbacks from the 
        // service and asigns to the client a unique client Id
        public int JoinGame()
        {
            // Identify which client is calling this method
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (callbacks.ContainsValue(cb))
            {
                // This client is already registered, so just return the id that was 
                // assigned previously
                int i = callbacks.Values.ToList().IndexOf(cb);
                return callbacks.Keys.ElementAt(i);
            }

            // Register this client and return a new client id
            callbacks.Add(nextClientId, cb);

            // Only client connected so far so release this client to do the first "count"
            // (necessary because a callback will only happen when a "count" is performed 
            // and a callback is the mechanism used to release a client)
            updateAllClients();
       
            return nextClientId++;
        }

        // ServiceContract method that lets the client "unregister" from the callbacks 
        // before disconnecting from the service
        public void LeaveGame()
        {
            // Identify which client is calling this method
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            if (callbacks.ContainsValue(cb))
            {
                // Identify which client is currently calling this method
                // - Get the index of the client within the callbacks collection
                int i = callbacks.Values.ToList().IndexOf(cb);
                // - Get the unique id of the client as stored in the collection
                int id = callbacks.ElementAt(i).Key;

                // Remove this client from receiving callbacks from the service
                callbacks.Remove(id);

                // Make sure the counting sequence isn't disrupted by removing this client
                if (i == clientIndex && callbacks.Count != 0)
                {
                    // This client was supposed to count next but is exiting the game
                    // Need to signal the next client to count instead 
                    NextPlayer();
                    updateAllClients();
                } 
                else if (clientIndex > i)
                {
                    // This prevents a player from being "skipped over" in the turn-taking
                    // of this "game"
                    clientIndex--;
                }
            }
        }

        public void PlayBlackRed(Card current, string color)
        {
            switch(color)
            {
                case "black":
                    if (current.Suit == SuitID.Clubs || current.Suit == SuitID.Spades)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
                case "red":
                    if (current.Suit == SuitID.Diamonds || current.Suit == SuitID.Hearts)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
            }

            updateAllClients();
        }

        public void PlayHighLow(Card current, Card last, string choice)
        {
            switch(choice)
            {
                case "high":
                    // card rank is ace high where ace = 0
                    if (current.Rank <= last.Rank)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
                case "low":
                    // card rank is ace high where ace = 0
                    if (current.Rank >= last.Rank)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
            }

            updateAllClients();
        }

        public void PlayInOut(Card next, Card current, Card last, string choice)
        {
            Card higher;
            Card lower;

            if (current.Rank >= last.Rank)
            {
                higher = current;
                lower = last;
            }
            else
            {
                higher = last;
                lower = current;
            }

            switch(choice)
            {
                case "in":
                    if(next.Rank >= lower.Rank && next.Rank <= higher.Rank)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
                case "out":
                    if (next.Rank < lower.Rank || next.Rank > higher.Rank)
                    {
                        winstreak++;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
            }

            updateAllClients();
        }

        public void PlayFaceNotFace(Card current, string choice)
        {
            switch (choice)
            {
                case "face":
                    if (current.Rank == RankID.Jack || current.Rank == RankID.Queen|| current.Rank == RankID.King)
                    {
                        winstreak++;
                        gameOver = true;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
                case "notface":
                    if (!(current.Rank == RankID.Jack || current.Rank == RankID.Queen || current.Rank == RankID.King))
                    {
                        winstreak++;
                        gameOver = true;
                    }
                    else
                    {
                        winstreak = 0;
                    }
                    break;
            }

            updateAllClients();
        }

        /*------------------------- Helper methods -------------------------*/
        private void Populate()
        {
            // For each deck in numDecks...
            for (int d = 0; d < 1; ++d)
                // For each suit..
                foreach (SuitID s in Enum.GetValues(typeof(SuitID)))
                    // For each rank..
                    foreach (RankID r in Enum.GetValues(typeof(RankID)))
                        cards.Add(new Card(s, r));

            // Randomize the cards collection
            Random rng = new Random();
            cards = cards.OrderBy(card => rng.Next()).ToList();

            // Reset the cards index
            cardIdx = 0;
        }

        public void NextPlayer()
        {
            clientIndex = ++clientIndex % callbacks.Count;
        }

        // Uses the client callback objects to send current Shoe information 
        // to clients. If the change in teh Shoe state was triggered by a method call 
        // from a specific client, then that particular client will be excluded from
        // the update since it will already be updated directly by the call.
        private void updateAllClients()
        {
            CallbackInfo info = new CallbackInfo(cards.Count - cardIdx, CurrentCard, LastCard, callbacks.Keys.ElementAt(clientIndex), winstreak, gameOver);

            foreach (ICallback cb in callbacks.Values)
                cb.UpdateClient(info);
        }

    }
}
