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
        [OperationContract]
        int JoinGame();
        [OperationContract(IsOneWay = true)]
        void LeaveGame();

    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RideTheBus : IRideTheBus
    {
        /*------------------------ Member Variables ------------------------*/
        // Cards
        private List<Card> cards = null;    
        private int cardIdx;                
        private static uint objCount = 0;
        private uint objNum;

        // Players
        private Dictionary<int, ICallback> callbacks = null;
        private int nextClientId;                               
        private int clientIndex;
        private bool gameOver = false;

        /*-------------------------- Constructors --------------------------*/

        public RideTheBus()
        {
            objNum = ++objCount;
            Console.WriteLine($"Creating Game object #{objNum}");
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
            if (cardIdx >= cards.Count)
            {
                gameOver = true;
                return null;
            }
               
            Card card = cards[cardIdx++];

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
            //updateAllClients();
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
                if (i == clientIndex)
                    // This client was supposed to count next but is exiting the game
                    // Need to signal the next client to count instead 
                    updateAllClients();
                else if (clientIndex > i)
                    // This prevents a player from being "skipped over" in the turn-taking
                    // of this "game"
                    clientIndex--;
            }
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

            Console.WriteLine($"Shoe #{objNum} Shuffling");

            // Randomize the cards collection
            Random rng = new Random();
            cards = cards.OrderBy(card => rng.Next()).ToList();

            // Reset the cards index
            cardIdx = 0;

            updateAllClients();
        }

        public void NextPlayer()
        {
            clientIndex = ++clientIndex % callbacks.Count;
            updateAllClients();
        }

        // Uses the client callback objects to send current Shoe information 
        // to clients. If the change in teh Shoe state was triggered by a method call 
        // from a specific client, then that particular client will be excluded from
        // the update since it will already be updated directly by the call.
        private void updateAllClients()
        {
            // Prepare the CallbackInfo parameter
            if (callbacks.Count != 0)
            {
                CallbackInfo info = new CallbackInfo(cards.Count - cardIdx, callbacks.Keys.ElementAt(clientIndex), gameOver);

                foreach (ICallback cb in callbacks.Values)
                    cb.UpdateClient(info);
            }
   
        }

    }
}
