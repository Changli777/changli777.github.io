using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace TenshiYamiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly string connectionString = "Server=localhost;Database=Github;Trusted_Connection=True;";

        [HttpPost("click")]
        public IActionResult Click([FromForm] string linkName)
        {
            if (string.IsNullOrEmpty(linkName))
                return BadRequest("Thiếu tên link!");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE link_views SET view_count = view_count + 1 WHERE link_name = @name";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", linkName);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            // Nếu chưa có link trong bảng, thêm mới
                            string insert = "INSERT INTO link_views (link_name, view_count) VALUES (@name, 1)";
                            using (SqlCommand insertCmd = new SqlCommand(insert, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@name", linkName);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return Ok("Đã tăng lượt xem cho " + linkName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi: " + ex.Message);
            }
        }
    }
}
