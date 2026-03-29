using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "Information", menuName = "Pearl/API/Information")]
public class API_Information : ScriptableObject{
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
