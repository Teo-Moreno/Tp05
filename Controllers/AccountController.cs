using Microsoft.AspNetCore.Mvc;
using tp05.Models;

namespace tp05.Controllers;

public class AccountController : Controller
{
    public IActionResult Register()
    {
        return View(new Usuario());
    }

    [HttpPost]
    public IActionResult Register(Usuario model)
    {
        if (model.Username == null || model.Username == "")
        {
            ModelState.AddModelError("Username", "El nombre de usuario es obligatorio.");
            return View(model);
        }

        if (model.Password == null || model.Password == "")
        {
            ModelState.AddModelError("Password", "La contraseña es obligatoria.");
            return View(model);
        }

        if (model.Nombre == null || model.Nombre == "")
        {
            ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
            return View(model);
        }

        if (model.Apellido == null || model.Apellido == "")
        {
            ModelState.AddModelError("Apellido", "El apellido es obligatorio.");
            return View(model);
        }

        if (model.TipoUsuario == null || model.TipoUsuario == "")
        {
            ModelState.AddModelError("TipoUsuario", "Debe seleccionar un tipo de usuario.");
            return View(model);
        }

        if (Database.GetUsuarioByUsername(model.Username) != null)
        {
            ModelState.AddModelError("Username", "El nombre de usuario ya existe.");
            return View(model);
        }

        var created = Database.AddUsuario(model);
        if (!created)
        {
            ModelState.AddModelError("Username", "El nombre de usuario ya existe.");
            return View(model);
        }

        TempData["RegisterSuccess"] = "Registro exitoso. Ahora puede iniciar sesión.";
        return RedirectToAction("Login");
    }

    public IActionResult Login()
    {
        ViewBag.Message = TempData["RegisterSuccess"];
        return View(new Usuario());
    }

    [HttpPost]
    public IActionResult Login(Usuario model)
    {
        if (model.Username == null || model.Username == "" || model.Password == null || model.Password == "")
        {
            ModelState.AddModelError(string.Empty, "Usuario y contraseña son obligatorios.");
            return View(model);
        }

        var usuario = Database.ValidateCredentials(model.Username, model.Password);
        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        HttpContext.Session.SetInt32("UserId", usuario.Id);
        HttpContext.Session.SetString("Username", usuario.Username);
        HttpContext.Session.SetString("Nombre", usuario.Nombre);
        HttpContext.Session.SetString("Apellido", usuario.Apellido);
        HttpContext.Session.SetString("TipoUsuario", usuario.TipoUsuario);

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
            Nombre = HttpContext.Session.GetString("Nombre") ?? string.Empty,
            Apellido = HttpContext.Session.GetString("Apellido") ?? string.Empty,
            TipoUsuario = HttpContext.Session.GetString("TipoUsuario") ?? string.Empty
        };

        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}

