using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserProvider
{
    public class GetUserByEmail
    {
        private readonly ILogger<GetUserByEmail> _logger;
        private readonly DataContext _context;

        public GetUserByEmail(ILogger<GetUserByEmail> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetUserByEmail")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUserByEmail")] HttpRequest req)
        {
            string email = req.Query["email"];
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email is missing in query parameters");
                return new BadRequestObjectResult("Email is required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return new NotFoundResult();
            }

            return new OkObjectResult(user);
        }
    }
}