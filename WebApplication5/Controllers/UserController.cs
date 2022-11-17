using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace WebApplication5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]

        public async Task<ActionResult<List<UserProfile>>> GetAllUserProfiles()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<UserProfile> users = await SelectAllUsers(connection);
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfile>> GetUser(int userId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var user = await connection.QueryFirstAsync<UserProfile>("select * from userprofile where id=@Id", new {Id=userId});
            return Ok(user);
        }
        private static async Task<IEnumerable<UserProfile>> SelectAllUsers(SqlConnection connection)
        {
            return await connection.QueryAsync<UserProfile>("select * from userprofile");
        }
        [HttpPost]
        public async Task<ActionResult<List<UserProfile>>> CreateUser(UserProfile user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into userprofile(firstName,lastName,email,passcode) values(@FirstName,@LastName,@Email,@Passcode)",user);
            return Ok(await SelectAllUsers(connection));
            
        }
        [HttpPut]
        public async Task<ActionResult<List<UserProfile>>> UpdateUser(UserProfile user)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update userprofile set firstName=@FirstName,lastName=@LastName,email=@email,passcode=@passcode where id=@Id", user);
            return Ok(await SelectAllUsers(connection));

        }
        [HttpDelete("{userId}")]
        public async Task<ActionResult<List<UserProfile>>> DeleteUser(int userId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from userprofile where id=@Id", new {Id=userId});
            return Ok(await SelectAllUsers(connection));

        }
    }
}
