using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RestSharp;

namespace Populr
{
	public class APIError
	{
		public string error_type { get; set; }
		public string error { get; set; }
	}

	public class APIException: ApplicationException
	{
		public string type { get; set; }
		public string message { get; set; }

		public APIException (string t, string err)
		{
			type = t;
			message = err;
		}

		public APIException (APIError err)
		{
			type = err.error_type;
			message = err.error;
		}
	}

	public class PopulrAPI : RestfulObject
	{
		RestClient _client;
		String _apiVersion;

		public PopulrAPI(String api_key, String host = "https://api.populr.me")
		{
			if ((host.Contains("https") == false) && (host.Contains("localhost") || host.Contains("lvh")) == false)
				throw new APIException("authentication", "Please connect to the Populr.me API via HTTPS - we use HTTP basic auth!");

			_apiVersion = "v0";
			_api = this;

			_client = new RestClient(host);
			_client.Authenticator = new HttpBasicAuthenticator(api_key, "");
		}

		// ---- Pop Templates ----//

		public RestfulModelCollection<Template> Templates ()
		{
			return new RestfulModelCollection<Template> (this);
		}

		// ---- Pops ----//
		
		public RestfulModelCollection<Pop> Pops ()
		{
			return new RestfulModelCollection<Pop> (this);
		}

		// ---- Assets ----//

		public RestfulModelCollection<DocumentAsset> Documents ()
		{
			return new RestfulModelCollection<DocumentAsset> (this, "documents");
		}
		
		public RestfulModelCollection<ImageAsset> Images ()
		{
			return new RestfulModelCollection<ImageAsset> (this, "images");
		}
		
		public RestfulModelCollection<EmbedAsset> Embeds ()
		{
			return new RestfulModelCollection<EmbedAsset> (this, "embeds");
		}

		public override string Path (Method method = Method.GET)
		{
			return "/" + _apiVersion;
		}

		internal T executeRequest<T> (string path, Method method, object body = null) where T: RestfulModel, new()
		{
			var request = new RestRequest (path, method);
			request.OnBeforeDeserialization = s => CheckForError(s);
			request.RequestFormat = DataFormat.Json;
			if (body != null)
				request.AddBody(body);

			RestResponse<T> response = (RestResponse<T>)_client.Execute<T>(request);
			T obj = (T)response.Data;
			if ((obj == null) || (obj._id == null))
				return null;
			
			obj._api = this;
			return obj;
		}

		internal T executeFilePostRequest<T>(string path, FileStream stream, string title, string link) where T: RestfulModel, new()
		{
			var request = new RestRequest (path, Method.POST);
			request.OnBeforeDeserialization = s => CheckForError(s);
			request.RequestFormat = DataFormat.Json;
			request.AddParameter("title", title);
			request.AddParameter("link", link);

			//convert stream to byte array
			var byteArray = new byte[stream.Length];
			stream.Read(byteArray, 0, (int)stream.Length);
			request.AddFile("file", byteArray, "file", "application/octet-stream");

			RestResponse<T> response = (RestResponse<T>)_client.Execute<T>(request);
			T obj = (T)response.Data;
			if ((obj == null) || (obj._id == null))
				return null;
			return obj;
		}
		
		internal List<T> executeIndexRequest<T>(string path, int offset = 0, int count = 50) where T : RestfulModel, new()
		{
			var request = new RestRequest (path + "?offset="+offset + "&count="+count, Method.GET);
			request.OnBeforeDeserialization = s => CheckForError(s);
			request.RequestFormat = DataFormat.Json;
			RestResponse<List<T>> response = (RestResponse<List<T>>)_client.Execute<List<T>>(request);
			List<T> list = response.Data;
			if (list == null)
				return new List<T>();

			foreach (T item in list)
				item._api = this;
			return list;
		}

		internal void CheckForError (IRestResponse response)
		{
			if (response.ErrorException != null)
				throw response.ErrorException;

			response.ContentType = "application/json";
			if (response.Content.Contains ("\"error_type\":")) {
				APIError details = new RestSharp.Deserializers.JsonDeserializer().Deserialize<APIError>(response);
				if (details.error_type != "not_found") {
					Console.WriteLine(details.error);
					throw new APIException (details);
				}
		    }
		}

		
		internal class APIPopTransaction
		{
			public Pop pop { get; set; }
			public JsonObject populate_tags { get; set; }
			public JsonObject populate_regions { get; set; }
			
			internal APIPopTransaction (Pop p)
			{
				this.pop = p;
				this.populate_tags = new JsonObject();
				this.populate_regions = new JsonObject();

				if (p._newly_populated_tags != null)
					foreach (System.Collections.Generic.KeyValuePair<string, string> pair in p._newly_populated_tags)
						this.populate_tags.Add (pair.Key, pair.Value);

				if (p._newly_populated_regions != null)
					foreach (KeyValuePair<string, List<string>> pair in p._newly_populated_regions)
						this.populate_regions.Add (pair.Key, pair.Value);
			}
		}
	}
}

