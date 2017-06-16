
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

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

namespace reto8
{
    [Activity(MainLauncher =true, Label = "Tienda", Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        // Client reference.
        private MobileServiceClient client;

        // URL of the mobile app backend.
        const string applicationURL = @"http://todolistdiego.azurewebsites.net";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Login);

            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            var ImageView1 = FindViewById<ImageView>(Resource.Id.imageView1);
            ImageView1.SetImageResource(Resource.Drawable.tiendav);
            var ButtonLogin = FindViewById<Button>(Resource.Id.btnIniciarSesion);
            ButtonLogin.Click += (s, ev) =>
            {
                LoginUser();
            };
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
        }// Define a authenticated user.
        private MobileServiceUser user;
        private async Task<bool> Authenticate()
        {
            var success = false;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                user = await client.LoginAsync(this, MobileServiceAuthenticationProvider.Facebook);
                //CreateAndShowDialog(string.Format("you are now logged in - {0}",
                //    user.UserId), "Logged in!");

                //ServiceHelper serviceHelper = new ServiceHelper();
                //String reto = "RetoN" + " + " + "6a47e" + " + https://github.com/DiegoMartinez89/XamarinChampionship/RetoN/";
                //String email = "d_martinezr89@hotmail.com";
                //String AndroidId = Android.Provider.Settings.Secure.GetString(ContentResolver, Android.Provider.Settings.Secure.AndroidId);
                //Toast.MakeText(this, "Enviando tu registro", ToastLength.Short).Show();
                //await serviceHelper.InsertarEntidad(email, reto, AndroidId);
                //Toast.MakeText(this, "Gracias por registrarte", ToastLength.Long).Show();

                success = true;

                var intent = new Intent(this, typeof(ProductosActivity));
                intent.PutExtra("token", user.MobileServiceAuthenticationToken);
                intent.PutExtra("userId", user.UserId); 
                StartActivityForResult(intent, 1);
            }
            catch (Exception ex)
            {
                CreateAndShowDialog(ex, "Authentication failed");
            }
            return success;
        }

        [Java.Interop.Export()]
        public async void LoginUser()
        {
            // Load data only after authentication succeeds.
            if (await Authenticate())
            {
                //Hide the button after authentication succeeds.
                FindViewById<Button>(Resource.Id.btnIniciarSesion).Visibility = ViewStates.Gone;
            }
        }
    }
}
