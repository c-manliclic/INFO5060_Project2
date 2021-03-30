using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RideTheBusLibrary
{
    //[ServiceContract]
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateClient(CallbackInfo info);
    }

    // Converted IShoe to a WCF service contract
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IRideTheBus
    {
        [OperationContract]
        Card Draw();
        Card LastCard { [OperationContract] get; }
        Card CurrentCard { [OperationContract] get; }
        int NumCards { [OperationContract] get; }
        [OperationContract(IsOneWay = true)]
        void RegisterForCallbacks();
        [OperationContract(IsOneWay = true)]
        void UnregisterFromCallbacks();

    }

    // The class that implements the service
    // ServiceBehavior is used here to select the desired instancing behaviour
    // of the Shoe class
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RideTheBus : IRideTheBus
    {
        /*------------------------ Member Variables ------------------------*/

        private List<Card> cards = null;    // collection of cards
        private int cardIdx;                // index of the next card to be dealt
        private Card currentCard;
        private Card lastCard;

        private static uint objCount = 0;
        private uint objNum;
        private HashSet<ICallback> callbacks = new HashSet<ICallback>();

        /*-------------------------- Constructors --------------------------*/

        public RideTheBus()
        {
            objNum = ++objCount;
            Console.WriteLine($"Creating Shoe object #{objNum}");
            cards = new List<Card>();
            populate();
        }

        /*------------------ Public properties and methods -----------------*/
        // Returns a copy of the next Card in the cards collection
        public Card Draw()
        {
            if (cardIdx >= cards.Count)
                throw new ArgumentException("The shoe is empty.");

            Card card = cards[cardIdx++];

            Console.WriteLine($"Shoe #{objNum} dealing {cards[cardIdx].ToString()}");

            updateClients(false);

            return card;
        }

        // Lets the client read or modify the number of decks in the shoe
        public Card CurrentCard
        {
            get
            {
                return currentCard;
            }
        }

        // Lets the client read the number of cards remaining in the shoe
        public int NumCards
        {
            get
            {
                return cards.Count - cardIdx;
            }
        }

        public Card LastCard
        {
            get
            {
                return lastCard;
            }
        }

        public void RegisterForCallbacks()
        {
            // Identify which client is calling this method
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            // Add client's callback object to the collection
            if (!callbacks.Contains(cb))
                callbacks.Add(cb);
        }

        public void UnregisterFromCallbacks()
        {
            // Identify which client is calling this method
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();

            // Remove client's callback object from the collection
            if (callbacks.Contains(cb))
                callbacks.Remove(cb);
        }


        /*------------------------- Helper methods -------------------------*/
        private void populate()
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

            updateClients(true);
        }

        // Uses the client callback objects to send current Shoe information 
        // to clients. If the change in teh Shoe state was triggered by a method call 
        // from a specific client, then that particular client will be excluded from
        // the update since it will already be updated directly by the call.
        private void updateClients(bool emptyHand)
        {
            // Identify which client just changed the Shoe object's state
            ICallback thisClient = null;
            if (OperationContext.Current != null)
                thisClient = OperationContext.Current.GetCallbackChannel<ICallback>();

            // Prepare the CallbackInfo parameter
            CallbackInfo info = new CallbackInfo(cards.Count - cardIdx, lastCard, currentCard);

            // Update all clients except thisClient
            foreach (ICallback cb in callbacks)
                if (thisClient == null || thisClient != cb)
                    cb.UpdateClient(info);
        }

    }
}
