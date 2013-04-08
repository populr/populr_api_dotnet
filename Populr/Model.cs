using System;
using System.Reflection;

namespace Populr
{
	public class Model
	{
		public string _id { get; set; }
		internal PopulrAPI _api = null;

		public Model ()
		{
		}

		protected void copyFrom(Model other) {
			PropertyInfo[] properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			
			foreach (PropertyInfo p in properties)
			{
				// If not writable then cannot null it; if not readable then cannot check it's value
				if (!p.CanWrite || !p.CanRead) { continue; }
				
				MethodInfo mget = p.GetGetMethod(false);
				MethodInfo mset = p.GetSetMethod(false);
				
				// Get and set methods have to be public
				if (mget == null) { continue; }
				if (mset == null) { continue; }
				
				p.SetValue(this, p.GetValue(other, null), null);
			}
		}
	}
}

