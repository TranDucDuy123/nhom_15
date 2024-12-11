using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using MyAspNetApp.Models;

namespace MyAspNetApp.Controllers
{
    public class XeController : Controller
    {
        private readonly IConfiguration _configuration;

        public XeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string message = "";
            List<Xe> xeList = new List<Xe>();

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    conn.Open();
                    message = "Kết nối thành công với cơ sở dữ liệu Oracle!";

                    // Truy vấn dữ liệu từ bảng XE
                    string query = "SELECT MAXE, MOHINH, NHASANXUAT, NAMSANXUAT, GIA, SOXECOTRONGKHO FROM XE";
                    OracleCommand cmd = new OracleCommand(query, conn);

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Lưu dữ liệu từ mỗi bản ghi vào danh sách xe
                            xeList.Add(new Xe
                            {       
                                Maxe = reader.GetString(0),
                                Mohinh = reader.GetString(1),
                                Nhasanxuat = reader.GetString(2),
                                Namsanxuat = reader.GetDateTime(3),
                                Gia = reader.GetDecimal(4),
                                Soxecotrongkho = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}";
            }

            // Gửi thông báo và danh sách xe tới View
            ViewBag.Message = message;
            return View(xeList); // Truyền danh sách xe đến View
        }
    }
}
