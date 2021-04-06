/* Programmers: Colin Manliclic, Zina Long
 * Date:        April 9, 2021
 * Purpose:
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RideTheBusGUIClient
{
    /// <summary>
    /// Interaction logic for RuleBook.xaml
    /// </summary>
    public partial class RuleBook : Window
    {
        public RuleBook()
        {
            InitializeComponent();
            this.TextBox_Rules.Text = "1. Ride the Bus is based on guessing. You guess 4 in a row, you win. You have until you run out of cards in the deck(52).\n\n" +
                                      "2. RED/ BLACK - Guess whether the next card is Red(Hearts Diamonds) or Black(Spades, Clubs).\n\n" +
                                      "3. HIGH/ LOW - Guess whether the next card is higher or lower than the next card. Tie is automatic win.\n\n" +
                                      "4. IN/ OUT - Guess whether the next card is inside or outside of the last and current card inclusive.\n\n" +
                                      "5. FACE/ NOT FACE Guess whether the card is a Face(Jack, Queen or King) or not.\n\n" + 
                                      "6. If you guess any of these wrong, Your winstreak goes to 0 and you start the guess at RED/ BLACK again.";
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
