using AuhtService.Interfaces;
using AuthData.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppJwt.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITasksService tasksService, ILogger<TaskController> logger)
        {
            _tasksService = tasksService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una tarea por su ID.
        /// </summary>
        /// <param name="id">El ID de la tarea a obtener.</param>
        /// <returns>La tarea encontrada o un mensaje de error si no se encuentra.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<string> GetTaskById(int id)
        {
            string task = _tasksService.GetFake(id);
            if (task == null)
            {
                _logger.LogWarning($"Tarea con ID {id} no encontrada.");
                return NotFound(new { Message = $"Tarea con ID {id} no encontrada." });
            }
            return Ok(task);
        }

        /// <summary>
        /// Crea una nueva tarea.
        /// </summary>
        /// <param name="taskDto">El TaskDto de la tarea a crear.</param>
        /// <returns>La tarea creada o un mensaje de error si ocurre un fallo.</returns>
        [HttpPost]
        [Authorize] // Protegido por JWT
        public ActionResult<TaskDto> CreateTask([FromBody] TaskDto taskDto)
        {
            if (string.IsNullOrEmpty(taskDto.Nombre))
            {
                _logger.LogWarning("El nombre de la tarea no puede estar vacío.");
                return BadRequest(new { Message = "El nombre de la tarea es requerido." });
            }

            var task = _tasksService.CrearTarea(taskDto.Nombre);
            return Ok(task);
        }

        /// <summary>
        /// Actualiza una tarea existente.
        /// </summary>
        /// <param name="id">El ID de la tarea a actualizar.</param>
        /// <param name="nombre">El nuevo nombre de la tarea.</param>
        /// <returns>True si la actualización fue exitosa, false si falló.</returns>
        [HttpPut]
        [Authorize] // Protegido por JWT
        public ActionResult<string> UpdateTask(int id, [FromBody] string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
            {
                _logger.LogWarning("El nombre de la tarea no puede estar vacío.");
                return BadRequest(new { Message = "El nombre de la tarea es requerido." });
            }

            var taskUpdated = _tasksService.ActualizarTarea(id, nombre, true);
            if (!taskUpdated)
            {
                _logger.LogWarning($"No se pudo actualizar la tarea con ID {id}.");
                return NotFound(new { Message = $"No se pudo actualizar la tarea con ID {id}." });
            }

            return Ok(new { Message = $"Tarea con ID {id} actualizada exitosamente." });
        }
    }
}
