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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat_Server.View
{
    /// <summary>
    /// Interaction logic for Client_Label.xaml
    /// </summary>
    public partial class Client_Label : UserControl
    {
        public Client_Label()
        {
            InitializeComponent();
        }
        public Client_Label(string Text)
        {
            InitializeComponent();
            lbl_txt.Text = Text;
        }
    }
}
