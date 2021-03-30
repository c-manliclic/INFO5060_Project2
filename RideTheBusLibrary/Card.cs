using System;
using System.Runtime.Serialization; 

namespace CardsLibrary
{
    public enum SuitID { Clubs, Diamonds, Hearts, Spades };
    public enum RankID { Ace, King, Queen, Jack, Ten, Nine, Eight, Seven, Six, Five, Four, Three, Two };

    [DataContract]
    public class Card
    {
        internal Card(SuitID s, RankID r)
        {
            Suit = s;
            Rank = r;
        }

        [DataMember]
        public SuitID Suit { get; private set; }
        [DataMember]
        public RankID Rank { get; private set; }

        public override string ToString()
        {
            return Rank.ToString() + " of " + Suit.ToString();
        }

    } 
}
