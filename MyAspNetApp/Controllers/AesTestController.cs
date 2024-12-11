using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MyAspNetApp.Controllers
{
    public class AesTestController : Controller
    {
        // Static Base64-encoded key and IV
        private static readonly string Base64Key = "2vK73pii4L/dL7Ep9oRotR4YriqV2h+rd6AkxxXlY4c=";
        private static readonly string Base64IV = "6kraZLq1nGe7iVbyoJw6Zg==";

        // Convert Base64 key and IV to byte arrays
        private static readonly byte[] Key = Convert.FromBase64String(Base64Key);
        private static readonly byte[] IV = Convert.FromBase64String(Base64IV);

        // GET: /AesTest/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: /AesTest/Encrypt
        [HttpPost]
        public IActionResult Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                ViewBag.ErrorMessage = "Plaintext cannot be empty.";
                return View("Index");
            }

            // Mã hóa sử dụng khóa và IV cố định
            byte[] encryptedBytes = AesEncryption.Encrypt(plaintext, Key, IV);
            string encryptedText = Convert.ToBase64String(encryptedBytes);

            // Gửi dữ liệu mã hóa về view
            ViewBag.EncryptedText = encryptedText;
            ViewBag.Key = Base64Key; // Hiển thị key dưới dạng Base64
            ViewBag.IV = Base64IV;   // Hiển thị IV dưới dạng Base64
            ViewBag.Plaintext = plaintext;

            return View("Index");
        }

        // POST: /AesTest/Decrypt
        [HttpPost]
        public IActionResult Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                ViewBag.ErrorMessage = "Ciphertext cannot be empty.";
                return View("Index");
            }

            try
            {
                // Chuyển đổi chuỗi Base64 thành byte[]
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                // Giải mã
                string decryptedText = AesEncryption.Decrypt(encryptedBytes, Key, IV);

                // Gửi dữ liệu giải mã về view
                ViewBag.DecryptedText = decryptedText;
                ViewBag.EncryptedText = encryptedText;
                ViewBag.Key = Base64Key;
                ViewBag.IV = Base64IV;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Decryption failed: {ex.Message}";
            }

            return View("Index");
        }
    }

    public static class AesEncryption
    {
        // Hàm mã hóa chuỗi
        public static byte[] Encrypt(string plaintext, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        // Hàm giải mã chuỗi
        public static string Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new System.IO.MemoryStream(ciphertext))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new System.IO.MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            return Encoding.UTF8.GetString(msPlain.ToArray());
                        }
                    }
                }
            }
        }
    }
}
