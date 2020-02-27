using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epok.Presentation.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        // GET: api/Suppliers
        [HttpGet]
        public IEnumerable<string> GetAsync()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public string GetAsync(Guid id)
        {
            return "value";
        }

        // POST: api/Suppliers
        [HttpPost]
        public void PostAsync([FromBody] string value)
        {
        }

        // PUT: api/Suppliers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void DeleteAsync(Guid id)
        {
        }
    }
}
