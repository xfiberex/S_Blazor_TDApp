namespace S_Blazor_TDApp.Server.Utilities
{
    public static class PasswordHelper
    {
        private const int WorkFactor = 12;

        /// <summary>
        /// Hashea la contraseña usando BCrypt con un work factor configurable.
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        /// <summary>
        /// Verifica que la contraseña proporcionada coincida con el hash almacenado.
        /// Soporta tanto el nuevo formato BCrypt como el formato legacy HMACSHA512 (salt:hash).
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            // Compatibilidad con hashes legacy (formato "salt:hash" de HMACSHA512)
            if (storedHash.Contains(':'))
            {
                return VerifyLegacyPassword(password, storedHash);
            }

            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        /// <summary>
        /// Verifica contraseñas con el formato legacy HMACSHA512 (salt:hash en Base64).
        /// </summary>
        private static bool VerifyLegacyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            using var hmac = new System.Security.Cryptography.HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}