using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MyAspNetApp.Controllers
{
    public class HybridEncryptionController : Controller
    {
        // Sử dụng khóa RSA cứng (Public và Private)
        private static string publicKey = "<RSAKeyValue><Modulus>u9c5gSS9B/5dudtnoS5r5+7IagjAGywDmIbp1z7Mzck6Zd59SLTnSfjPI4BdRORlQ64gM9ygBpe/P4wc+8JkonhTBGuaAN3hxKbTHJwylMxhbkAixGwbnnoYaU/uSU6IDTwgLmdrwr+85xMadutJAV/+OdkZJnRIkVPs4kTX8shE5ovNBvtWLiENPwFl/nBMLaJMKDrMJCSBVSoQri58wvsGaRE+8o/+C5BJNxjmDZf5ukvzR7c2HNMNquk2aa/YBlehJ68kfmPoY4YjmPrLXLXdMciEGRU9pX3aq+XBli9l+L48P4qQZpi8pJjiNtWarLW7PFAIjxd7rBzgVN4mAQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        private static string privateKey = "<RSAKeyValue><Modulus>u9c5gSS9B/5dudtnoS5r5+7IagjAGywDmIbp1z7Mzck6Zd59SLTnSfjPI4BdRORlQ64gM9ygBpe/P4wc+8JkonhTBGuaAN3hxKbTHJwylMxhbkAixGwbnnoYaU/uSU6IDTwgLmdrwr+85xMadutJAV/+OdkZJnRIkVPs4kTX8shE5ovNBvtWLiENPwFl/nBMLaJMKDrMJCSBVSoQri58wvsGaRE+8o/+C5BJNxjmDZf5ukvzR7c2HNMNquk2aa/YBlehJ68kfmPoY4YjmPrLXLXdMciEGRU9pX3aq+XBli9l+L48P4qQZpi8pJjiNtWarLW7PFAIjxd7rBzgVN4mAQ==</Modulus><Exponent>AQAB</Exponent><P>5IbJD4Q08/JHyL3N1nAtOI2R2eglP3RVK3Pzap7USrVlA0S0/Y1Ygh6uRUfSmzJ1T1xD9f5WrMzeTAcI+c1RN5dhpvV4x6+tNFf2mAOh1u/5DNhWfYnS3rQ1pAuhd9vsAlMFF1WloIE2p3z3CdHfoPlIrjx7a616hR3VGY7BAW8=</P><Q>0mxHXlWuronYiHeUAEjzVzTi1ubBKEugEirvDj2kYKptA2RWdLl9Enyc40M/LqKvSBP7HLSBnCFZH9SnFtFIDjqv0tw7dppejZ20bL7YRuHo/9mwmJTYv9oFRPfeVI8HuxWcCVoExL6f3fgzBCguGs8LR8HkiNs23v7rWr14t48=</Q><DP>kUSM8uwbrSbKMvVUr4fHdzenC1nis5+DtIeUqKYmVQdSGu3GD5eWN8DBDxFE85rzr2r7WJhBCL+CX9no/sJhanE0hilsiaNG+wsfmipU6ojJTD0JXewKu77aillKXyLRt/iGN/sduhXpZDav4vIisfVFGl0gJ6azR4NbP0/bqMk=</DP><DQ>aLpvguoOONxuou0xAjtMsbimteK/fDi3gJNRy7quV1Y7T5be2NWBBHR0+T988M8TgkI/lvfYEkcevpLuXpLm0/4tiDsIdW07Zix5oi4SIqVJfB1yoV5jyojOWpcrVtahOGH2+UiOWwWtN6LScV02JS2rJZnBDsRcOPyTUuQ+cs0=</DQ><InverseQ>YHAzzw3nQSifD49CTaqzzsQN/XcCbKdQx7m4a7TMWJR660HecLEeFmtqsby2WoIWiX8TjpvMFYyu3dvTcPZefMHEsyN7X1ebAa4zyUuDfZyZGwCveQLLa26DzkkUg8sU/CYbUw3iEaizIoZn27rkwlHfc0hMP1VfIamUudA6eQU=</InverseQ><D>fZTjniKYuopl0QcNnmXxwGsU1EcrYD8p3ER+e+hBdmdNQqPf8Mb5RqwwmBSOxqtHxFrarrGWzzGOrAaZrfuF2lVFc1QY7vFoqle6FoubCX4muVXpqJZ6VJZeUbIdpib5sJd9EGkWU2pq+bX84HpnIaMyzSwGs33UskyAt7m9AlOavajCtdv9yEjTbj+dwoIPfzxLewp/37b1txzgNFUxOvFDIvb9oeYL7gJZWImcZWVG1ikL0JvFyH5GTMtxJ/jcIFSyoboPQ/mXH6T6VgBLheAcqf0A8QTCVwPrmmEn6xKEnGzJa4Q8dTotOOoIvOYHb3qqKsGiMbD5mV/53kFHnQ==</D></RSAKeyValue>";

        private static RSACryptoServiceProvider rsa;

        public HybridEncryptionController()
        {
            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey); // Nạp khóa riêng từ chuỗi XML
        }

        // GET: /HybridEncryption/Index
        public IActionResult Index()
        {
            ViewBag.RsaPublicKey = rsa.ToXmlString(false); // Hiển thị khóa công khai
            ViewBag.RsaPrivateKey = rsa.ToXmlString(true); // Hiển thị khóa riêng
            return View();
        }

        // POST: /HybridEncryption/Encrypt
        [HttpPost]
        public IActionResult Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
            {
                ViewBag.ErrorMessage = "Plaintext cannot be empty.";
                return View("Index");
            }

            // Tạo khóa và IV ngẫu nhiên cho AES
            byte[] aesKey = HybridEncryption.GenerateRandomBytes(32); // Khóa AES 256-bit
            byte[] aesIv = HybridEncryption.GenerateRandomBytes(16);  // IV 128-bit

            // Lưu trữ khóa AES và IV trước khi mã hóa
            string aesKeyBase64 = Convert.ToBase64String(aesKey);
            string aesIvBase64 = Convert.ToBase64String(aesIv);

            // Mã hóa dữ liệu bằng AES
            byte[] encryptedData = HybridEncryption.EncryptWithAES(plaintext, aesKey, aesIv);
            string encryptedDataBase64 = Convert.ToBase64String(encryptedData);

            // Mã hóa khóa AES và IV bằng RSA
            byte[] encryptedKey = rsa.Encrypt(aesKey, false);
            byte[] encryptedIv = rsa.Encrypt(aesIv, false);

            string encryptedKeyBase64 = Convert.ToBase64String(encryptedKey);
            string encryptedIvBase64 = Convert.ToBase64String(encryptedIv);

            // Gửi dữ liệu mã hóa và các giá trị khóa về view
            ViewBag.EncryptedText = encryptedDataBase64;
            ViewBag.EncryptedKey = encryptedKeyBase64;
            ViewBag.EncryptedIV = encryptedIvBase64;
            ViewBag.Plaintext = plaintext;

            // Hiển thị khóa AES và IV trước và sau khi mã hóa
            ViewBag.AesKey = aesKeyBase64;
            ViewBag.AesIV = aesIvBase64;

            return View("Index");
        }

        // POST: /HybridEncryption/Decrypt
        [HttpPost]
        public IActionResult Decrypt(string encryptedText, string encryptedKey, string encryptedIv)
        {
            if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(encryptedKey) || string.IsNullOrEmpty(encryptedIv))
            {
                ViewBag.ErrorMessage = "Ciphertext, key, and IV cannot be empty.";
                return View("Index");
            }

            try
            {
                // Chuyển đổi chuỗi Base64 thành byte[]
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKey);
                byte[] encryptedIvBytes = Convert.FromBase64String(encryptedIv);

                // Giải mã khóa AES và IV bằng RSA
                byte[] aesKey = rsa.Decrypt(encryptedKeyBytes, false);
                byte[] aesIv = rsa.Decrypt(encryptedIvBytes, false);

                // Giải mã dữ liệu bằng AES
                string decryptedText = HybridEncryption.DecryptWithAES(encryptedBytes, aesKey, aesIv);

                // Gửi dữ liệu giải mã về view
                ViewBag.DecryptedText = decryptedText;
                ViewBag.EncryptedText = encryptedText;
                ViewBag.EncryptedKey = encryptedKey;
                ViewBag.EncryptedIV = encryptedIv;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Decryption failed: {ex.Message}";
            }

            return View("Index");
        }
    }

    public static class HybridEncryption
    {
        // Tạo chuỗi byte ngẫu nhiên (cho khóa AES và IV)
        public static byte[] GenerateRandomBytes(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);
                return randomBytes;
            }
        }

        // Mã hóa bằng AES
        public static byte[] EncryptWithAES(string plaintext, byte[] key, byte[] iv)
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

        // Giải mã bằng AES
        public static string DecryptWithAES(byte[] ciphertext, byte[] key, byte[] iv)
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
