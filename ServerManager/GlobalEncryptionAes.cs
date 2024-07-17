namespace blog.dachs.ServerManager
{
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    /// <summary>
    /// Extension class for encryption based on an algorithm described in https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
    /// Changed the fixed derivationIterations to a random short-number and algorithm to AesCng.
    /// Changed HashAlgorithm to SHA512, as SHA1 became deprecated
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
            byte[] derivationIterationsBytes = GenerateBitsOfRandomEntropy(2);
            int derivationIterations = (int)BitConverter.ToUInt16(derivationIterationsBytes);
            if (derivationIterations == 0) derivationIterations++;

            byte[] saltStringBytes = GenerateBitsOfRandomEntropy(32);
            byte[] ivStringBytes = GenerateBitsOfRandomEntropy(16);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations, HashAlgorithmName.SHA512))
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
            string plainText = string.Empty;

            // Get the complete stream of bytes that represent:
            // [2 byte of Derivation Iteration], [32 bytes of Salt] + [16 bytes of IV] + [n bytes of CipherText]
            byte[] cipherTextBytesWithDerivationIterationSaltIv = Convert.FromBase64String(cipherText);

            if (cipherTextBytesWithDerivationIterationSaltIv.Length <= 50)
            {
                throw new ArgumentException("cipherText is not valid");
            }

            // Get the DerivationIterationBytes by extracting the first 2 bytes from the supplied cipherText bytes.
            byte[] derivationIterationsBytes = cipherTextBytesWithDerivationIterationSaltIv.Take(2).ToArray();
            int derivationIterations = (int)BitConverter.ToUInt16(derivationIterationsBytes);

            // Get the saltStringBytes by extracting the next 32 bytes from the supplied cipherText bytes.
            byte[] saltStringBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(2).Take(32).ToArray();

            // Get the ivStringBytes by extracting the next 24 bytes from the supplied cipherText bytes.
            byte[] ivStringBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(34).Take(16).ToArray();

            // Get the actual cipher text bytes by removing the first 58 bytes (2 + 32 + 16) from the cipherText string.
            byte[] cipherTextBytes = cipherTextBytesWithDerivationIterationSaltIv.Skip(50).Take(cipherTextBytesWithDerivationIterationSaltIv.Length - 50).ToArray();

            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, derivationIterations, HashAlgorithmName.SHA512))
            {
                var keyBytes = password.GetBytes(32);
                using (AesCng symmetricKey = new AesCng())
                {
                    symmetricKey.KeySize = 256;
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (StreamReader plainTextReader = new StreamReader(cryptoStream))
                                {
                                    plainText = plainTextReader.ReadToEnd();

                                    memoryStream.Close();
                                    cryptoStream.Close();
                                }

                                return plainText;
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
        /// generates entropy
        /// </summary>
        /// <returns>random entropy of length</returns>
        public static byte[] GenerateBitsOfRandomEntropy(byte numberOfBytes)
        {
            byte[] randomBytes = new byte[numberOfBytes];

            RandomNumberGenerator.Fill(randomBytes);

            return randomBytes;
        }
    }
}
