﻿/* Programmers: Colin Manliclic, Zina Long
 * Date:        April 9, 2021
 * Purpose:
 */
using System;
using System.Windows;
using System.ServiceModel;
using System.Threading;

using RideTheBusLibrary;


namespace RideTheBusGUIClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant, UseSynchronizationContext=false)]
    public partial class MainWindow : Window, ICallback
    {
        private IRideTheBus bus = null;
        Card currentCard;
        Card lastCard;
        Card discardedCard;
        private static int clientId = 0;
        private static int activeClientId = 1;
        //private static CBObject cbObj = new CBObject();
        private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static bool gameOver = false;

        public MainWindow()
        {
            InitializeComponent();

            DuplexChannelFactory<IRideTheBus> channel = new DuplexChannelFactory<IRideTheBus>(this, "BusEndPoint");
            bus = channel.CreateChannel();

            clientId = bus.JoinGame();

            Label_PlayerNum.Content = $"Player {clientId}";
            Label_CardsLeft.Content = bus.NumCards.ToString();
            Label_WinStreakScore.Content = bus.Winstreak.ToString();
        }
        private void Button_RuleBook_Click(object sender, RoutedEventArgs e)
        {
            new RuleBook().ShowDialog();
        }

        private void Button_Black_Click(object sender, RoutedEventArgs e)
        {
            if (lastCard!= null)
            {
                discardedCard = lastCard;
            }
            if (currentCard != null)
            {
                lastCard = currentCard;
                Label_LastRank.Content = lastCard.Rank.ToString();
                Label_LastSuit.Content = lastCard.Suit.ToString();
            }
            currentCard = bus.Draw();
            bus.PlayBlackRed(currentCard, "black");
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
        }

        private void Button_Red_Click(object sender, RoutedEventArgs e)
        {
            if (lastCard != null)
            {
                discardedCard = lastCard;
            }
            if (currentCard != null)
            {
                lastCard = currentCard;
                Label_LastRank.Content = lastCard.Rank.ToString();
                Label_LastSuit.Content = lastCard.Suit.ToString();
            }
            currentCard = bus.Draw();
            bus.PlayBlackRed(currentCard, "red");
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
        }

        private void Button_High_Click(object sender, RoutedEventArgs e)
        {
            discardedCard = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayHighLow(currentCard,lastCard,"high");
        }

        private void Button_Low_Click(object sender, RoutedEventArgs e)
        {
            discardedCard = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayHighLow(currentCard, lastCard, "low");
        }

        private void Button_In_Click(object sender, RoutedEventArgs e)
        {
            discardedCard = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayInOut(currentCard, lastCard, discardedCard, "in");
        }

        private void Button_Out_Click(object sender, RoutedEventArgs e)
        {
            discardedCard = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayInOut(currentCard, lastCard, discardedCard, "out");
        }

        private void Button_Face_Click(object sender, RoutedEventArgs e)
        {
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayFaceNotFace(currentCard, "face");
        }

        private void Button_NotFace_Click(object sender, RoutedEventArgs e)
        {
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayFaceNotFace(currentCard, "notface");
        }

        private delegate void ClientUpdateDelegate(CallbackInfo info);

        public void UpdateClient(CallbackInfo info)
        {
            if(System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                // Update gui
                Label_CardsLeft.Content = info.NumCards.ToString();

                if (info.CurrentCard != null)
                {
                    currentCard = info.CurrentCard;
                    Label_CurrentRank.Content = currentCard.Rank;
                    Label_CurrentSuit.Content = currentCard.Suit;
                }
         
                if (info.LastCard != null)
                {
                    lastCard = info.LastCard;
                    Label_LastRank.Content = lastCard.Rank;
                    Label_LastSuit.Content = lastCard.Suit;
                }

                Label_WinStreakScore.Content = info.WinStreak.ToString();
                gameOver = info.GameOver;

                // Active player 
                activeClientId = info.NextClient;

                // Player 1 always starts
                if (activeClientId == clientId)
                {
                    Label_Status.Content = $"Your turn";
            
                }
                else
                {
                    Label_Status.Content = $"Player #{activeClientId}'s turn";
                }

                string choice;
                switch(info.WinStreak)
                {
                    case 0:
                        DisableAllButtons();
                        if(activeClientId == clientId)
                        {
                            Button_Black.IsEnabled = true;
                            Button_Red.IsEnabled = true;
                        }
                        Label_QuestionText.Content = "Will the next card be Black or Red?";
                        if (currentCard != null)
                            Label_Update.Content = "You guessed incorrect! Restarting to Black or Red game.";
                        break;
                    case 1:
                        DisableAllButtons();
                        choice = (currentCard.Suit == SuitID.Hearts || currentCard.Suit == SuitID.Diamonds) ? "red" : "black";
                        Label_Update.Content = $"You guessed correct! {currentCard.Rank} of {currentCard.Suit} is {choice}";
                        if (activeClientId == clientId)
                        {
                            Button_High.IsEnabled = true;
                            Button_Low.IsEnabled = true;
                        }
                        Label_QuestionText.Content = $"Will the next card be higher or lower than the {currentCard.Rank}?";
                        break;
                    case 2:
                        DisableAllButtons();
                        choice = (lastCard.Rank < currentCard.Rank) ? "higher" : "lower";
                        Label_Update.Content = $"You guessed correct! {lastCard.Rank} was {choice} or equal to {currentCard.Rank}";
                        if (activeClientId == clientId)
                        {
                            Button_In.IsEnabled = true;
                            Button_Out.IsEnabled = true;
                        }
                        Label_QuestionText.Content = $"Will the next card be inside(inclusive) or outside the {lastCard.Rank} and {currentCard.Rank}?";
                        break;
                    case 3:
                        DisableAllButtons();
                        Card higher;
                        Card lower;
                        if (lastCard.Rank >= discardedCard.Rank)
                        {
                            higher = discardedCard;
                            lower = lastCard;
                        }
                        else
                        {
                            higher = lastCard;
                            lower = discardedCard;
                        }
                        choice = (currentCard.Rank <= lower.Rank && currentCard.Rank >= higher.Rank) ? "inside" : "outside";
                        Label_Update.Content = $"You guessed correct! {currentCard.Rank} was {choice} {lower.Rank} and {higher.Rank}";
                        if (activeClientId == clientId)
                        {
                            Button_Face.IsEnabled = true;
                            Button_NotFace.IsEnabled = true;
                        }
                        Label_QuestionText.Content = "Will the next card be a face(Jack, Queen, King) card or not?";
                        break;
                    case 4:
                        DisableAllButtons();
                        choice = (currentCard.Rank == RankID.Jack || currentCard.Rank == RankID.Queen || currentCard.Rank == RankID.King) ? "" : " not";
                        Label_Update.Content = $"You guessed correct! {currentCard.Rank} was{choice} a face card";
                        break;

                }

                if (gameOver && int.Parse(Label_WinStreakScore.Content.ToString()) == 4)
                {
                    MessageBox.Show("You Won!", "Game Over");
                }

                if (gameOver && info.NumCards == 0)
                {
                    DisableAllButtons();
                    MessageBox.Show("You Lose!", "Game Over");
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateClient), info);
            }
        }

        private void DisableAllButtons()
        {
            Button_Black.IsEnabled = false;
            Button_Red.IsEnabled = false;
            Button_High.IsEnabled = false;
            Button_Low.IsEnabled = false;
            Button_In.IsEnabled = false;
            Button_Out.IsEnabled = false;
            Button_Face.IsEnabled = false;
            Button_NotFace.IsEnabled = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bus?.LeaveGame();
        }
    }
}
