using System.Windows;
using System.Windows.Controls;

namespace GameUI
{
    /// <summary>
    /// Interaction logic for PauseMenu.xaml
    /// </summary>
    public partial class PauseMenu : UserControl
    {
        public event Action<Option> OptionSelected;
        public PauseMenu()
        {
            InitializeComponent();
        }

        private void Continue_Clicked(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Continue);
        }

        private void Restart_Clicked(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }
    }
}
