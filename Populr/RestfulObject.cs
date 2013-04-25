using System;
using RestSharp;

namespace Populr
{
	public abstract class RestfulObject
	{
		internal PopulrAPI _api = null;
		internal RestfulObject _parent = null;
		
		public RestfulObject ()
		{
		}
		
		public RestfulObject (RestfulObject parent)
		{
			_parent = parent;
			_api = _parent.API ();
		}
		
		public virtual PopulrAPI API()
		{
			return _api;
		}
		
		public virtual RestfulObject Parent ()
		{
			return _parent;
		}
		
		public abstract string Path (Method method = Method.GET);
	}
}

