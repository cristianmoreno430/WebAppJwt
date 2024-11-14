using AuthData.Dtos;
using AuthData.Interfaces;

namespace AuthData
{
    public class TaskInformation : ITaskInformation
    {
        private List<TaskDto> _tareas = new List<TaskDto>();

        public TaskInformation()
        {
            _tareas = new List<TaskDto>();
        }

        public void Agregar(TaskDto tarea)
        {
            _tareas.Add(tarea);
        }

        public void Actualizar(TaskDto tarea)
        {
            var tareaExistente = _tareas.FirstOrDefault(t => t.Nombre == tarea.Nombre);
            if (tareaExistente != null)
            {
                tareaExistente.Nombre = tarea.Nombre;
            }
        }

        public TaskDto? ObtenerPorId(int id)
        {
            return _tareas.FirstOrDefault(t => t.Id == id);
        }

        public List<TaskDto> ObtenerTodas()
        {
            return _tareas;
        }

        public void Eliminar(TaskDto tarea)
        {
            _tareas.Remove(tarea);
        }
    }
}
