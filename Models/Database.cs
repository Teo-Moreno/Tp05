using System.Text;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace tp05.Models;

public static class Database
{
    private static string GetConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        return configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }

    // Inicializa la base de datos y crea la tabla si no existe
    public static void Initialize()
    {
        var cs = GetConnectionString();
        var builder = new SqlConnectionStringBuilder(cs);
        var dbName = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(dbName))
        {
            dbName = "tp05db";
            builder.InitialCatalog = dbName;
            cs = builder.ConnectionString;
        }

        // Ensure database exists: connect to master and create DB if missing
        try
        {
            using var testConn = new SqlConnection(cs);
            testConn.Open();
        }
        catch (SqlException)
        {
            var masterBuilder = new SqlConnectionStringBuilder(GetConnectionString()) { InitialCatalog = "master" };
            using var masterConn = new SqlConnection(masterBuilder.ConnectionString);
            masterConn.Open();
            var createDbSql = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}];";
            masterConn.Execute(createDbSql);
        }

        using var connection = new SqlConnection(cs);
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


    public static Usuario? GetUsuarioByUsername(string username)
    {
        using var connection = new SqlConnection(GetConnectionString());
        var sql = "SELECT Id, Username, PasswordHash AS Password, Nombre, Apellido, TipoUsuario FROM Usuarios WHERE Username = @Username";
        var usuario = connection.QueryFirstOrDefault<Usuario>(sql, new { Username = username });
        return usuario; 
    }


    public static bool AddUsuario(Usuario usuario)
    {
        if (usuario.Username == null || usuario.Username == string.Empty)
        {
            return false;
        }

        if (GetUsuarioByUsername(usuario.Username) != null)
        {
            return false;
        }

        if (usuario.Password == null || usuario.Password == string.Empty)
        {
            return false;
        }

        var passwordHash = HashPassword(usuario.Password);

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

    public static Usuario? ValidateCredentials(string username, string password)
    {
        var usuario = GetUsuarioByUsername(username);
        if (usuario == null)
        {
            return null;
        }

        if (usuario.Password == null)
        {
            return null;
        }

        if (VerifyPassword(password, usuario.Password))
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
