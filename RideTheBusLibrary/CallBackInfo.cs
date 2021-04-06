/* Programmers: Colin Manliclic, Zina Long
 * Date:        April 9, 2021
 * Purpose:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RideTheBusLibrary
{
    [DataContract]
    public class CallbackInfo
    {
        [DataMember]
        public int NumCards { get; private set; }
        [DataMember]
        public Card CurrentCard { get; private set; }

        [DataMember]
        public Card LastCard { get; private set; }

        [DataMember]
        public Card DiscardedCard { get; private set; }

        [DataMember]
        public int NextClient { get; private set; }

        [DataMember]
        public int WinStreak { get; private set; }

        [DataMember]
        public bool GameOver { get; private set; }

        public CallbackInfo(int numCards, Card currentCard, Card lastCard, Card discardedCard, int nextClient, int winStreak, bool gameOver)
        {
            NumCards = numCards;
            CurrentCard = currentCard;
            LastCard = lastCard;
            DiscardedCard = discardedCard;
            NextClient = nextClient;
            WinStreak = winStreak;
            GameOver = gameOver;
        }
    }
}
