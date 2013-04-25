using System;
using System.IO;
using RestSharp;
using Populr;

namespace Populr
{
	public class Asset : RestfulModel
	{

		public string description { get; set; }
		public DateTime created_at { get; set; }
		public string link { get; set; }
		public string title { get; set; }
		internal FileStream _file;

		public Asset ()
		{
		}
		public Asset (RestfulObject parent) : base (parent)
		{
		}

		public override object Save ()
		{
			if ((_file == null || _id != null))
				return base.Save();
			else {
				Asset saved = _api.executeFilePostRequest<Asset>(this.Path (Method.POST), _file, title, link);
				if (saved != null) {
					this.CopyFrom (saved);
					return this;
				}
				return null;
			}	
		}
	}

	public class ImageAsset : Asset
	{
		public ImageAsset () {}
		public ImageAsset (RestfulObject parent, FileStream file, string title = null, string link = null) : base(parent)
		{
			this.title = title;
			this.link = link;
			_file = file;
		}
	}


	public class EmbedAsset : Asset
	{
		public EmbedAsset ()
		{
		}
	}

	public class DocumentAsset : Asset
	{
		public DocumentAsset() {}
		public DocumentAsset (RestfulObject parent, FileStream file, string title = null) : base(parent)
		{
			this.title = title;
			_file = file;
		}

	}

}

