using Microsoft.AspNetCore.Mvc;
using MyAspNetApp.Models;
namespace MyAspNetApp.Controllers
{
    public class ProfileController : Controller
    {
        // Hiển thị trang Profile
        public IActionResult Index()
        {
            // Lấy thông tin người dùng từ Session
            var username = HttpContext.Session.GetString("Username");
            var fullName = HttpContext.Session.GetString("FullName");
            var email = HttpContext.Session.GetString("Email");
            var role = HttpContext.Session.GetString("Role");
            var phone = HttpContext.Session.GetString("Phone");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account"); // Nếu không có thông tin, chuyển hướng về trang đăng nhập
            }

            var model = new ProfileViewModel
            {
                Username = username,
                FullName = fullName,
                Email = email,
                Role = role,
                Phone = phone
            };

            return View(model);
        }

        // Hiển thị trang thay đổi mật khẩu
        public IActionResult ChangePassword()
        {
            return View();
        }

        // Xử lý thay đổi mật khẩu
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra mật khẩu cũ và cập nhật mật khẩu mới
                // Ví dụ: gọi đến DB để thay đổi mật khẩu

                // Cập nhật mật khẩu (giả sử bạn có phương thức UpdatePassword trong lớp service của mình)
                // UserService.UpdatePassword(username, model.NewPassword);

                // Sau khi cập nhật thành công, có thể thông báo
                ViewBag.Message = "Mật khẩu đã được thay đổi thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
