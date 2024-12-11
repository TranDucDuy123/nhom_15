using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
namespace MyAspNetApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private static RSACryptoServiceProvider rsa;

        public AccountController(IConfiguration configuration, ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            // Tạo cặp khóa RSA cho mã hóa lai
            rsa = new RSACryptoServiceProvider(2048);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
          
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                string encryptedPassword = EncryptPassword(password, conn);
                string query = "SELECT * FROM USERS WHERE USERNAME = :username AND MATKHAU = :password";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("username", username));
                    cmd.Parameters.Add(new OracleParameter("password", encryptedPassword));

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            string makh = reader["MAKH"].ToString();
                            string manv = reader["MANV"].ToString();

                            // Kiểm tra trạng thái xác thực trong USER_VERIFICATION
                            string verificationQuery = @"
                        SELECT IS_VERIFIED FROM USER_VERIFICATION 
                        WHERE MAKH = :makh AND MANV = :manv";

                            using (OracleCommand verificationCmd = new OracleCommand(verificationQuery, conn))
                            {
                                verificationCmd.Parameters.Add(new OracleParameter("makh", makh));
                                verificationCmd.Parameters.Add(new OracleParameter("manv", manv));

                                var isVerified = verificationCmd.ExecuteScalar();

                                if (isVerified == null || Convert.ToInt32(isVerified) == 0)
                                {
                                    // Tài khoản chưa xác thực, hiển thị thông báo và chặn đăng nhập
                                    ViewBag.Message = "Tài khoản của bạn chưa được xác thực. Vui lòng kiểm tra email để xác thực tài khoản.";
                                    return View();
                                }
                            }
                            // Lưu thông tin người dùng vào session
                            HttpContext.Session.SetString("Username", reader["USERNAME"].ToString());
                            HttpContext.Session.SetString("FullName", reader["TENDN"].ToString());
                            HttpContext.Session.SetString("Email", reader["EMAIL"].ToString());
                            HttpContext.Session.SetString("Role", reader["VAITRO"].ToString());
                            HttpContext.Session.SetString("Phone", reader["SODT"].ToString());

                            // Chuyển hướng đến trang xác thực khuôn mặt
                            //return RedirectToAction("FaceRecognition");
                            ViewBag.Message = "Chào mừng bạn đến với trang chủ!";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng!";
                        }
                    }
                }
            }

            return View();
        }

        // GET: Account/FaceRecognition
        public IActionResult FaceRecognition()
        {
            return View();
        }

        // POST: Account/LoginWithFaceRecognition
        [HttpPost]
        public IActionResult LoginWithFaceRecognition(string faceData)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                // Chuyển đổi chuỗi base64 của ảnh khuôn mặt thành byte[]
                byte[] currentFaceData = Convert.FromBase64String(faceData.Replace("data:image/jpeg;base64,", ""));

                if (currentFaceData == null || currentFaceData.Length == 0)
                {
                    ViewBag.Message = "Không thể nhận diện khuôn mặt. Vui lòng thử lại.";
                    return View("Login");
                }

                // Truy xuất ảnh khuôn mặt đã lưu từ cơ sở dữ liệu
                string query = "SELECT USERNAME, FACE_DATA FROM USERS";
                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Kiểm tra nếu cột FACE_DATA là NULL
                            if (reader["FACE_DATA"] != DBNull.Value)
                            {
                                byte[] savedFaceData = (byte[])reader["FACE_DATA"];
                                if (CompareFaces(currentFaceData, savedFaceData))
                                {
                                    // Đăng nhập thành công, lưu thông tin người dùng vào session
                                    HttpContext.Session.SetString("Username", reader["USERNAME"].ToString());
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("No face data found for user: " + reader["USERNAME"].ToString());
                            }
                        }
                    }
                }
            }

            ViewBag.Message = "Không nhận diện được khuôn mặt!";
            return View("Login");
        }


        // Hàm so sánh hai ảnh khuôn mặt
        private bool CompareFaces(byte[] currentFace, byte[] savedFace)
        {
            // Bạn có thể sử dụng các thư viện như EmguCV để so sánh khuôn mặt chính xác hơn.
            // Hiện tại so sánh cơ bản giữa hai byte[]
            return currentFace.SequenceEqual(savedFace);
        }

        private string EncryptPassword(string password, OracleConnection conn)
        {
            using (OracleCommand cmd = new OracleCommand("SELECT ENCRYPT_PASSWORD(:p_PASSWORD) FROM dual", conn))
            {
                cmd.Parameters.Add(new OracleParameter("p_PASSWORD", password));
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(string username, string password, string fullname, string email, string phone, string role, string faceData)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                try
                {
                    // Lấy ID nhân viên tiếp theo
                    string nextEmployeeId = "";
                    using (OracleCommand getEmployeeIdCmd = new OracleCommand("SELECT GetNextEmployeeId() FROM dual", conn))
                    {
                        nextEmployeeId = getEmployeeIdCmd.ExecuteScalar()?.ToString();
                    }

                    // Lấy ID khách hàng tiếp theo
                    string nextCustomerId = "";
                    using (OracleCommand getCustomerIdCmd = new OracleCommand("SELECT GetNextCustomerId() FROM dual", conn))
                    {
                        nextCustomerId = getCustomerIdCmd.ExecuteScalar()?.ToString();
                    }

                    // Log email và phone trước khi mã hóa
                    _logger.LogInformation($"Original Email: {email}");
                    _logger.LogInformation($"Original Phone: {phone}");

                    // Mã hóa email và phone bằng cách gọi hàm mã hóa AES từ Oracle
                    string encryptedEmail = EncryptDataInOracle(email, conn);
                    string encryptedPhone = EncryptDataInOracle(phone, conn);

                    // Log kết quả sau khi mã hóa
                    _logger.LogInformation($"Encrypted Email: {encryptedEmail}");
                    _logger.LogInformation($"Encrypted Phone: {encryptedPhone}");

                    // Thêm khách hàng mới vào bảng KHACHHANG (bao gồm email và phone đã mã hóa)
                    string insertCustomerQuery = @"
                INSERT INTO KHACHHANG (MAKH, HOTENKH, DIACHI, SODT, EMAIL)
                VALUES (:nextCustomerId, :fullname, '50 Nguyễn Huệ, HCM', :encryptedPhone, :encryptedEmail)";
                    using (OracleCommand cmd = new OracleCommand(insertCustomerQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("nextCustomerId", nextCustomerId));
                        cmd.Parameters.Add(new OracleParameter("fullname", fullname));
                        cmd.Parameters.Add(new OracleParameter("encryptedPhone", encryptedPhone));
                        cmd.Parameters.Add(new OracleParameter("encryptedEmail", encryptedEmail));

                        cmd.ExecuteNonQuery();
                        _logger.LogInformation($"Customer {fullname} (ID: {nextCustomerId}) inserted successfully.");
                    }

                    //// Log thông tin sau khi giải mã
                    //string decryptedEmail = DecryptDataInOracle(encryptedEmail, conn);
                    //string decryptedPhone = DecryptDataInOracle(encryptedPhone, conn);
                    //_logger.LogInformation($"Decrypted Email: {decryptedEmail}");
                    //_logger.LogInformation($"Decrypted Phone: {decryptedPhone}");

                    // Thêm nhân viên mới vào bảng NHANVIEN
                    string insertEmployeeQuery = @"
                INSERT INTO NHANVIEN (MANV, HOTENNV, CHUCVU, NGAYBDLV, LUONG)
                VALUES (:nextEmployeeId, :fullname, 'Marketing', TO_DATE('2022-01-01', 'YYYY-MM-DD'), 13000)";
                    using (OracleCommand cmd = new OracleCommand(insertEmployeeQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("nextEmployeeId", nextEmployeeId));
                        cmd.Parameters.Add(new OracleParameter("fullname", fullname));

                        cmd.ExecuteNonQuery();
                        _logger.LogInformation($"Employee {fullname} (ID: {nextEmployeeId}) inserted successfully.");
                    }

                    // Mã hóa mật khẩu
                    string encryptedPassword = EncryptPassword(password, conn);
                    _logger.LogInformation($"Password for user {username} encrypted.");

                    // Chuyển đổi chuỗi base64 chứa ảnh khuôn mặt thành byte[]
                    //byte[] faceImageBytes = Convert.FromBase64String(faceData.Replace("data:image/jpeg;base64,", ""));


                    // Nếu đăng ký thành công, chuyển hướng về trang đăng nhập
                    string userInsertQuery = @"INSERT INTO USERS (MAKH, MANV, USERNAME, MATKHAU, TENDN, EMAIL, SODT, VAITRO, LASTPASSWORDCHANGED)
                     VALUES (:nextCustomerId, :nextEmployeeId, :username, :password, :fullname, :encryptedEmail, :encryptedPhone, :role , :lastPasswordChanged)";
                    using (OracleCommand cmd = new OracleCommand(userInsertQuery, conn))
                    {
                        cmd.Parameters.Add(new OracleParameter("nextCustomerId", nextCustomerId));
                        cmd.Parameters.Add(new OracleParameter("nextEmployeeId", nextEmployeeId));
                        cmd.Parameters.Add(new OracleParameter("username", username));
                        cmd.Parameters.Add(new OracleParameter("password", encryptedPassword));
                        cmd.Parameters.Add(new OracleParameter("fullname", fullname));
                        cmd.Parameters.Add(new OracleParameter("encryptedEmail", encryptedEmail));
                        cmd.Parameters.Add(new OracleParameter("encryptedPhone", encryptedPhone));
                        cmd.Parameters.Add(new OracleParameter("role", role));
                        cmd.Parameters.Add(new OracleParameter("lastPasswordChanged", DateTime.Now)); // Lưu thời gian thay đổi mật khẩu
                        // cmd.Parameters.Add(new OracleParameter("faceData", faceImageBytes)); // Lưu dữ liệu ảnh khuôn mặt

                        cmd.ExecuteNonQuery();
                        _logger.LogInformation($"User {username} registered successfully with Employee ID: {nextEmployeeId} and Customer ID: {nextCustomerId}.");
                    }
                    // Tạm thời bỏ mã hóa trên email
                    string emailtxt = email;
                    string token = Guid.NewGuid().ToString();

                    // Lưu token xác thực mà không mã hóa email
                    SaveVerificationToken(nextCustomerId, nextEmployeeId, emailtxt, token);

                    // Gửi email xác thực mà không mã hóa email
                    SendVerificationEmail(emailtxt, token);

                    return RedirectToAction("Login");

                }
                catch (Exception ex)
                {
                    // Log lỗi và hiển thị thông báo lỗi
                    _logger.LogError($"Error occurred during user registration for {username}: {ex.Message}");
                    ViewBag.Message = "Đăng ký thất bại, vui lòng thử lại.";
                }
            }

            return View();
        }
        private void SaveVerificationToken(string makh, string manv, string email, string token)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                // Chèn dữ liệu vào bảng USER_VERIFICATION
                string insertQuery = @"
        INSERT INTO USER_VERIFICATION (MAKH, MANV, EMAIL, VERIFY_TOKEN, TOKEN_CREATED_AT)
        VALUES (:makh, :manv, :email, :token, SYSDATE)";

                using (var insertCmd = new OracleCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.Add(new OracleParameter("makh", makh));
                    insertCmd.Parameters.Add(new OracleParameter("manv", manv));
                    insertCmd.Parameters.Add(new OracleParameter("email", email));
                    insertCmd.Parameters.Add(new OracleParameter("token", token));
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        private void SendVerificationEmail(string email, string token)
        {
            var fromAddress = _configuration["Smtp:Username"];
            var verificationLink = Url.Action("VerifyEmail", "Account", new { token }, Request.Scheme);
            var subject = "Xác thực email của bạn";
            var body = $"<h1>Xác thực tài khoản</h1><p>Vui lòng nhấp vào <a href=\"{verificationLink}\">đây</a> để xác thực tài khoản của bạn.</p>";

            using (var client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"])))
            {
                client.Credentials = new NetworkCredential(fromAddress, _configuration["Smtp:Password"]);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, "Your Name"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                client.Send(mailMessage);
            }
        }

        // GET: Account/VerifyEmail
        public IActionResult VerifyEmail(string token)
        {
            if (ValidateToken(token))
            {
                MarkEmailAsVerified(token);
                return View("Login");
            }
            return View("EmailVerificationFailed");
        }

        private bool ValidateToken(string token)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM USER_VERIFICATION WHERE VERIFY_TOKEN = :token AND IS_VERIFIED = 0";
                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("token", token));

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private void MarkEmailAsVerified(string token)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new OracleConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE USER_VERIFICATION SET IS_VERIFIED = 1 WHERE VERIFY_TOKEN = :token";
                using (var cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("token", token));
                    cmd.ExecuteNonQuery();
                }
            }
        }
        // Hàm để gọi hàm mã hóa AES trong Oracle
        private string EncryptDataInOracle(string data, OracleConnection conn)
        {
            string encryptedData = "";
            using (OracleCommand cmd = new OracleCommand("SELECT ENCRYPT_AES(:data, :key, :iv) FROM dual", conn))
            {
                string key = "12345678901234567890123456789012"; // Key 32 bytes hợp lệ
                string iv = "1234567890123456"; // IV 16 bytes hợp lệ

                cmd.Parameters.Add(new OracleParameter("data", data));
                cmd.Parameters.Add(new OracleParameter("key", key));
                cmd.Parameters.Add(new OracleParameter("iv", iv));

                // Lấy dữ liệu mã hóa dưới dạng byte[] và chuyển đổi thành Base64
                var encryptedBytes = (byte[])cmd.ExecuteScalar();
                encryptedData = Convert.ToBase64String(encryptedBytes); // Chuyển sang chuỗi Base64
            }

            return encryptedData;
        }

        // Hàm để gọi hàm giải mã AES trong Oracle
        private string DecryptDataInOracle(string encryptedData, OracleConnection conn)
        {
            string decryptedData = "";
            using (OracleCommand cmd = new OracleCommand("SELECT DECRYPT_AES(:encryptedData, :key, :iv) FROM dual", conn))
            {
                string key = "12345678901234567890123456789012"; // Key 32 bytes hợp lệ
                string iv = "1234567890123456"; // IV 16 bytes hợp lệ

                // Chuyển đổi chuỗi Base64 thành byte[]
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                cmd.Parameters.Add(new OracleParameter("encryptedData", encryptedBytes));
                cmd.Parameters.Add(new OracleParameter("key", key));
                cmd.Parameters.Add(new OracleParameter("iv", iv));

                decryptedData = cmd.ExecuteScalar()?.ToString();
            }

            return decryptedData;
        }




        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
