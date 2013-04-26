using System;
using System.Collections.Generic;
using RestSharp;

namespace Populr
{
	public class Tracer : RestfulModel
	{
		public string name { get; set; }
		public string code { get; set; }
		public bool notify_on_open { get; set; }
		public string notify_webhook { get; set; }
		public TracerAnalytics analytics { get; set; }

		public Tracer () { }
		public Tracer (RestfulObject parent) : base(parent) {}

		public int Views() 
		{
			return analytics.views;
		}

		public int Clicks() 
		{
			return analytics.clicks;
		}

		public int ClicksForRegion(string region_id) 
		{
			if ((analytics.assets == null) || (analytics.assets.ContainsKey(region_id) == false))
				return 0;

			int clicks = 0;
			foreach (KeyValuePair<string, Dictionary<string, int>> region in analytics.assets[region_id])
				clicks += region.Value["clicks"];
			return clicks;
		}

		public int ClicksForLink(string link) 
		{
			if (analytics.links.ContainsKey(link) == false)
				return 0;
			return analytics.links[link]["clicks"];
		}

		public int ClicksForAsset(string asset_id)
		{
			if ((analytics == null) || (analytics.assets == null))
				return 0;

			foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, int>>> entry in analytics.assets) {
				foreach (KeyValuePair<string, Dictionary<string, int>> asset in entry.Value) {
					if (asset.Key == asset_id) {
						return asset.Value["clicks"];
					}
				}
			}
			return 0;
		}
	}

	public class TracerAnalytics
	{
		public int views { get; set; }
		public int clicks { get; set; }
		public Dictionary<string, Dictionary<string, int>> links { get; set; }
		public Dictionary<string, Dictionary<string, Dictionary<string, int>>> assets { get; set; }
	}

}

