using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messaging : Menu {

    private Button config;
    private Window config_window;
    private TMP_InputField config_address;
    private TMP_InputField config_port;
    private TMP_InputField config_user;
    private TMP_InputField config_password;


    private ScrollRect message_tabs;
    private TMP_InputField input;
    private Button send;

    private Message.Data[] messages;
    [SerializeField] private API.Information API_ACCESS = default;

    void Start(){
        Hide();
    }

    public override async void SetUI(){
        Button[] buttons = this.GetComponentsInChildren<Button>();
        this.config = buttons[0];
        this.config.onClick.AddListener(()=>{this.config_window.Alternate();});

        this.exit = buttons[1];
        this.exit.onClick.AddListener(Hide);

        this.send = buttons[2];
        this.send.onClick.AddListener(Send);

        this.message_tabs = this.GetComponentInChildren<ScrollRect>();
        TMP_InputField[] inputs = this.GetComponentsInChildren<TMP_InputField>();

        this.input = this.GetComponentInChildren<TMP_InputField>();
        this.input.onSubmit.AddListener((string foo)=>{Send();});

        this.drag_area = this.GetComponentInChildren<Image>();

        this.config_window = this.GetComponentsInChildren<Window>()[1];

        TMP_InputField[] config_inputs = this.config_window.GetComponentsInChildren<TMP_InputField>();
        this.config_address = config_inputs[0];
        this.config_address.text = API_ACCESS.address;
        this.config_address.onSubmit.AddListener((string address)=>{this.API_ACCESS.address=address;});

        this.config_port = config_inputs[1];
        this.config_port.text = (API_ACCESS.port>0) ? API_ACCESS.port.ToString() : "";
        this.config_port.onSubmit.AddListener((string port)=>{
            if(port=="") port = "-1";
            this.API_ACCESS.port=Int32.Parse(port);
        });

        this.config_user = config_inputs[2];
        this.config_user.text = API_ACCESS.user;
        this.config_user.onSubmit.AddListener((string user)=>{this.API_ACCESS.user=user;});

        this.config_password = config_inputs[3];
        this.config_password.text = API_ACCESS.password;
        this.config_password.onSubmit.AddListener((string password)=>{this.API_ACCESS.password=password;});

        this.config_window.Hide();

        this.messages = await Request.Read.Messages(API_ACCESS);
    }

    private IEnumerator WaitForUpdate(){
        while(true){
            Task<Message.Data[]> req = Request.Read.Messages(API_ACCESS);
            while(!req.IsCompleted) yield return null;
            Message.Data[] newest = req.Result;

            if(newest!=null) {
                if (newest != messages) {
                    this.messages = newest;
                    yield return StartCoroutine(LoadTabs(true));
                }
            }


            yield return new WaitForSecondsRealtime(0.1f);
        }
    }

    private bool HasTabs(){
        if(this.message_tabs ==null) return false;
        return (this.message_tabs.content.childCount>0);
    }

    private void ClearTabs(){
        if(HasTabs()) {
            foreach (Transform child in this.message_tabs.content) {
                RemoveTab(child.gameObject, this.message_tabs);
            }
        }
    }

    private IEnumerator LoadTabs(bool reload = false){
        if(this.messages == null) yield break;
        float t = message_tabs.verticalNormalizedPosition;
        bool is_add = this.message_tabs.content.childCount<this.messages.Length;

        if(reload) ClearTabs();

        if(this.messages[0].timestamp>this.messages[this.messages.Length-1].timestamp) Array.Reverse(this.messages);
        foreach (Message.Data message in this.messages){
            AddTab(message, this.message_tabs);
        }
        yield return null;

        if(is_add && t<=0.01f) {
            yield return new WaitForEndOfFrame();
            message_tabs.verticalNormalizedPosition = (t);
        }
    }

    public int GetTabIndex(Message.Data tab){
        for(int i = 0; i<this.messages.Length; i++){
            if(this.messages[i] == tab) return i;
        }
        return -1;
    }

    private GameObject AddTab(Message.Data tab_message, ScrollRect tab_list){
        GameObject prefab = Utils.getPrefab(Path.Prefabs.Messaging.Message);
        GameObject new_tab = Instantiate(prefab, this.message_tabs.content);

        // Set message data
        new_tab.GetComponent<Message>().Set(tab_message);
        return new_tab;
    }

    private void RemoveTab(GameObject tab, ScrollRect tab_list){
        for(int i = 0; i<this.message_tabs.content.childCount; i++){
            if(this.message_tabs.content.GetChild(i).gameObject == tab) RemoveTab(i, tab_list);
        }
    }

    public void RemoveTab(int index, ScrollRect tab_list){
        Destroy(tab_list.content.GetChild(index).gameObject);
    }

    private void Send(){
        Add(this.input.text);
        this.input.text = "";
    }

    private void Add(string text){
        long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        Message.Data new_message = new Message.Data(API_ACCESS.user, timestamp, text);

        Request.Create.Message(API_ACCESS, new_message);
    }

    public override void Show_ExtendedBehaviour(){
        StartCoroutine(WaitForUpdate());
        StartCoroutine(Drag());
    }
    public override void Hide_ExtendedBehaviour(){
        this.config_window.Hide();
    }




    private static class Request {
        public static class Create {
            public static async Task<string> Message(API.Information info, Message.Data message) {
                return await API.Request.Post(info,"Message", message.ToString());
            }
        }

        public static class Read {
            public static async Task<Message.Data[]> Messages(API.Information info) {
                if(info.address == "localhost") {
                    string msg = "["+
                    (new Message.Data("Tester", 0, "1.").ToString()+",")+
                    (new Message.Data("Tester", 0, "2.").ToString()+",")+
                    (new Message.Data("Tester", 0, "3.").ToString()+",")+
                    (new Message.Data("Tester", 0, "4.").ToString()+",")+
                    (new Message.Data("Tester", 0, "5.").ToString()+",")+
                    (new Message.Data("Tester", 0, "6.").ToString()+",")+
                    (new Message.Data("Tester", 0, "7.").ToString()+",")+
                    (new Message.Data("Tester", 0, "8.").ToString()+",")+
                    (new Message.Data("Tester", 0, "9.").ToString()+",")+
                    (new Message.Data("Tester", 0, "10.").ToString()+",")+
                    (new Message.Data("Tester", 0, "11.").ToString()+",")+
                    (new Message.Data("Tester", 0, "12.").ToString()+",")+
                    (new Message.Data("Tester", 0, "13.").ToString()+",")+
                    (new Message.Data("Tester", 0, "14.").ToString()+",")+
                    (new Message.Data("Tester", 0, "15.").ToString()+",")+
                    (new Message.Data("Tester", 0, "16.").ToString()+",")+
                    (new Message.Data("Tester", 0, "17.").ToString()+",")+
                    (new Message.Data("Tester", 0, "18.").ToString()+",")+
                    ((UnityEngine.Random.Range(0, 60) > 30) ? new Message.Data("Dos", 10000, "Oh\nshit").ToString() : "")+
                    "]";
                    return JSON.Get<Message.Data[]>(msg);
                }

                string res = await API.Request.Get(info, "");
                Message.Data[] messages = JSON.Get<Message.Data[]>(res);

                if(messages==null) messages = new Message.Data[]{new Message.Data("ERROR", -1, res)};

                return messages;
            }
        }

        public static class Update {
            public static async Task<string> Message(API.Information info, Message.Data message) {
                return await API.Request.Put(info, "Message", message.ToString());
            }
        }

        public static class Delete {
            public static async Task<string> Message(API.Information info, Message.Data message) {
                return await API.Request.Delete(info, "Message", message.ToString());
            }
        }

    }
}