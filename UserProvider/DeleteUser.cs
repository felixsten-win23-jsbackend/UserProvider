using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider
{
    public class DeleteUser(ILogger<DeleteUser> logger, DataContext context)
    {
        private readonly ILogger<DeleteUser> _logger = logger;
        private readonly DataContext _context = context;

        [Function("DeleteUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{email}")] HttpRequest req, string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return new NotFoundResult();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with email {Email}", email);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}