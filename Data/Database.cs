using System.Text;
using Microsoft.Data.SqlClient;
using Dapper;
using tp05.Models;

namespace tp05.Data;

public static class Database
{
    private static string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration.GetConnectionString("DefaultConnection") ?? "";
    }

    // Inicializa la base de datos y crea la tabla si no existe
    public static void Initialize()
    {
        using var connection = new SqlConnection(GetConnectionString());
        connection.Open();

        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuarios')
            BEGIN
                CREATE TABLE Usuarios (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Username NVARCHAR(30) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(255) NOT NULL,
                    Nombre NVARCHAR(50) NOT NULL,
                    Apellido NVARCHAR(50) NOT NULL,
                    TipoUsuario NVARCHAR(30) NOT NULL
                );
            END
        ";

        connection.Execute(sql);
    }

    // Obtiene un usuario por nombre de usuario
    public static Usuario GetUsuarioByUsername(string username)
    {
        using var connection = new SqlConnection(GetConnectionString());
        var sql = "SELECT Id, Username, PasswordHash, Nombre, Apellido, TipoUsuario FROM Usuarios WHERE Username = @Username";
        var usuario = connection.QueryFirstOrDefault<Usuario>(sql, new { Username = username });
        return usuario;
    }

    // Agrega un nuevo usuario a la base de datos
    public static bool AddUsuario(Usuario usuario)
    {
        if (GetUsuarioByUsername(usuario.Username ?? "") != null)
        {
            return false;
        }

        var passwordHash = HashPassword(usuario.PasswordHash ?? "");

        using var connection = new SqlConnection(GetConnectionString());
        var sql = @"
            INSERT INTO Usuarios (Username, PasswordHash, Nombre, Apellido, TipoUsuario)
            VALUES (@Username, @PasswordHash, @Nombre, @Apellido, @TipoUsuario)
        ";

        var result = connection.Execute(sql, new
        {
            Username = usuario.Username,
            PasswordHash = passwordHash,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            TipoUsuario = usuario.TipoUsuario
        });

        return result == 1;
    }

    // Valida credenciales del usuario
    public static Usuario ValidateCredentials(string username, string password)
    {
        var usuario = GetUsuarioByUsername(username);
        if (usuario == null)
        {
            return null;
        }

        var passwordHash = usuario.PasswordHash ?? "";
        if (VerifyPassword(password, passwordHash))
        {
            return usuario;
        }

        return null;
    }

    // Encripta una contraseña con Base64
    private static string HashPassword(string password)
    {
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(passwordBytes);
    }

    // Verifica una contraseña contra su hash
    private static bool VerifyPassword(string password, string storedHash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == storedHash;
    }
}
