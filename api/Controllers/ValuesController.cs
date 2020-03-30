using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        [AllowAnonymous]
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
        public async Task<IActionResult> Post(string value)
        {
          var valueToAdd = new Value() {Name = value};
          context.Values.Add(valueToAdd);
          await context.SaveChangesAsync();
          return Ok();
        }

        // * PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, string value)
        {
          // TODO: 
        }

        // * DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
          var valueToDelete = await context.Values.SingleOrDefaultAsync(value => value.Id == id);
          if (valueToDelete == null) {
            return NotFound();
          }
          
          context.Values.Remove(valueToDelete);
          await context.SaveChangesAsync();
          return NoContent();
        }
    }
}