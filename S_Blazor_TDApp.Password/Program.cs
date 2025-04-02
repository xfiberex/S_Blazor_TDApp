using S_Blazor_TDApp.Server.Utilities;

// Programa de consola, para utilizar el metodo que hashea la contraseña, en caso de querer otra para el admin por defecto.
string hashedPassword = PasswordHelper.HashPassword("pass123"); // <--- Aqui se pone la contraseña que se quiere hashear
Console.WriteLine("La contraseña hasheada es:\n\n" + hashedPassword);