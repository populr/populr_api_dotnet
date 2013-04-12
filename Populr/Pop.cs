using System;
using System.Collections.Generic;
using RestSharp;
using Populr;

namespace Populr
{
	public class Pop : Model
	{
		public string template_id { get; set; }
		public string title { get; set; }
		public DateTime created_at { get; set; }
		public string domain { get; set; }
		public string slug { get; set; }
		public string name { get; set; }
		public string password { get; set; }
		public string published_pop_url { get; set; }
		public List<string> label_names { get; set; }
		public List<string> unpopulated_api_tags { get; set; }
		public List<string> unpopulated_api_regions { get; set; }

		internal Dictionary<string, string> _newly_populated_tags;
		internal Dictionary<string, List<string>> _newly_populated_regions;

		public Pop ()
		{
			// the pop is auto-inflated from RESTSharp's JSON deserializer
			throw new APIException("You must create a pop from a template.");
		}
		
		public Pop (PopTemplate template)
		{
			_api = template._api;
			template_id = template._id;
		}

		public bool HasUnpopulatedTag (string tag)
		{
			return unpopulated_api_tags.Contains(tag);
		}

		public void PopulateTag (string tag, string value)
		{
			if (_newly_populated_tags == null)
				_newly_populated_tags = new Dictionary<string, string> ();
			_newly_populated_tags [tag] = value;
		}

		public bool HasUnpopulatedRegion (string region_identifier)
		{
			return unpopulated_api_regions.Contains(region_identifier);
		}

		public void PopulateRegion (string region_identifier, Asset asset)
		{
			PopulateRegion(region_identifier, asset._id);
		}

		public void PopulateRegion (string region_identifier, string assetID)
		{
			if (_newly_populated_regions == null)
				_newly_populated_regions = new Dictionary<string, List<string>> ();
			if (!_newly_populated_regions.ContainsKey(region_identifier))
				_newly_populated_regions [region_identifier] = new List<string>();
			_newly_populated_regions [region_identifier].Add(assetID);
		}

		public void Save ()
		{
			Pop saved = null;
			if (_id != null)
				saved = _api.executeRequest<Pop>("/pops/{id}", Method.PUT, _id, new PopulrAPI.APIPopTransaction(this));
			else
				saved = _api.executeRequest<Pop>("/pops", Method.POST, null, new PopulrAPI.APIPopTransaction(this));

			if (saved != null) {
				this.copyFrom(saved);
			}
		}

		public void Publish ()
		{
			if (_id == null)
				Save ();
			Pop published = _api.executeRequest<Pop>("/pops{id}/publish", Method.POST, null, null);
			if (published != null) {
				this.copyFrom(published);
			}
		}
		
		public void Unpublish ()
		{
			if (_id == null)
				Save ();
			Pop unpublished = _api.executeRequest<Pop>("/pops{id}/unpublish", Method.POST, null, null);
			if (unpublished != null) {
				this.copyFrom(unpublished);
			}
		}
	}
}

