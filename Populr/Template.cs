using System;
using System.Collections.Generic;

namespace Populr
{
	public class Template : RestfulModel
	{
		public string title { get; set; }
		public DateTime created_at { get; set; }
		public string name { get; set; }
		public List<string> label_names { get; set; }
		public List<string> api_tags { get; set; }
		public Dictionary<string, Dictionary<string, string>> api_regions { get; set; }

		public Template ()
		{
		}

		public RestfulModelCollection<Pop> Pops ()
		{
			return new RestfulModelCollection<Pop>(this);
		}
	}
}

