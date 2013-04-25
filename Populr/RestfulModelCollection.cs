using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace Populr
{
	public class RestfulModelCollection<TModel> : RestfulObject where TModel: RestfulModel, new()
	{
		internal string _collectionName;

		public RestfulModelCollection (RestfulObject parent, string collectionName = null) : base(parent) 
		{
			this._collectionName = (collectionName != null) ? collectionName : typeof(TModel).Name.ToLower() + "s";
		}

		public override string Path(Method method = Method.GET) {
			if (_parent != null)
				return _parent.Path() + "/" +  _collectionName;
			else
				return _collectionName;
		}

		public IEnumerable<TModel> Each() {
			int offset = 0;
			bool finished = false;
			while (!finished) {
				List<TModel> items = _api.executeIndexRequest<TModel>(Path(), offset, 50);
				if (items.Count == 0)
					break;
				foreach (TModel item in items)
					yield return item;
				offset += items.Count;
			}
		}

		public TModel First() {
			return _api.executeIndexRequest<TModel>(Path()).First();
		}

		public List<TModel> All() {
			return this.Range(0, int.MaxValue);
		}

		public List<TModel> Range(int offset = 0, int count = 0) {
			List<TModel> accumulated = new List<TModel>();
			bool finished = false;
			int chunkSize = 50;

			while (!finished && accumulated.Count < count) {
				List<TModel> results = _api.executeIndexRequest<TModel>(Path(), offset + accumulated.Count, chunkSize);
				accumulated.AddRange(results);

				// we're done if we have more than 'count' items, or if we asked for 50 and got less than 50...
				finished = (accumulated.Count >= count || results.Count == 0 || (results.Count % chunkSize != 0));
			}
			if (accumulated.Count > count)
				return accumulated.GetRange(0,count);
			return accumulated;
		}

		public void Delete(string id){
			_api.executeRequest<TModel>(Path(), RestSharp.Method.DELETE, id);
		}

		public void Delete(TModel model) {
			_api.executeRequest<TModel>(Path(), RestSharp.Method.DELETE, model._id);
		}

		public TModel Find(string id) {
			if (id == null)
				return null;
			return _api.executeRequest<TModel>(Path(), RestSharp.Method.GET, id);
		}

		public TModel Build(params object[] args) {
			var l = args.ToList();
			l.Insert(0, this);
			args = l.ToArray();

			return (TModel)Activator.CreateInstance(typeof(TModel), args);
		}

	}
}