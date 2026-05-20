using System.Security.Cryptography;

namespace ApiFinanceiro.Services
{
    public class PasswordService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;
        private const string Algorithm = "PBKDF2-SHA256";

        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"{Algorithm}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split('$');

            if (parts.Length != 4 || parts[0] != Algorithm || !int.TryParse(parts[1], out var iterations))
            {
                return false;
            }

            byte[] salt;
            byte[] expectedHash;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedHash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}
