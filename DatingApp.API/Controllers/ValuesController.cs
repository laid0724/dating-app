using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize] 
    // ? this tells .NET that every calls made to this controller must be an authorized request, i.e., logged in,
    // ? and is handled through authentication middleware
    // ? all methods below are protected and returns 401 Unauthorized unless logged-in, unless they have the [AllowAnonymous] attribute.
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly DataContext context;

        // ? injects db context
        public ValuesController(DataContext context)
        {
            this.context = context;
        }

        // * GET api/values
        [HttpGet]
        // not async:
        // public IActionResult GetValues()
        // {
        //     var values = context.Values.ToList();
        //     return Ok(values);
        // }
        // async version:
        public async Task<IActionResult> GetValues()
        // ? Task<T> represents an async operation that can return a value
        // ? IActionResult = http responses, e.g, 200, 404, etc.
        {
            var values = await context.Values.ToListAsync();
            return Ok(values);
        }

        // * GET api/values/5
        [AllowAnonymous] // ? this tells .NET that this specific endpoint doesn't need authorization
        [HttpGet("{id}")]
        // not async:
        // public IActionResult GetValue(int id)
        // {
        //     var value = context.Values
        //       .FirstOrDefault(v => v.Id == id);

        //     if (value != null)
        //     {
        //         return Ok(value);
        //     }
        //     else
        //     {
        //         return NoContent();
        //     }
        // }
        // async version:
        public async Task<IActionResult> GetValue(int id)
        {
            var value = await context.Values.FirstOrDefaultAsync(v => v.Id == id);
            return Ok(value);
        }

        // * POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // * PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // * DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}