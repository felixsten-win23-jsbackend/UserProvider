using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Text.Json;
using Data.Entities;

namespace UserProvider
{
    public class UpdateUser(ILogger<UpdateUser> logger, DataContext context)
    {
        private readonly ILogger<UpdateUser> _logger = logger;
        private readonly DataContext _context = context;

        [Function("UpdateUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateUser")] HttpRequest req)
        {
            string email = req.Query["email"];
            _logger.LogInformation("Received request to update user with email: {Email}", email);
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found", email);
                    return new NotFoundResult();
                }

                var updatedUser = await JsonSerializer.DeserializeAsync<ApplicationUser>(req.Body);
                if (updatedUser == null)
                {
                    _logger.LogWarning("Invalid user data provided in request body");
                    return new BadRequestResult();
                }

                
                if (string.IsNullOrEmpty(updatedUser.FirstName) ||
                    string.IsNullOrEmpty(updatedUser.LastName) ||
                    string.IsNullOrEmpty(updatedUser.Email))
                {
                    _logger.LogWarning("Missing required user data");
                    return new BadRequestObjectResult("Missing required user data");
                }

                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                user.Biography = updatedUser.Biography;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.UserName = updatedUser.Email;
                user.Email = updatedUser.Email;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with email {Email} updated successfully", email);
                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with email {Email}", email);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}