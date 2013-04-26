using System;
using System.Collections.Generic;
using RestSharp;
using Populr;

namespace Populr
{
	public class Pop : RestfulModel
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
		public Dictionary<string, Dictionary<string, string>> unpopulated_api_regions { get; set; }

		internal Dictionary<string, string> _newly_populated_tags;
		internal Dictionary<string, List<string>> _newly_populated_regions;

		public Pop ()
		{
			// the pop is auto-inflated from RESTSharp's JSON deserializer
			// throw new APIException("You must create a pop from a template.");
		}

		public Pop (RestfulObject parent)
		{
			if (parent.Parent () == null)
				throw new Exception("You must create a pop from a template's pops collection.");
			Setup((Template)parent.Parent());
		}

		public Pop (Template template)
		{
			Setup (template);
		}

		internal void Setup(Template template) 
		{
			_api = template._api;
			_parent = template.Pops();
			template_id = template._id;
			unpopulated_api_tags = template.api_tags;
			unpopulated_api_regions = new Dictionary<string, Dictionary<string, string>>();
			foreach (KeyValuePair<string, Dictionary<string, string>> entry in template.api_regions)
				unpopulated_api_regions.Add(entry.Key, entry.Value);
			title = template.title;
			name = template.name;
		}

		public override object APIRepresentation ()
		{
			return new PopulrAPI.APIPopTransaction(this);
		}

		public override string Path (Method method = Method.GET) 
		{
			if ((method == Method.POST) || (method == Method.PUT) || (method == Method.DELETE)){
				if (_id != null)
					return _api.Path () + "/pops/"+ _id;
				else
					return _api.Path () + "/pops";
			} else {
				return base.Path(method);
			}
		}

		public RestfulModelCollection<Tracer> Tracers ()
		{
			if (_id == null)
				throw new APIException("unsaved", "Sorry, you can't create tracers for an unsaved pop. Call pop.Save() first.");
			return new RestfulModelCollection<Tracer>(this);
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
			return unpopulated_api_regions.ContainsKey(region_identifier);
		}

		public void PopulateRegion (string region_identifier, Asset asset)
		{
			if ((asset == null) || (asset._id == null))
				throw new APIException("unsaved", "Please save the asset before adding it to a pop.");
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

		public void Publish ()
		{
			if (_id == null)
				Save ();
			Pop published = _api.executeRequest<Pop>(this.Path(Method.POST) + "/publish", Method.POST, null);
			if (published != null) {
				this.CopyFrom(published);
			}
		}
		
		public void Unpublish ()
		{
			if (_id == null)
				Save ();
			Pop unpublished = _api.executeRequest<Pop>(this.Path(Method.POST) + "/unpublish", Method.POST, null);
			if (unpublished != null) {
				this.CopyFrom(unpublished);
			}
		}
	}
}

