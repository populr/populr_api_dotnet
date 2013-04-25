using System;
using System.Reflection;
using RestSharp;

namespace Populr
{
	public class RestfulModel : RestfulObject
	{
		public string _id { get; set; }

		public RestfulModel () {}
		public RestfulModel (RestfulObject parent) : base(parent) {}

		public override string Path (Method method = Method.GET)
		{
			string path = (_parent != null) ? _parent.Path() + "/" : "/";
			if (_id != null)
				path += _id;
			return path;
		}

		public virtual object APIRepresentation ()
		{
			return this;
		}

		public virtual object Save ()
		{
			MethodInfo executeRequest = typeof(PopulrAPI).GetMethod ("executeRequest", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo executeTypedRequest = executeRequest.MakeGenericMethod (this.GetType ());
			RestfulModel saved = null;

			if (_id != null)
				saved = (RestfulModel)executeTypedRequest.Invoke (_api, new Object[] {
					this.Path (Method.PUT),
					Method.PUT,
					APIRepresentation ()
				});
			else
				saved = (RestfulModel)executeTypedRequest.Invoke (_api, new Object[] {
					this.Path (Method.POST),
					Method.POST,
					APIRepresentation ()
				});

			if (saved != null) {
				this.CopyFrom (saved);
				return this;
			}
			return null;
		}

		protected void CopyFrom(RestfulModel other) {
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

