using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class API {
    public static class Request{

        // Read
        public static async Task<string> Get(API.Information info, string endpoint)
        {
            return await Send(Request.Type.GET, endpoint, "", info.user, info.password, info.address, info.port);
        }

        // Content = JSON
        // Create
        public static async Task<string> Post(API.Information info, string endpoint, string content)
        {
            return await Send(Request.Type.POST, endpoint, content, info.user, info.password, info.address, info.port);
        }
        // Update
        public static async Task<string> Put(API.Information info, string endpoint, string content)
        {
            return await Send(Request.Type.PUT, endpoint, content, info.user, info.password, info.address, info.port);
        }
        // Delete
        public static async Task<string> Delete(API.Information info, string endpoint, string content)
        {
            return await Send(Request.Type.DELETE, endpoint, content, info.user, info.password, info.address, info.port);
        }

        private static async Task<string> Send(Request.Type method, string endpoint, string content, string user, string password, string url, int port = -1){

            if(port!=-1) url+=":"+port;

            if(url[url.Length-1]!='/') url+='/';
            url+=endpoint;

            UnityWebRequest request = new UnityWebRequest(url, method.ToString());
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            request.downloadHandler = dH;

            string auth = "{"+$"\"username\":\"{user}\", \"password\":\"{password}\""+"}";

            request.SetRequestHeader("User-Agent", "User agent");
            request.SetRequestHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(auth)));
            if(content!="") {
                request.SetRequestHeader("Content-Type", "application/json");
                UploadHandlerRaw uH = new UploadHandlerRaw(System.Text.Encoding.ASCII.GetBytes(content));
                request.uploadHandler = uH;
            }

            request.certificateHandler = new BypassCertificate();
            await request.SendWebRequest();

            if(request.error!=null) return request.error;
            else return request.downloadHandler.text;
        }

        public class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                // Always returns true, indicating that the certificate is valid
                return true;
            }
        }

        public enum Type{
            GET,
            DELETE,
            PUT,
            POST,
        }
    }

    [Serializable]
    public class Information{
        public string address = "localhost";
        public int port = -1;

        public string user = "";
        public string password = "";

        public Metadata metadata = null;

        public override string ToString(){
            return "{"+$"address:\"{address}\", port:\"{port}\", user:\"{user}\", password:\"{password}\""+"}";
        }

        public class Metadata{
            public string id = "";
            public bool is_admin = false;
        }
    }
}
