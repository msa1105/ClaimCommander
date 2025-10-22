using System.Security.Cryptography;
using System.Text;

namespace ClaimCommander.Services
{
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