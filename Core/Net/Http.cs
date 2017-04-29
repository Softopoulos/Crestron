using System.Text;

using Crestron.SimplSharp.Net.Http;
using Https = Crestron.SimplSharp.Net.Https;
using HttpsRequestType = Crestron.SimplSharp.Net.Https.RequestType;

// ReSharper disable SpellingError
namespace Softopoulos.Crestron.Core.Net
{
    public static class Http
    {
		public static string SecureGet(string url)
		{
			Https.HttpsClient client = new Https.HttpsClient();
			Https.HttpsClientRequest request = new Https.HttpsClientRequest();
			request.Url.Parse(url);
			request.RequestType = HttpsRequestType.Get;
			request.Encoding = Encoding.ASCII;
			Https.HttpsClientResponse response = client.Dispatch(request);
			return response.ContentString;
		}

		public static string Get(string url)
		{
			HttpClient client = new HttpClient();
			HttpClientRequest request = new HttpClientRequest();
			request.Url.Parse(url);
			request.RequestType = RequestType.Get;
			//request.Encoding = Encoding.ASCII;
			HttpClientResponse response = client.Dispatch(request);
			return response.ContentString;
		}

		public static string Put(string url, string body)
		{
			HttpClient client = new HttpClient();
			HttpClientRequest request = new HttpClientRequest();
			request.Url.Parse(url);
			request.RequestType = RequestType.Put;
			//request.Encoding = Encoding.ASCII;
			request.ContentString = body != null ? body : "";
			HttpClientResponse response = client.Dispatch(request);
			return response.ContentString;
		}

		public static string Post(string url, string body)
		{
			HttpClient client = new HttpClient();
			HttpClientRequest request = new HttpClientRequest();
			request.Url.Parse(url);
			request.RequestType = RequestType.Post;
			//request.Encoding = Encoding.ASCII;
			request.ContentString = body != null ? body : "";
			HttpClientResponse response = client.Dispatch(request);
			return response.ContentString;
		}

		public static string Delete(string url)
		{
			HttpClient client = new HttpClient();
			HttpClientRequest request = new HttpClientRequest();
			request.Url.Parse(url);
			request.RequestType = RequestType.Delete;
			//request.Encoding = Encoding.ASCII;
			HttpClientResponse response = client.Dispatch(request);
			return response.ContentString;
		}
	}
}
