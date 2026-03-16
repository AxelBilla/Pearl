using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Message : Window, IClickable {

    public Messaging parent;

    TMP_Text username;
    TMP_Text timestamp;
    public TMP_Text content;

    public Button context;

    Message.Data data = null;
    public void Set(Message.Data data, Messaging parent = null){
        this.data = data;
        Set(data.username, data.timestamp, data.content, parent);
    }
    public void Set(string username, long timestamp, string content, Messaging parent = null){
        this.parent = parent;
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

        StartCoroutine(OnClick());
    }

    private static float timer = 0f;
    private static Button[] option_buttons = null;
    public IEnumerator OnClick(){
        Vector3 origin_pos = this.context.transform.localPosition;
        while(true){
            while(timer>0f){
                timer-=Time.unscaledDeltaTime;
                yield return null;
            }
            if(parent!=null && this.context.interactable) {
                if(option_buttons==null) option_buttons = parent.options.GetComponentsInChildren<Button>();

                bool is_hovering_options = false;
                foreach (Button option in option_buttons){
                    if(IsHovering(option)){
                        is_hovering_options = true;
                        break;
                    }
                }
                if (IsHovering(this.context) && (!is_hovering_options)) {
                    if(Actions.Cursor.Click() > 0f){
                        if (parent.options_current != this.data) {
                            parent.options_current = this.data;
                        }

                        if (parent.options.IsVisible() && parent.options_current == this.data) parent.options.Hide();
                        else {
                            parent.options.Show();
                            if(parent.API_ACCESS!=null) {
                                if (parent.API_ACCESS.metadata.id != this.data.user_id) {
                                    Hide(option_buttons[0].gameObject);
                                    Hide(option_buttons[1].gameObject);
                                }
                                else {
                                    Show(option_buttons[0].gameObject);
                                    Show(option_buttons[1].gameObject);
                                }
                            }

                            parent.options_current = this.data;
                            parent.options.transform.position = Actions.Cursor.Position();
                        }
                        timer = 0.5f;
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    private static bool IsHovering(Button hoverable){
        if(!hoverable.IsActive()) return false;

        Vector3 cursor_pos = Actions.Cursor.Position();
        Vector2 object_size = (hoverable.GetComponent<RectTransform>().sizeDelta/2f);

        // corners
        float left = hoverable.transform.position.x-object_size.x;
        float right = hoverable.transform.position.x+object_size.x;

        float bottom = hoverable.transform.position.y-object_size.y;
        float top = hoverable.transform.position.y+object_size.y;

        return ((cursor_pos.x>=left && cursor_pos.x<=right) && (cursor_pos.y>=bottom && cursor_pos.y<=top));
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
        public string id;
        public string username;
        public string content;
        public long timestamp;
        public long last_update;
        public string user_id;
        public Data(string id, string username, string content, long timestamp, long last_update, string user_id){
            this.id = id;
            this.username = username;
            this.content = content;
            this.timestamp = timestamp;
            this.last_update = last_update;
            this.user_id = user_id;
        }

        public override string ToString(){
            return "{"+$"\"id\": \"{this.id}\", \"username\": \"{this.username}\", \"content\": \"{this.content}\", \"timestamp\": {this.timestamp}, \"last_update\": {this.last_update}, \"user_id\": \"{this.user_id}\""+"}";
        }
    }
}