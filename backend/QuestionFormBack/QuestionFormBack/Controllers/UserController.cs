using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using QuestionFormBack.Models;

namespace QuestionFormBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public UserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // GET: api/user
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                SELECT Id, Nickname, Email, Password
                FROM Users
            ";

            var users = new List<User>();
            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    using (var myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            var user = new User
                            {
                                Id = myReader.GetInt32(0),
                                Nickname = myReader.GetString(1),
                                Email = myReader.GetString(2),
                                Password = myReader.GetString(3)
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return new JsonResult(users);
        }

        // POST: api/user
        [HttpPost]
        public JsonResult Post(User user)
        {
            string query = @"
                INSERT INTO Users (Nickname, Email, Password) 
                VALUES (@Nickname, @Email, @Password)
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");

            try
            {
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Nickname", user.Nickname);
                        myCommand.Parameters.AddWithValue("@Email", user.Email);
                        myCommand.Parameters.AddWithValue("@Password", user.Password);
                        myCommand.ExecuteNonQuery();
                    }
                }

                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                return new JsonResult($"Error: {ex.Message}");
            }
        }

        // PUT: api/user
        [HttpPut]
        public JsonResult Put(User user)
        {
            string query = @"
                UPDATE Users
                SET Nickname = @Nickname,
                    Email = @Email,
                    Password = @Password
                WHERE Id = @Id
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", user.Id);
                    myCommand.Parameters.AddWithValue("@Nickname", user.Nickname);
                    myCommand.Parameters.AddWithValue("@Email", user.Email);
                    myCommand.Parameters.AddWithValue("@Password", user.Password);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();

                }
            }

            return new JsonResult("Updated Successfully");
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                DELETE FROM Users
                WHERE Id = @Id
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            NpgsqlDataReader myReader;
            using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    myCon.Close();

                }
            }

            return new JsonResult("Deleted Successfully");
        }
    }
}