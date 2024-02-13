namespace blog.dachs.ServerManager
{
    public class ProcessOutput
    {
        public long handle;
        public string randomNumber;

        public ProcessOutput(long handle)
        {
            this.handle = handle;
            this.randomNumber = string.Empty;

            byte[] randomNumberBytes = GlobalEncryptionAes.GenerateBitsOfRandomEntropy(32);
            
            foreach (byte randomNumberByte in randomNumberBytes.ToList<byte>())
            {
                this.randomNumber += Convert.ToString(randomNumberByte, 16);
            }
        }
    }
}
