document.addEventListener('DOMContentLoaded', function () {
    var registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function (event) {
            var username = document.getElementById('Username').value;
            var password = document.getElementById('Password').value;
            var nombre = document.getElementById('Nombre').value;
            var apellido = document.getElementById('Apellido').value;
            var tipoUsuario = document.getElementById('TipoUsuario').value;
            var errorDiv = document.getElementById('registerError');
            var nameRegex = /^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$/;
            var errors = [];

            var usernameRegex = /^[a-zA-Z0-9_]+$/;
            if (username.length < 4) {
                errors.push('El nombre de usuario debe tener al menos 4 caracteres.');
            }
            if (!usernameRegex.test(username)) {
                errors.push('El nombre de usuario sólo puede contener letras, números y guión bajo.');
            }
            if (password.length < 8) {
                errors.push('La contraseña debe tener al menos 8 caracteres.');
            }
            if (nombre.length < 2) {
                errors.push('El nombre es obligatorio.');
            }
            if (!nameRegex.test(nombre)) {
                errors.push('Nombre inválido. Sólo letras y espacios.');
            }
            if (apellido.length < 2) {
                errors.push('El apellido es obligatorio.');
            }
            if (!nameRegex.test(apellido)) {
                errors.push('Apellido inválido. Sólo letras y espacios.');
            }
            if (!tipoUsuario) {
                errors.push('Debe seleccionar un tipo de usuario.');
            }

            if (errors.length > 0) {
                event.preventDefault();
                errorDiv.textContent = errors.join(' ');
            }
        });
    }

    var loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (event) {
            var username = document.getElementById('Username').value;
            var password = document.getElementById('Password').value;
            var errorDiv = document.getElementById('loginError');
            var errors = [];

            if (username.length === 0) {
                errors.push('El nombre de usuario es obligatorio.');
            }
            if (password.length === 0) {
                errors.push('La contraseña es obligatoria.');
            }

            if (errors.length > 0) {
                event.preventDefault();
                errorDiv.textContent = errors.join(' ');
            }
        });
    }
});
