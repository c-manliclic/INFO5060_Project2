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

            TextBox_CardsLeft.Text = bus.NumCards.ToString();
        }

        private void Button_Black_Click(object sender, RoutedEventArgs e)
        {
            //bus.Draw();
        }

        private void Button_RuleBook_Click(object sender, RoutedEventArgs e)
        {
            new RuleBook().ShowDialog();
        }

        public void UpdateClient(CallbackInfo info)
        {
            if(System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                // Update gui
                
            }
        }
    }
}
