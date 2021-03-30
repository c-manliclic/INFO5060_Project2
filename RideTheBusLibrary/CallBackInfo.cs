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
        public Card LastCard { get; private set; }
        [DataMember]
        public Card CurrentCard { get; private set; }

        public CallbackInfo(int n, Card l, Card c)
        {
            NumCards = n;
            LastCard = l;
            CurrentCard = c;
        }
    }
}
