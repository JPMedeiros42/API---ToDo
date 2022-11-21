using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuTodo.Controllers
{
    [Route("v1")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        [Route("todos")]
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var todos = await context
                .Todos
                .AsNoTracking()
                .ToListAsync();

            return Ok(todos);
        }

        [Route("todos/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todos = await context
                .Todos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return todos == null
                ? NotFound()
                : Ok(todos);
        }

        [HttpPost("todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created($"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("todos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
                return NotFound();
            try
            {
                todo.Title = model.Title;

                context.Todos.Update(todo);
                await context.SaveChangesAsync();

                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {
            var todo = await context
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id);

            if (todo == null)
                return NotFound();
            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


    }
}
