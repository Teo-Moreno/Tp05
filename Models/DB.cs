namespace tp05.Models;

using System.Data.SqlClient;
using Dapper;

public static class DB
{
    private static readonly string _connectionString =
        @"Server=localhost;Database=LogIn2026SQL;User Id=alumno;Password=alumno;TrustServerCertificate=True;";


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