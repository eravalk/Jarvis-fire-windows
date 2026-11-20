using System.Windows;
using System.Windows.Controls;
namespace JarvisFire.Desktop;
public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
    private void Section_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string section)
            SectionTitle.Text = section;
    }
}