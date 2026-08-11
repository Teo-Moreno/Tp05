using Microsoft.AspNetCore.Mvc;
using tp05.Data;
using tp05.Models;

namespace tp05.Controllers;

public class AccountController : Controller
{
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (Database.GetUsuarioByUsername(model.Username ?? "") != null)
        {
            ModelState.AddModelError(nameof(model.Username), "El nombre de usuario ya existe.");
            return View(model);
        }

        var usuario = new Usuario
        {
            Username = model.Username,
            PasswordHash = model.Password,
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            TipoUsuario = model.TipoUsuario
        };

        Database.AddUsuario(usuario);

        TempData["RegisterSuccess"] = "Registro exitoso. Ahora puede iniciar sesión.";
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        ViewBag.Message = TempData["RegisterSuccess"];
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = Database.ValidateCredentials(model.Username ?? "", model.Password ?? "");
        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", usuario.Id);
        HttpContext.Session.SetString("Username", usuario.Username ?? "");
        HttpContext.Session.SetString("Nombre", usuario.Nombre ?? "");
        HttpContext.Session.SetString("Apellido", usuario.Apellido ?? "");
        HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario ?? "");

        return RedirectToAction("Bienvenida");
    }

    public IActionResult Bienvenida()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null || username == "")
        {
            return RedirectToAction("Login");
        }

        var model = new Usuario
        {
            Username = username,
            Nombre = HttpContext.Session.GetString("Nombre") ?? "",
            Apellido = HttpContext.Session.GetString("Apellido") ?? "",
            TipoUsuario = HttpContext.Session.GetString("TipoUsuario") ?? ""
