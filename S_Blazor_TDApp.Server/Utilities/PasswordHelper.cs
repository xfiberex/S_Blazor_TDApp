using System.Security.Cryptography;
using System.Text;

namespace S_Blazor_TDApp.Server.Utilities
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hashea la contraseña usando HMACSHA512. 
        /// El resultado se almacena en el formato: salt:hash (ambos en Base64)
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                var salt = hmac.Key; // clave generada aleatoriamente que funciona como salt
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(passwordBytes);
                return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
            }
        }

        /// <summary>
        /// Verifica que la contraseña proporcionada, una vez hasheada con el salt almacenado, 
        /// coincida con el hash guardado.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hash);
            }
        }
    }
}