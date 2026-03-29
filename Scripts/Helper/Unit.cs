using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Unit : MonoBehaviour{
    public static Unit Manager = null;
    void Awake(){
        if(Unit.Manager==null) Unit.Manager = this;
        else Destroy(this);
    }

    [SerializeField] private API_Information[] API_ACCESS;

    void Start(){
        StartCoroutine(StartTesting());
    }

    private IEnumerator StartTesting(){
        Debug.Log("== START ==");
        foreach (API_Information ACCESS in API_ACCESS){
            yield return StartCoroutine(Run(ACCESS));
            Debug.Log("");
        }
        Debug.Log("=== END ===");
    }

    private IEnumerator Run(API_Information api){
        Debug.Log("["+api.name+"]: "+api.user+" - "+api.address);

        Debug.Log("CREATE:");
        yield return Create.Message(api);
        yield return Create.Newsletter(api);
        yield return Create.Admin(api);

        Debug.Log("READ:");
        yield return Read.Message(api);
        yield return Read.Newsletter(api);
        yield return Read.Users(api);
        yield return Read.Admin(api);

        Debug.Log("UPDATE:");
        yield return Update.Message(api);
        yield return Update.Admin(api);

        Debug.Log("DELETE:");
        yield return Delete.Message(api);
        yield return Delete.Newsletter(api);
        yield return Delete.Admin(api);
    }

    private static class Create {
        public static IEnumerator Message(API_Information api) {
            yield break;
        }
        public static IEnumerator Newsletter(API_Information api) {
            yield break;
        }
        public static IEnumerator Admin(API_Information api) {
            yield break;
        }
    }

    private static class Read {
        public static IEnumerator Message(API_Information api) {
            Task<string> res = API.Request.Get(api, "message");
            while (!res.IsCompleted) yield return null;

            Message.Data[] res_data = JSON.Get<Message.Data[]>(res.Result);
            if(res_data!=null && res_data.Length>0) Debug.Log("Working!");
            else Debug.Log("Error!\n"+res.Result);
        }
        public static IEnumerator Newsletter(API_Information api) {
            Task<string> res = API.Request.Get(api, "newsletter");
            while (!res.IsCompleted) yield return null;

            Article.Data[] res_data = JSON.Get<Article.Data[]>(res.Result);
            if(res_data!=null && res_data.Length>0) Debug.Log("Working!");
            else Debug.Log("Error!\n"+res.Result);
        }
        public static IEnumerator Users(API_Information api) {
            Task<string> res = API.Request.Get(api, "users");
            while (!res.IsCompleted) yield return null;

            JToken res_data = JSON.Get<JToken>(res.Result);
            if(res_data.HasValues) Debug.Log("Working!");
            else Debug.Log("Error!\n"+res.Result);
        }
        public static IEnumerator Admin(API_Information api) {
            Task<string> res = API.Request.Get(api, "admin");
            while (!res.IsCompleted) yield return null;

            JToken res_data = JSON.Get<JToken>(res.Result);
            if(res_data.HasValues) Debug.Log("Working!");
            else Debug.Log("Error!\n"+res.Result);
        }
    }

    private static class Update {
        public static IEnumerator Message(API_Information api) {
            Task<string> res = API.Request.Get(api, "message");
            while (!res.IsCompleted) yield return null;

            Message.Data test_data = (JSON.Get<Message.Data[]>(res.Result))[0];

            string content = test_data.content;
            test_data.content+="12";

            res = API.Request.Put(api, "message", test_data.ToString());
            while (!res.IsCompleted) yield return null;


            res = API.Request.Get(api, "message");
            while (!res.IsCompleted) yield return null;
            Message.Data test_data_updated = (JSON.Get<Message.Data[]>(res.Result))[0];


            if(test_data_updated!=null && test_data_updated.content!=content) {
                Debug.Log("Working!");

                test_data.content = content;
                res = API.Request.Put(api, "message", test_data.ToString());
            }
            else Debug.Log("Error!\n"+res.Result);
        }
        public static IEnumerator Admin(API_Information api) {

            Task<string> res = API.Request.Get(api, "users");
            while (!res.IsCompleted) yield return null;
            JToken user = JSON.Get<JToken>(res.Result);

            string test_id = "";
            string name = "";
            string updated_name = "";
            if(user.HasValues && user.First!=null) {
                user = user.First;

                test_id = user.SelectToken("id").ToString();
                name = user.SelectToken("name").ToString();

                res = API.Request.Put(api, "admin", "{\"id\":\"" + test_id +"\", \"username\":\"" + name + "12\"}");
                while (!res.IsCompleted) yield return null;


                res = API.Request.Get(api, "users");
                while (!res.IsCompleted) yield return null;
                user = (JSON.Get<JToken>(res.Result)).First;

                updated_name = user.SelectToken("name").ToString();
            }

            if(user.HasValues && user.First!=null && updated_name!=null && name!=null && updated_name != name) {
                Debug.Log("Working!");

                API.Request.Put(api, "admin", "{\"id\":\"" + test_id +"\", \"username\":\""+name+"\"}");
            }
            else {
                if(name!="") Debug.Log("Error!\n (old)"+name+" == (new)"+updated_name);
                else Debug.Log("Error!\n"+res.Result);
            }
        }
    }
    
    private static class Delete {
        public static IEnumerator Message(API_Information api) {
            yield break;
        }
        public static IEnumerator Newsletter(API_Information api) {
            yield break;
        }
        public static IEnumerator Admin(API_Information api) {
            yield break;
        }
    }
}