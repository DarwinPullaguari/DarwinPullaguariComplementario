namespace DarwinPullaguariComplementario.Views;
using DarwinPullaguariComplementario.Models;
using System.Collections.ObjectModel;
using System.Data;
using DarwinPullaguariComplementario.Services;

public partial class PrincipalPage : ContentPage
{
    string connectionString = "Server=localhost;Database=DBUISRAEL;Uid=root;Pwd=;";
    ObservableCollection<Estudiante> estudiantes = new ObservableCollection<Estudiante>();
    public PrincipalPage()
	{
		InitializeComponent();
        LoadEstudiantes();
    }

    private async void LoadEstudiantes()
    {
        estudiantes.Clear();

        using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var command = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM ESTUDIANTES", connection);
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    estudiantes.Add(new Estudiante
                    {
                        COD_ESTUDIANTE = reader.GetInt32("COD_ESTUDIANTE"),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        Curso = reader.GetString("Curso"),
                        Paralelo = reader.GetString("Paralelo"),
                        NOTA_FINAL = reader.GetFloat("NOTA_FINAL")
                    });
                }
            }
        }

        EstudiantesList.ItemsSource = estudiantes;
    }
    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        string nombre = await DisplayPromptAsync("Nuevo Estudiante", "Ingrese el nombre:");
        string apellido = await DisplayPromptAsync("Nuevo Estudiante", "Ingrese el apellido:");
        string curso = await DisplayPromptAsync("Nuevo Estudiante", "Ingrese el curso:");
        string paralelo = await DisplayPromptAsync("Nuevo Estudiante", "Ingrese el paralelo:");
        string notaFinalStr = await DisplayPromptAsync("Nuevo Estudiante", "Ingrese la nota final:");
        if (!float.TryParse(notaFinalStr, out float notaFinal))
        {
            await DisplayAlert("Error", "La nota final no es válida.", "OK");
            return;
        }

        var nuevoEstudiante = new Estudiante
        {
            Nombre = nombre,
            Apellido = apellido,
            Curso = curso,
            Paralelo = paralelo,
            NOTA_FINAL = notaFinal
        };

        using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var command = new MySql.Data.MySqlClient.MySqlCommand(
                "INSERT INTO ESTUDIANTES (Nombre, Apellido, Curso, Paralelo, NOTA_FINAL) VALUES (@Nombre, @Apellido, @Curso, @Paralelo, @NotaFinal)",
                connection);

            command.Parameters.AddWithValue("@Nombre", nuevoEstudiante.Nombre);
            command.Parameters.AddWithValue("@Apellido", nuevoEstudiante.Apellido);
            command.Parameters.AddWithValue("@Curso", nuevoEstudiante.Curso);
            command.Parameters.AddWithValue("@Paralelo", nuevoEstudiante.Paralelo);
            command.Parameters.AddWithValue("@NotaFinal", nuevoEstudiante.NOTA_FINAL);

            await command.ExecuteNonQueryAsync();
        }

        LoadEstudiantes();
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (EstudiantesList.SelectedItem is Estudiante estudianteSeleccionado)
        {
            if (estudianteSeleccionado.COD_ESTUDIANTE <= 0)
            {
                await DisplayAlert("Error", "El estudiante seleccionado no tiene un ID válido.", "OK");
                return;
            }


            string nuevoNombre = await DisplayPromptAsync("Editar Estudiante", "Ingrese el nuevo nombre:", initialValue: estudianteSeleccionado.Nombre);
            string nuevoApellido = await DisplayPromptAsync("Editar Estudiante", "Ingrese el nuevo apellido:", initialValue: estudianteSeleccionado.Apellido);
            string nuevoCurso = await DisplayPromptAsync("Editar Estudiante", "Ingrese el nuevo curso:", initialValue: estudianteSeleccionado.Curso);
            string nuevoParalelo = await DisplayPromptAsync("Editar Estudiante", "Ingrese el nuevo paralelo:", initialValue: estudianteSeleccionado.Paralelo);
            string nuevaNotaStr = await DisplayPromptAsync("Editar Estudiante", "Ingrese la nueva nota final:", initialValue: estudianteSeleccionado.NOTA_FINAL.ToString());


            if (!float.TryParse(nuevaNotaStr, out float nuevaNota))
            {
                await DisplayAlert("Error", "La nota final no es válida.", "OK");
                return;
            }


            estudianteSeleccionado.Nombre = nuevoNombre;
            estudianteSeleccionado.Apellido = nuevoApellido;
            estudianteSeleccionado.Curso = nuevoCurso;
            estudianteSeleccionado.Paralelo = nuevoParalelo;
            estudianteSeleccionado.NOTA_FINAL = nuevaNota;


            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySql.Data.MySqlClient.MySqlCommand(
                    @"UPDATE ESTUDIANTES 
                  SET Nombre=@Nombre, Apellido=@Apellido, Curso=@Curso, Paralelo=@Paralelo, NOTA_FINAL=@NotaFinal 
                  WHERE COD_ESTUDIANTE=@CodEstudiante", connection);

                command.Parameters.AddWithValue("@Nombre", estudianteSeleccionado.Nombre);
                command.Parameters.AddWithValue("@Apellido", estudianteSeleccionado.Apellido);
                command.Parameters.AddWithValue("@Curso", estudianteSeleccionado.Curso);
                command.Parameters.AddWithValue("@Paralelo", estudianteSeleccionado.Paralelo);
                command.Parameters.AddWithValue("@NotaFinal", estudianteSeleccionado.NOTA_FINAL);
                command.Parameters.AddWithValue("@CodEstudiante", estudianteSeleccionado.COD_ESTUDIANTE);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                {
                    await DisplayAlert("Error", "No se pudo actualizar el estudiante. Verifica los datos.", "OK");
                    return;
                }
            }


            LoadEstudiantes();
        }
        else
        {
            await DisplayAlert("Error", "Selecciona un estudiante para editar", "OK");
        }
    }


    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (EstudiantesList.SelectedItem is Estudiante estudianteSeleccionado)
        {
            using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var command = new MySql.Data.MySqlClient.MySqlCommand(
                    "DELETE FROM ESTUDIANTES WHERE COD_ESTUDIANTE=@CodEstudiante",
                    connection);

                command.Parameters.AddWithValue("@CodEstudiante", estudianteSeleccionado.COD_ESTUDIANTE);

                await command.ExecuteNonQueryAsync();
            }

            LoadEstudiantes();
        }
        else
        {
            await DisplayAlert("Error", "Selecciona un estudiante para eliminar", "OK");
        }
    }
    private async void OnAbrirCamaraClicked(object sender, EventArgs e)
    {
        var photo = await MediaPicker.Default.CapturePhotoAsync();
        if (photo != null)
        {
            var stream = await photo.OpenReadAsync();

        }
    }
    private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Estudiante estudianteSeleccionado)
        {

        }
    }
}