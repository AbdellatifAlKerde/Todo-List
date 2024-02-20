using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo_List.Models;

namespace Todo_List.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TodoDBContext _dbContext;

        public TasksController(TodoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Tasks
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasks()
        {
            if (_dbContext.Tasks == null) 
            { 
                return NotFound();
            }
            return await _dbContext.Tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]

        public async Task<ActionResult<Models.Task>> GetTask([FromRoute]int id)
        {
            if (_dbContext.Tasks == null)
            {
                return NotFound();
            }
            var task = await _dbContext.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        //GET: api/Tasks/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetTasksByUser([FromRoute]int userId)
        {
            var tasks = await _dbContext.Tasks
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TaskId)
                .ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound();
            }

            return Ok(tasks);
        }

        // POST: api/Tasks
        [HttpPost]

        public async Task<ActionResult<Models.Task>> PostTask([FromBody]Models.Task task)
        {
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
        }


        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTaskTitle(int id, [FromBody] UpdateTaskTitleDto editDto)
        {
            if (editDto == null || string.IsNullOrEmpty(editDto.Title))
                return BadRequest("Invalid request data");

            var task = await _dbContext.Tasks.FindAsync(id);
            if (task == null)
                return NotFound("Task not found.");

            task.Title = editDto.Title;
            await _dbContext.SaveChangesAsync();

            return Ok(task);
        }

        // PUT: api/Tasks/Status/5
        [HttpPut("Status/{id}")]
        public async Task<IActionResult> EditTaskStatus([FromRoute] int id, [FromBody] UpdateTaskStatusDto statusDto)
        {
            try
            {
                var task = await _dbContext.Tasks.FindAsync(id);
                if (task == null)
                    return NotFound("Task not found.");

                task.Status = statusDto.Status;

                await _dbContext.SaveChangesAsync();

                return Ok(task);
            }catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An Error occured while updating the task.");
            }
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _dbContext.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
