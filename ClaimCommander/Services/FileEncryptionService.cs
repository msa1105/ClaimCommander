using System.Security.Cryptography;
using System.Text;

namespace ClaimCommander.Services
{
    /// <summary>
    /// Provides AES-based encryption and decryption services for file storage.
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item>
    /// StackOverflow (2016) ‘Encrypt and decrypt a file with AES in C#’, 
    /// <em>Stack Overflow</em>, available at: https://stackoverflow.com/questions/40829058/encrypt-and-decrypt-a-file-with-aes-in-c 
    /// (Accessed: 21 October 2025).
    /// </item>
    /// <item>
    /// StackOverflow (2019) ‘Correct AesCryptoServiceProvider usage’, 
    /// <em>Stack Overflow</em>, available at: https://stackoverflow.com/questions/56860188/correct-aescryptoserviceprovider-usage 
    /// (Accessed: 21 October 2025).
    /// </item>
    /// <item>
    /// StackOverflow (2013) ‘C# AES Decryption – encryption’, 
    /// <em>Stack Overflow</em>, available at: https://stackoverflow.com/questions/17511279/c-sharp-aes-decryption 
    /// (Accessed: 21 October 2025).
    /// </item>
    /// <item>
    /// Littlemaninmyhead (2021) ‘If you copied any of these popular StackOverflow encryption code snippets then you did it wrong’, 
    /// <em>Little Man in My Head</em>, available at: https://littlemaninmyhead.wordpress.com/2021/09/15/if-you-copied-any-of-these-popular-stackoverflow-encryption-code-snippets-then-you-did-it-wrong/ 
    /// (Accessed: 21 October 2025).
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IFileEncryptionService
    {
        Task<string> EncryptAndSaveFileAsync(IFormFile file, string uploadPath);
        Task<byte[]> DecryptFileAsync(string encryptedFilePath);
    }

    public class FileEncryptionService : IFileEncryptionService
    {
        // Use a fixed key for demonstration (in production, store securely in config/vault)
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public FileEncryptionService()
        {
            // Generate consistent key and IV (32 bytes for key, 16 for IV with AES)
            using var sha256 = SHA256.Create();
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes("ClaimCommanderSecretKey2024"));
            _iv = MD5.HashData(Encoding.UTF8.GetBytes("ClaimCommanderIV"));
        }

        /// <summary>
        /// Encrypts and saves a file asynchronously using AES.
        /// <para>
        /// Note: Fixed IV is used here only for demonstration. A unique IV per file is recommended 
        /// (see StackOverflow, 2019; StackOverflow, 2016).
        /// </para>
        /// </summary>
        public async Task<string> EncryptAndSaveFileAsync(IFormFile file, string uploadPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            // Create upload directory if it doesn't exist
            Directory.CreateDirectory(uploadPath);

            // Generate unique encrypted file name
            var encryptedFileName = $"{Guid.NewGuid()}.encrypted";
            var encryptedFilePath = Path.Combine(uploadPath, encryptedFileName);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var fileStream = new FileStream(encryptedFilePath, FileMode.Create);
            using var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await file.CopyToAsync(cryptoStream);

            return encryptedFilePath;
        }

        /// <summary>
        /// Decrypts an AES-encrypted file and returns its bytes.
        /// <para>
        /// Ensure the correct key and IV are used (see StackOverflow, 2013; StackOverflow, 2019).
        /// </para>
        /// </summary>
        public async Task<byte[]> DecryptFileAsync(string encryptedFilePath)
        {
            if (!File.Exists(encryptedFilePath))
                throw new FileNotFoundException("Encrypted file not found", encryptedFilePath);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var fileStream = new FileStream(encryptedFilePath, FileMode.Open);
            using var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var memoryStream = new MemoryStream();

            await cryptoStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
