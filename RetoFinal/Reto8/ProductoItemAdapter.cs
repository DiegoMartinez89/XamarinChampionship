using System;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Reto4.Services;
using Reto8.Services;
using static Reto8.Services.Productos;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;

namespace reto8
{
	public class ProductoAdapter : BaseAdapter<Productos>
	{
		Activity activity;
		int layoutResourceId;
		List<Productos> items = new List<Productos> ();
        private MobileServiceUser user;

        public ProductoAdapter(Activity activity, int layoutResourceId, MobileServiceUser user)
		{
			this.activity = activity;
			this.layoutResourceId = layoutResourceId;
            this.user = user;
		}

		//Returns the view for a specific item on the list
		public override View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			var row = convertView;
			var currentItem = this [position];
            TextView txtNombre;
            TextView txtDescripcion;
            Button btnEditar;

            txtNombre = row.FindViewById<TextView>(Resource.Id.txtNombre);
            txtDescripcion = row.FindViewById<TextView>(Resource.Id.txtDescripcion);
            if (row == null) {
				var inflater = activity.LayoutInflater;
				row = inflater.Inflate (layoutResourceId, parent, false);

                btnEditar = row.FindViewById<Button>(Resource.Id.button1);

                btnEditar.Click += (sender, e) => {
					var cbSender = sender as Button;
					if (cbSender != null && cbSender.Tag is ProductoItemWrapper)
                    {                        
                        var intent = new Intent(parent.Context, typeof(AgregarActivity));
                        intent.PutExtra("token", user.MobileServiceAuthenticationToken);
                        intent.PutExtra("userId", user.UserId);
                        intent.PutExtra("idProducto", currentItem.Id);
                        activity.StartActivityForResult(intent, 1);                        
					}
				};
			}
            else
                btnEditar = row.FindViewById<Button>(Resource.Id.button1);

            txtNombre.Text = currentItem.Nombre;
            txtDescripcion.Text = currentItem.Descripcion;
			btnEditar.Tag = new ProductoItemWrapper (currentItem);

			return row;
		}

		public void Add (Productos item)
		{
			items.Add (item);
			NotifyDataSetChanged ();
		}

		public void Clear ()
		{
			items.Clear ();
			NotifyDataSetChanged ();
		}

		public void Remove (Productos item)
		{
			items.Remove (item);
			NotifyDataSetChanged ();
		}

		#region implemented abstract members of BaseAdapter

		public override long GetItemId (int position)
		{
			return position;
		}

		public override int Count {
			get {
				return items.Count;
			}
		}

		public override Productos this [int position] {
			get {
				return items [position];
			}
		}

		#endregion
	}
}

