namespace blog.dachs.ServerManager
{
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    /// <summary>
    /// Extension class for encryption based on an algorithm described in https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
    /// Changed the fixed derivationIterations to a random short-number and algorithm to AesCng.
    /// </summary>
    public static class GlobalEncryptionAes
    {
        public static string Encrypt(this string plainText)
        {
            return Encrypt(plainText, GeneratePassPhrase());
        }

        public static string Encrypt(this string plainText, string passPhrase)
        {
            // DerivationIterations, Salt and IV are randomly generated each time, but is prepended to encrypted cipher text, so they can be used for decryption
            byte[] derivationIterationsBytes = Generate16BitDerivationIterations();
            int derivationIterations = (int)BitConverter.ToUInt16(derivationIterationsBytes);
            if (derivationIterations == 0) derivationIterations++;

            byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate128BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(32);
                using (AesCng symmetricKey = new AesCng())
                {
                    symmetricKey.KeySize = 256;
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                // Create the final bytes as a concatenation of the random derivation iteration bytes, the random salt bytes, the random iv bytes and the cipher bytes.
                                byte[] cipherTextBytes;
                                cipherTextBytes = derivationIterationsBytes;
                                cipherTextBytes = cipherTextBytes.Concat(saltStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();

                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// decrypts a string
        /// </summary>
        /// <param name="cipherText">encrypted string</param>
        /// <returns></returns>
        public static string Decrypt(this string cipherText)
        {
            return Decrypt(cipherText, GeneratePassPhrase());
        }

        /// <summary>
        /// decrypts a string
        /// </summary>
        /// <param name="cipherText">encrypted string</param>
        /// <param name="passPhrase">encryption key</param>
        /// <returns></returns>
        public static string Decrypt(this string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [2 byte of Derivation Iteration], [32 bytes of Salt] + [16 bytes of IV] + [n bytes of CipherText]
            byte[] cipherTextBytesWithDerivationIterationSaltIv = Convert.FromBase64String(cipherText);

            // Get the DerivationIterationBytes by extracting the first 2 bytes from the supplied cipherText bytes.
            byte[] derivationIterationsBytes = cipherTextBytesWithDerivationIterationSaltIv.Take(2).ToArray();
            int derivationIterations = (int)BitConverter.ToUInt16(derivationIterationsBytes);

            // Get the saltStringBytes by extracting the next 32 bytes from the supplied cipherText bytes.
            byte[] saltStringBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(2).Take(32).ToArray();

            // Get the ivStringBytes by extracting the next 16 bytes from the supplied cipherText bytes.
            byte[] ivStringBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(34).Take(16).ToArray();

            // Get the actual cipher text bytes by removing the first 50 bytes (2 + 32 + 16) from the cipherText string.
            byte[] cipherTextBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(50).Take(cipherTextBytesWithDerivationIterationSaltIv.Length - 50).ToArray();

            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations))
            {
                var keyBytes = password.GetBytes(32);
                using (AesCng symmetricKey = new AesCng())
                {
                    symmetricKey.KeySize = 256;
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();

                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// generates string dependent on machine name and user name and SID of user
        /// </summary>
        /// <returns>passphrase</returns>
        private static string GeneratePassPhrase()
        {
            string machineName;
            string currentUserName;
            SecurityIdentifier? currentUserSecurityIdentifier;
            string currentUserSecurityIdentifierString;
            string passPhrasString;
            byte[] passPhraseBytes;

            machineName = Environment.MachineName;
            currentUserName = WindowsIdentity.GetCurrent().Name;
            currentUserSecurityIdentifier = WindowsIdentity.GetCurrent().User;
            if (currentUserSecurityIdentifier is not null)
            {
                currentUserSecurityIdentifierString = currentUserSecurityIdentifier.Value;
            }
            else
            {
                currentUserSecurityIdentifierString = "";
            }

            passPhrasString = machineName + currentUserName + currentUserSecurityIdentifierString;
            passPhraseBytes = Encoding.UTF8.GetBytes(passPhrasString);

            return Convert.ToBase64String(passPhraseBytes);
        }

        /// <summary>
        /// generates int for DerivationIteration
        /// </summary>
        /// <returns>short random number</returns>
        private static byte[] Generate16BitDerivationIterations()
        {
            byte[] randomBytes = new byte[2];

            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }

        /// <summary>
        /// generates 16 bytes or 128 bit entropy
        /// </summary>
        /// <returns>random 16 bytes or 128 bit entropy</returns>
        private static byte[] Generate128BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[16];

            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }

        /// <summary>
        /// generates 32 bytes or 256 bit entropy
        /// </summary>
        /// <returns>random 32 bytes or 256 bit entropy</returns>
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32];

            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }
    }
}
