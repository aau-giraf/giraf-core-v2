using core.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace core.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitizensController : ControllerBase
    {
        private readonly CoreDbContext _context;

        public CitizensController(CoreDbContext context)
        {
            _context = context;
        }

        /* CRUD OPERATIONS */

    }
}
