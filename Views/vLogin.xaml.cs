using DarwinPullaguariComplementario.Services;
using MySql.Data.MySqlClient;
namespace DarwinPullaguariComplementario.Views;



public partial class vLogin : ContentPage
{
    string connectionString = "Server=localhost;Database=DBUISRAEL;Uid=root;Pwd=;";
    public vLogin()
	{
		InitializeComponent();
	}
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var usuario = UsuarioEntry.Text;
        var contrasena = ContrasenaEntry.Text;

        using var connection = new DatabaseService().GetConnection();
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT * FROM ESTUDIANTES_LOGIN WHERE USUARIO=@Usuario AND CONTRASEÑA=@Contrasena", connection);
        command.Parameters.AddWithValue("@Usuario", usuario);
        command.Parameters.AddWithValue("@Contrasena", contrasena);

        using var reader = await command.ExecuteReaderAsync();
        if (reader.HasRows)
        {
            await Navigation.PushAsync(new PrincipalPage());
        }
        else
        {
            ErrorLabel.Text = "Usuario o contraseña incorrectos.";
            ErrorLabel.IsVisible = true;
        }
    }
}