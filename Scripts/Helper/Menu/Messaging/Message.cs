using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Message : Window, IClickable {

    TMP_Text username;
    TMP_Text timestamp;
    public TMP_Text content;

    public Button context;
    [SerializeField] Selector options; // i.e, "Delete" & "Edit" options, common to all messages from user

    public void Set(Message.Data data){
        Set(data.username, data.timestamp, data.content);
    }
    public void Set(string username, long timestamp, string content){
        SetUI();

        this.username.text = username;
        this.timestamp.text = TimestampToDate(timestamp);
        this.content.text = content;

        this.content.ForceMeshUpdate();
        if(this.content.isTextOverflowing) {
            float height = this.content.textInfo.lineInfo[0].lineHeight*(this.content.textInfo.lineCount);
            this.context.image.rectTransform.sizeDelta = new Vector2(this.context.image.rectTransform.sizeDelta.x, height);
        }
    }

    public override void SetUI(){
        TMP_Text[] texts = this.GetComponentsInChildren<TMP_Text>();

        this.username = texts[0];
        this.timestamp = texts[1];
        this.content = texts[2];

        this.context = this.GetComponent<Button>();

        this.context.onClick.AddListener(()=>{OnClick();});
    }

    public void OnClick(){
        if(options.IsVisible()) options.Hide();
        else options.Show();
    }

    public static string TimestampToDate(long timestamp)
    {
        // Unix timestamp is seconds past epoch
        if(timestamp<0) return "";

        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
        dateTime = dateTime.AddMilliseconds((double)timestamp).ToLocalTime();
        return dateTime.ToString("dd:mm:yyyy HH:mm:ss");
    }

    public class Data{
        public string username;
        public long timestamp;
        public string content;
        public Data(string username, long timestamp, string content){
            this.username = username;
            this.timestamp = timestamp;
            this.content = content;
        }

        public override string ToString(){
            return "{"+$"username: \"{this.username}\", timestamp: {this.timestamp}, content: \"{this.content}\""+"}";
        }
    }
}