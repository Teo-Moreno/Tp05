using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace tp05.Models;

public static class Database
{
    private static string connectionString;

    public static void Initialize()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        Initialize(configuration);
    }

    public static void Initialize(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public static Usuario GetUsuarioByUsername(string username)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string sql = @"SELECT Id, Username, Password, Nombre, Apellido, TipoUsuario
                           FROM Usuarios
                           WHERE Username = @Username";

            return connection.QueryFirstOrDefault<Usuario>(sql, new { Username = username });
        }
    }

    public static bool AddUsuario(Usuario usuario)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string sql = @"INSERT INTO Usuarios
                           (Username, Password, Nombre, Apellido, TipoUsuario)
                           VALUES
                           (@Username, @Password, @Nombre, @Apellido, @TipoUsuario)";

            int cantidad = connection.Execute(sql, usuario);
            return cantidad == 1;
        }
    }

    public static Usuario ValidateCredentials(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string sql = @"SELECT Id, Username, Password, Nombre, Apellido, TipoUsuario
                           FROM Usuarios
                           WHERE Username = @Username
                           AND Password = @Password";

            return connection.QueryFirstOrDefault<Usuario>(
                sql,
                new
                {
                    Username = username,
                    Password = password
                }
            );
        }
    }

    public static Usuario GetUsuario(string username)
    {
        return GetUsuarioByUsername(username);
    }

    public static bool AgregarUsuario(Usuario usuario)
    {
        return AddUsuario(usuario);
    }

    public static Usuario ValidarUsuario(string username, string password)
    {
        return ValidateCredentials(username, password);
    }
}