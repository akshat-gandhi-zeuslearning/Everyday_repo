using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSV.Models;

namespace CSV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly MySqlConnection _connection;

        public InformationController(MySqlConnection connection)
        {
            _connection = connection;
        }

        // GET: api/information
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Information>>> GetInformations()
        {
            var informations = new List<Information>();

            await _connection.OpenAsync();
            using var command = new MySqlCommand("SELECT Id, Name, Email, Contact FROM information", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                informations.Add(new Information
                {
                    id = reader.GetInt32(0),
                    name = reader.GetString(1),
                    email = reader.GetString(2),
                    contact = reader.GetString(3)
                });
            }

            await _connection.CloseAsync();
            return informations;
        }

        // GET: api/information/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Information>> GetInformation(int id)
        {
            Information information = null;

            await _connection.OpenAsync();
            using var command = new MySqlCommand("SELECT Id, Name, Email, Contact FROM information WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                information = new Information
                {
                    id = reader.GetInt32(0),
                    name = reader.GetString(1),
                    email = reader.GetString(2),
                    contact = reader.GetString(3)
                };
            }

            await _connection.CloseAsync();

            if (information == null)
            {
                return NotFound();
            }

            return information;
        }

        // POST: api/information
        [HttpPost]
        public async Task<ActionResult<Information>> PostInformation(Information information)
        {
            await _connection.OpenAsync();
            using var command = new MySqlCommand("INSERT INTO information (Name, Email, Contact) VALUES (@Name, @Email, @Contact); SELECT LAST_INSERT_ID();", _connection);
            command.Parameters.AddWithValue("@Name", information.name);
            command.Parameters.AddWithValue("@Email", information.email);
            command.Parameters.AddWithValue("@Contact", information.contact);
            information.id = (int)(ulong)await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return CreatedAtAction(nameof(GetInformation), new { id = information.id }, information);
        }

        // PUT: api/information/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInformation(int id, Information information)
        {
            if (id != information.id)
            {
                return BadRequest();
            }

            await _connection.OpenAsync();
            using var command = new MySqlCommand("UPDATE information SET Name = @Name, Email = @Email, Contact = @Contact WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Name", information.name);
            command.Parameters.AddWithValue("@Email", information.email);
            command.Parameters.AddWithValue("@Contact", information.contact);
            command.Parameters.AddWithValue("@Id", id);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/information/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInformation(int id)
        {
            await _connection.OpenAsync();
            using var command = new MySqlCommand("DELETE FROM information WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);
            var rowsAffected = await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();

            if (rowsAffected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
