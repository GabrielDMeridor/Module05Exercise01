using Module0Exercise01.Services;
using MySql.Data.MySqlClient;

namespace Module05Exercise01;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly DatabaseConnectionService _databaseConnectionService;

    public MainPage()
    {
        InitializeComponent();
        _databaseConnectionService = new DatabaseConnectionService();
    }

    private async void OnTestConnectionClicked(object sender, EventArgs e)
    {
        var connectionString = _databaseConnectionService.GetConnectionString();
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                ConnectionStatusLabel.Text = "Connection Successful!";
                ConnectionStatusLabel.TextColor = Colors.Green;
            }
        }
        catch (Exception ex)
        {
            ConnectionStatusLabel.Text = $"Connection Failed: {ex.Message}";
            ConnectionStatusLabel.TextColor = Colors.Red;
        }
    }

    private async void OpenViewEmployee(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ViewEmployee");
    }
}
