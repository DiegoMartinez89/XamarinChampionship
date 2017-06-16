using Microsoft.WindowsAzure.MobileServices;
using reto8;
using System.Threading.Tasks;

namespace Reto4.Services
{
    public class ServiceHelper
    {
        MobileServiceClient clienteServicio = new MobileServiceClient(@"http://todolistdiego.azurewebsites.net");

        private IMobileServiceTable<ToDoItem> _ToDoItemTable;

        public async Task<System.Collections.Generic.List<ToDoItem>> BuscarRegistrosById(string id)
        {
            _ToDoItemTable = clienteServicio.GetTable<ToDoItem>();
            System.Collections.Generic.List<ToDoItem> items = await _ToDoItemTable.Where(
                ToDoItem => ToDoItem.Id == id).ToListAsync();
            return items;
        }

        public async Task<System.Collections.Generic.List<ToDoItem>> BuscarRegistros(string clave)
        {
            _ToDoItemTable = clienteServicio.GetTable<ToDoItem>();
            System.Collections.Generic.List<ToDoItem> items = await _ToDoItemTable.Where(
                ToDoItem => ToDoItem.Text.ToLower().Contains(clave)).ToListAsync();
            return items;
        }

        public async Task InsertarProducto(string nombre, string descripcion, double inventario, double precio)
        {
            _ToDoItemTable = clienteServicio.GetTable<ToDoItem>();

            await _ToDoItemTable.InsertAsync(new ToDoItem
            {
                Text = nombre
            });
        }

        public async Task ActualizarProducto(string id, string nombre, string descripcion, double inventario, double precio)
        {
            _ToDoItemTable = clienteServicio.GetTable<ToDoItem>();

            await _ToDoItemTable.UpdateAsync(new ToDoItem
            {
                Id = id,
                Text = nombre
            });
        }
    }
}
