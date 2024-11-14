using AuthData.Dtos;

namespace AuhtService.Interfaces
{
    public interface ITasksService
    {
        string GetStub(int id);
        string GetFake(int id);
        TaskDto? ObtenerPorId(int id);
        bool ActualizarTarea(int id, string nombre, bool completada);
        TaskDto CrearTarea(string nombre);
        bool EliminarTarea(int id);
        List<TaskDto> ObtenerTareas();
    }
}