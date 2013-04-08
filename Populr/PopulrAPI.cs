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

		public APIException (APIError err)
		{
			type = err.error_type;
			message = err.error;
		}
	}

	public class PopulrAPI
	{
		RestClient client;

		public PopulrAPI(String api_key, String host = "api.lvh.me:3000")
		{
			client = new RestClient("http://" + host);
			client.Authenticator = new HttpBasicAuthenticator(api_key, "");
		}

		// ---- Pop Templates ----//

		public List<PopTemplate> getTemplates ()
		{
			return executeIndexRequest<PopTemplate> ("/templates");
		}

		public PopTemplate getTemplate(string id)
		{
			return executeRequest<PopTemplate>("/templates/{id}", Method.GET, id, null);
		}


		// ---- Pops ----//

		public Pop getPop(string id)
		{
			return executeRequest<Pop>("/pops/{id}", Method.GET, id, null);
		}

		public List<Pop> getPops()
		{
			return executeIndexRequest<Pop>("/pops");
		}

		// ---- Assets ----//

		public ImageAsset getImageAsset (string id)
		{
			return executeRequest<ImageAsset>("/images", Method.GET, id, null);
		}

		public ImageAsset createImageAsset (FileStream stream, string title, string link)
		{
			return executeFilePostRequest<ImageAsset>("/images", stream, title, link);
		}

		public DocumentAsset getDocumentAsset (string id)
		{
			return executeRequest<DocumentAsset>("/images", Method.GET, id, null);
		}

		public DocumentAsset createDocumentAsset (FileStream stream, string title)
		{
			return executeFilePostRequest<DocumentAsset>("/documents", stream, title, null);
		}




		internal T executeRequest<T> (string path, Method method, string id, object body) where T: Model, new()
		{
			var request = new RestRequest (path, method);
			if (id != null)
				request.AddUrlSegment ("id", id);
			request.OnBeforeDeserialization = s => checkForError(s);
			request.RequestFormat = DataFormat.Json;
			if (body != null)
				request.AddBody(body);

			RestResponse<T> response = (RestResponse<T>)client.Execute<T>(request);
			T obj = (T)response.Data;
			if ((obj == null) || (obj._id == null))
				return null;
			
			obj._api = this;
			return obj;
		}

		internal List<T> executeIndexRequest<T>(string path) where T : Model, new()
		{
			var request = new RestRequest (path, Method.GET);
			request.OnBeforeDeserialization = s => checkForError(s);
			request.RequestFormat = DataFormat.Json;
			RestResponse<List<T>> response = (RestResponse<List<T>>)client.Execute<List<T>>(request);
			List<T> list = response.Data;
			foreach (T item in list)
				item._api = this;
			return list;
		}

		internal T executeFilePostRequest<T>(string path, FileStream stream, string title, string link) where T: Model, new()
		{
			var request = new RestRequest (path, Method.POST);
			request.OnBeforeDeserialization = s => checkForError(s);
			request.RequestFormat = DataFormat.Json;
			request.AddParameter("title", title);
			request.AddParameter("link", link);

			//convert stream to byte array
			var byteArray = new byte[stream.Length];
			stream.Read(byteArray, 0, (int)stream.Length);
			request.AddFile("file", byteArray, "file", "application/octet-stream");

			RestResponse<T> response = (RestResponse<T>)client.Execute<T>(request);
			T obj = (T)response.Data;
			if ((obj == null) || (obj._id == null))
				return null;
			return obj;
		}

		internal void checkForError (IRestResponse response)
		{
			response.ContentType = "application/json";
			if (response.Content.Contains ("\"error_type\":")) {
				APIError details = new RestSharp.Deserializers.JsonDeserializer().Deserialize<APIError>(response);
				if (details.error_type != "not_found")
					throw new APIException (details);
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

