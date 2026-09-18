using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class CitizensController(CoreDbContext context, ICitizensService iCitizensService ) : ControllerBase {
    [HttpGet({"citizen_id"})]
}