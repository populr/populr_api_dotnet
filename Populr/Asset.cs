using System;
using RestSharp;
using Populr;

namespace Populr
{
	public class Asset : Model
	{

		public string description { get; set; }
		public DateTime created_at { get; set; }
		public string link { get; set; }
		public string title { get; set; }

		public Asset ()
		{
		}
	}

	public class ImageAsset : Asset
	{
		public ImageAsset ()
		{
		}

		public void Save()
		{
			var saved = _api.executeRequest<ImageAsset>("/images/{id}", Method.PUT, this._id, this);
			if (saved != null) {
				this.copyFrom(saved);
			}
		}
	}

	public class DocumentAsset : Asset
	{
		public DocumentAsset ()
		{
		}

		public void Save()
		{
			var saved = _api.executeRequest<ImageAsset>("/images/{id}", Method.PUT, this._id, this);
			if (saved != null) {
				this.copyFrom(saved);
			}
		}
	}

}

