
using System;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using reto8;
using Reto4.Services;
using Android.Content;
using Reto8.Services;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

namespace reto8
{
    [Activity(Icon = "@drawable/ic_launcher", Label = "Productos", Theme = "@style/AppTheme")]
    public class ProductosActivity : Activity
    {
        //Mobile Service Client reference
        private MobileServiceUser user;
        private MobileServiceClient client;
        // Adapter to map the items list to the view
        private ProductoAdapter adapter;
#if OFFLINE_SYNC_ENABLED
        //Mobile Service sync table used to access data
        private IMobileServiceSyncTable<Productos> torneoItemTable;
        const string localDbFilename = "localstore.db";
#else
        private IMobileServiceTable<Productos> torneoItemTable;
#endif
        const string applicationURL = @"http://todolistdiego.azurewebsites.net";
        Button agregarProductos;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Productos);
                string token = Intent.GetStringExtra("token");
                string userid = Intent.GetStringExtra("userId");
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
                agregarProductos = FindViewById<Button>(Resource.Id.button1);
                //// Registrar el manejador de evento click del botón Siguiente
                agregarProductos.Click += AgregarProductos_Click;

                OnRefreshItemsSelected();

            }
            catch (Exception e)
            {
                Toast.MakeText(this, "Error: " + e.Message, ToastLength.Long);
            }
        }

        private void AgregarProductos_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AgregarActivity));
            intent.PutExtra("token", user.MobileServiceAuthenticationToken);
            intent.PutExtra("userId", user.UserId);
            StartActivityForResult(intent, 1);
        }

        // Called when the refresh menu option is selected.
        private async void OnRefreshItemsSelected()
        {
#if OFFLINE_SYNC_ENABLED
			// Get changes from the mobile app backend.
            await SyncAsync(pullData: true);
#endif
            // refresh view using local store.
            await RefreshItemsFromTableAsync();
        }

        //Refresh the list with the items in the local store.
        private async Task RefreshItemsFromTableAsync()
        {
            try
            {
                // Get the items that weren't marked as completed and add them in the adapter
                var list = await torneoItemTable.Where(item => item.Complete == false).ToListAsync();

                adapter.Clear();

                foreach (Productos current in list)
                    adapter.Add(current);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
            }
        }

        public async Task CheckItem(Productos item)
        {
            if (client == null)
            {
                return;
            }

            // Set the item as completed and update it in the table
            item.Complete = true;
            try
            {
                // Update the new item in the local store.
                await torneoItemTable.UpdateAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
				await SyncAsync();
#endif

                if (item.Complete)
                    adapter.Remove(item);

            }
            catch (Exception e)
            {
                CreateAndShowDialog(e, "Error");
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
    }
}