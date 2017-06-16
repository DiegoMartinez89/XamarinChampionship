using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using Reto8.Services;

namespace reto8
{

    [Activity(Icon = "@drawable/ic_launcher", Label = "Producto", Theme = "@style/AppTheme")]
    public class AgregarActivity : Activity
    {
        //Mobile Service Client reference
        private MobileServiceUser user;
        private MobileServiceClient client;
        private string idProducto;
#if OFFLINE_SYNC_ENABLED
        //Mobile Service sync table used to access data
        private IMobileServiceSyncTable<Productos> torneoItemTable;
        const string localDbFilename = "localstore.db";
#else
        private IMobileServiceTable<Productos> torneoItemTable;
#endif
        const string applicationURL = @"http://todolistdiego.azurewebsites.net";
        Button btnAgregar;
        Button btnEliminar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Agregar);
                string token = Intent.GetStringExtra("token");
                string userid = Intent.GetStringExtra("userId");
                idProducto = Intent.GetStringExtra("idProducto");
                user = new MobileServiceUser(userid);
                user.MobileServiceAuthenticationToken = token;
                // Create the Mobile Service Client instance, using the provided
                // Mobile Service URL
                client = new MobileServiceClient(applicationURL);
                client.CurrentUser = user;

#if OFFLINE_SYNC_ENABLED
                await InitLocaIStoreAsync();

                // Get the Mobile Service sync table instance to use
                torneoItemTable = client.GetSyncTable<Productos>();
#else
                torneoItemTable = client.GetTable<Productos>();
#endif
                // Obtener una referencia al botón Siguiente
                btnAgregar = FindViewById<Button>(Resource.Id.btnAddProducto);
                //// Registrar el manejador de evento click del botón Siguiente
                btnAgregar.Click += BtnAgregarClick;

                // Obtener una referencia al botón Siguiente
                btnEliminar = FindViewById<Button>(Resource.Id.btnEliminar);
                //// Registrar el manejador de evento click del botón Siguiente
                btnEliminar.Click += BtnEliminarClick;

                if (!string.IsNullOrWhiteSpace(idProducto))
                {
                    EditText nombre = FindViewById<EditText>(Resource.Id.editText1);
                    EditText descripcion = FindViewById<EditText>(Resource.Id.editText2);
                    EditText inventario = FindViewById<EditText>(Resource.Id.editText3);
                    EditText precio = FindViewById<EditText>(Resource.Id.editText4);

                    btnAgregar.Text = "Modificar Producto";
                    btnEliminar.Visibility = ViewStates.Visible;

                    torneoItemTable.Where(x => x.Id == idProducto);
                }
                else
                {
                    btnAgregar.Text = "Agregar Producto";
                    btnEliminar.Visibility = ViewStates.Gone;
                }

            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error: " + e.Message, ToastLength.Long);
            }
        }

        private async void BtnAgregarClick(object sender, EventArgs e)
        {
            try
            {
                EditText nombre = FindViewById<EditText>(Resource.Id.editText1);
                EditText descripcion = FindViewById<EditText>(Resource.Id.editText2);
                EditText inventario = FindViewById<EditText>(Resource.Id.editText3);
                EditText precio = FindViewById<EditText>(Resource.Id.editText4);

                if (string.IsNullOrWhiteSpace(idProducto))
                {
                    await torneoItemTable.InsertAsync(new Productos
                    {
                        Nombre = nombre.Text,
                        Descripcion = descripcion.Text,
                        Inventario = Convert.ToDecimal(inventario.Text),
                        Precio = Convert.ToDecimal(precio.Text)
                    });
                }
                else
                {
                    await torneoItemTable.UpdateAsync(new Productos
                    {
                        Id = idProducto,
                        Nombre = nombre.Text,
                        Descripcion = descripcion.Text,
                        Inventario = Convert.ToDecimal(inventario.Text),
                        Precio = Convert.ToDecimal(precio.Text)
                    });
                }
            }
            catch(Exception ex) {
                CreateAndShowDialog(ex, "Error al guardar");
            }
        }

        private async void BtnEliminarClick(object sender, EventArgs e)
        {
            try
            {
                EditText nombre = FindViewById<EditText>(Resource.Id.editText1);
                EditText descripcion = FindViewById<EditText>(Resource.Id.editText2);
                EditText inventario = FindViewById<EditText>(Resource.Id.editText3);
                EditText precio = FindViewById<EditText>(Resource.Id.editText4);

                await torneoItemTable.DeleteAsync(new Productos
                {
                    Id = idProducto,
                    Nombre = nombre.Text,
                    Descripcion = descripcion.Text,
                    Inventario = Convert.ToDecimal(inventario.Text),
                    Precio = Convert.ToDecimal(precio.Text)
                });
            }
            catch (Exception ex)
            {
                CreateAndShowDialog(ex, "Error al eliminar");
            }
        }

        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
#if OFFLINE_SYNC_ENABLED
        private async Task InitLocaIStoreAsync()
        {
            string path = Path.Combine(System.Environment.
            GetFolderPath(System.Environment.SpecialFolder.Personal), localDbFilename);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Productos>();
            await client.SyncContext.InitializeAsync(store);
        }

        private async Task SyncAsync(bool pullData = false)
        {
            try
            {
                await client.SyncContext.PushAsync();
                if (pullData)
                    await torneoItemTable.PullAsync("allTorneoItems", torneoItemTable.CreateQuery().Where(
                    item => item.Email == emailParticipante));
            }
            catch (Exception e)
            {
                string error = e.Message;
                Toast.MakeText(this, "Error: " + error, ToastLength.Long);
            }
        }
#endif
    }
}