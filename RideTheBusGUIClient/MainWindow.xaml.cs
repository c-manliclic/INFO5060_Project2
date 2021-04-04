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
        private static int clientId, activeClientId = 0;
        //private static CBObject cbObj = new CBObject();
        private static EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private static bool gameOver = false;

        public MainWindow()
        {
            InitializeComponent();

            DuplexChannelFactory<IRideTheBus> channel = new DuplexChannelFactory<IRideTheBus>(this, "BusEndPoint");
            bus = channel.CreateChannel();

            bus.JoinGame();

            Label_CardsLeft.Content = bus.NumCards.ToString();
            Label_WinStreakScore.Content = bus.Winstreak.ToString();
        }
        private void Button_RuleBook_Click(object sender, RoutedEventArgs e)
        {
            new RuleBook().ShowDialog();
        }

        private void Button_Black_Click(object sender, RoutedEventArgs e)
        {
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
            Card last = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayInOut(currentCard, lastCard, last, "in");
        }

        private void Button_Out_Click(object sender, RoutedEventArgs e)
        {
            Card last = lastCard;
            lastCard = currentCard;
            Label_LastRank.Content = lastCard.Rank.ToString();
            Label_LastSuit.Content = lastCard.Suit.ToString();
            currentCard = bus.Draw();
            Label_CurrentRank.Content = currentCard.Rank.ToString();
            Label_CurrentSuit.Content = currentCard.Suit.ToString();
            bus.PlayInOut(currentCard, lastCard, last, "out");
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

                currentCard = info.CurrentCard;
                Label_CurrentRank.Content = currentCard.Rank;
                Label_CurrentSuit.Content = currentCard.Suit;

                if (info.LastCard != null)
                {
                    lastCard = info.LastCard;
                    Label_LastRank.Content = lastCard.Rank;
                    Label_LastSuit.Content = lastCard.Suit;
                }

                Label_WinStreakScore.Content = info.WinStreak.ToString();
                gameOver = info.GameOver;

                switch(info.WinStreak)
                {
                    case 0:
                        DisableAllButtons();
                        Button_Black.IsEnabled = true;
                        Button_Red.IsEnabled = true;
                        Label_QuestionText.Content = "Will the next card be Black or Red?";
                        break;
                    case 1:
                        DisableAllButtons();
                        Button_High.IsEnabled = true;
                        Button_Low.IsEnabled = true;
                        Label_QuestionText.Content = "Will the next card be higher or lower than the current card?";
                        break;
                    case 2:
                        DisableAllButtons();
                        Button_In.IsEnabled = true;
                        Button_Out.IsEnabled = true;
                        Label_QuestionText.Content = "Will the next card be inside(inclusive) or outside the last and current card?";
                        break;
                    case 3:
                        DisableAllButtons();
                        Button_Face.IsEnabled = true;
                        Button_NotFace.IsEnabled = true;
                        Label_QuestionText.Content = "Will the next card be a face(Jack, Queen, King) card or not?";
                        break;
                }

                if (gameOver && int.Parse(Label_WinStreakScore.Content.ToString()) == 4)
                {
                    DisableAllButtons();
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
    }
}
