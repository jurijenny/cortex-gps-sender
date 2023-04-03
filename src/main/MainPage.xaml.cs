namespace ei8.Cortex.Gps.Sender;

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