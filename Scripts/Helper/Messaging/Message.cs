using System;
using TMPro;
using UnityEngine.UI;

public class Message : Window, IClickable {

    TMP_Text username;
    TMP_Text timestamp;
    TMP_Text content;

    Button context;
    Selector options;

    void Start(){
        SetUI();
    }

    public void Set(string username, float timestamp, string content, Selector options = null){
        this.username.text = username;
        this.timestamp.text = TimestampToDate(timestamp);
        this.content.text = content;

        if(options!=null) this.options = options;
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

    public static string TimestampToDate(float timestamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
        dateTime = dateTime.AddSeconds((double)timestamp).ToLocalTime();
        return dateTime.ToString();
    }
}