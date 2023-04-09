using ei8.Cortex.Gps.Sender.ViewModels;

namespace ei8.Cortex.Gps.Sender.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        BindingContext = mainViewModel;
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}