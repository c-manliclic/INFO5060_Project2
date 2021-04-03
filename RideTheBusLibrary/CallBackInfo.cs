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
        public int NextClient { get; private set; }

        [DataMember]
        public int WinStreak { get; private set; }

        [DataMember]
        public bool GameOver { get; private set; }

        public CallbackInfo(int numCards, int nextClient, int winStreak, bool gameOver)
        {
            NumCards = numCards;
            NextClient = nextClient;
            WinStreak = winStreak;
            GameOver = gameOver;
        }
    }
}
