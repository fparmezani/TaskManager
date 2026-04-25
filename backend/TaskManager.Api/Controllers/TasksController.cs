using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extensions;
using TaskManager.Application.Tasks;

namespace TaskManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetAllAsync(User.GetUserId(), cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _taskService.GetByIdAsync(id, User.GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var created = await _taskService.CreateAsync(User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _taskService.UpdateAsync(id, User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { message = result.Error });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, ChangeTaskStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _taskService.ChangeStatusAsync(id, User.GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { message = result.Error });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}
