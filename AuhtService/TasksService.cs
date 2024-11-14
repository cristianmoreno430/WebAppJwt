using AuhtService.Interfaces;
using AuthData.Dtos;
using AuthData.Interfaces;

namespace AuhtService
{
    public class TasksService : ITasksService
    {
        private readonly ITaskInformation _taskInformation;
        private readonly Dictionary<int, string> _taskData = new Dictionary<int, string>(); //this is a Fake
                
        public TasksService(ITaskInformation taskInformation)
        {
            _taskInformation = taskInformation;
            // Inicializa con algunos datos de ejemplo el Fake
            _taskData[0] = "Leer";
            _taskData[1] = "Estudiar";
            _taskData[2] = "Lavar";
        }

        public string GetStub(int id)
        {

            return id == 0 ? "Leer" : "Estudiar";

        }

        public string GetFake(int id)
        {
            if (_taskData.TryGetValue(id, out var Nombre))
            {
                return Nombre;
            }

            return "Unknown";
        }

        public TaskDto CrearTarea(string nombre)
        {
            List<TaskDto> taskDtos = ObtenerTareas();
            int id = taskDtos == null ? 1 : taskDtos.Count + 1;
            TaskDto tarea = new TaskDto { Id = id, Nombre = nombre, Completada = false };
            _taskInformation.Agregar(tarea);
            return tarea;
        }

        public TaskDto? ObtenerPorId(int id)
        {
            return _taskInformation.ObtenerPorId(id);
        }

        public List<TaskDto> ObtenerTareas()
        {
            return _taskInformation.ObtenerTodas();
        }

        public bool ActualizarTarea(int id, string nombre, bool completada)
        {
            TaskDto? taskDtos = ObtenerPorId(id);
            if (taskDtos == null) return false;
            taskDtos.Nombre = nombre;
            taskDtos.Completada = completada;
            _taskInformation.Actualizar(taskDtos);
            return true;
        }

        public bool EliminarTarea(int id)
        {
            TaskDto? taskDtos = ObtenerPorId(id);           
            if (taskDtos == null) return false;
            _taskInformation.Eliminar(taskDtos);
            return true;
        }
    }
}
