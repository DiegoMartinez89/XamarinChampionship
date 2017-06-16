using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace Reto8.Services
{
    public class Productos
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Nombre")]
        public string Nombre { get; set; }

        [JsonProperty(PropertyName = "Descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty(PropertyName = "Inventario")]
        public decimal Inventario { get; set; }

        [JsonProperty(PropertyName = "Precio")]
        public decimal Precio { get; set; }

        [JsonProperty(PropertyName = "Complete")]
        public bool Complete { get; set; }

        public class ProductoItemWrapper : Java.Lang.Object
        {
            public ProductoItemWrapper(Productos item)
            {
                Producto = item;
            }

            public Productos Producto { get; private set; }
        }
    }
}
