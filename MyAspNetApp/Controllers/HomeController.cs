using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace MyAspNetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (HttpContext.Session.GetString("Username") == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang Login
                return RedirectToAction("Login", "Account");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string message = "";

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    message = "Kết nối thành công với cơ sở dữ liệu Oracle!";
                }
            }
            catch (Exception ex)
            {
                message = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}";
            }

            ViewBag.Message = message; // Gửi thông điệp này đến View
            return View();
        }
    }
}
