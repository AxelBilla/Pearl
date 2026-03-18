using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Article : Window, IClickable {

    public Newsletter parent;

    TMP_Text category;
    TMP_Text timestamp;
    public TMP_Text content;

    public Button context;

    Article.Data data = null;
    public void Set(Article.Data data, Newsletter parent = null){
        this.data = data;
        Set(data.category, data.timestamp, data.content, data.sources, parent);
    }
    public void Set(string category, long timestamp, string content, Article.Data.Source[] sources, Newsletter parent = null){
        this.parent = parent;
        SetUI();

        this.category.text = category;
        this.timestamp.text = TimestampToDate(timestamp);
        this.content.text = content + ((sources.Length>0) ? "\n\n" : "");
        foreach (Article.Data.Source source in sources){
            this.content.text += "\n["+source.name+"]("+source.score+"): <color=#0000EE>"+source.link+"</color>";
        }

        this.content.ForceMeshUpdate();
        if(this.content.isTextOverflowing) {
            float height = this.content.textInfo.lineInfo[0].lineHeight*(this.content.textInfo.lineCount);
            this.context.image.rectTransform.sizeDelta = new Vector2(this.context.image.rectTransform.sizeDelta.x, height);
        }
    }

    public override void SetUI(){
        TMP_Text[] texts = this.GetComponentsInChildren<TMP_Text>();

        this.category = texts[0];
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
        public string content;
        public long timestamp;
        public string category;
        public Article.Data.Source[] sources;

        public Data(string id, string content, long timestamp, string category, Article.Data.Source[] sources){
            this.id = id;
            this.content = content;
            this.timestamp = timestamp;
            this.category = category;
            this.sources = sources;
        }

        public override string ToString(){
            string str = $"\"id\": \"{this.id}\", \"content\": \"{this.content}\", \"timestamp\": \"{this.timestamp}\", \"category\": \"{this.category}\", \"sources\": [";
            foreach (Article.Data.Source source in this.sources){
                str+=source.ToString()+",";
            }
            return "{"+str+"]}";
        }

        public class Source{
            public string name;
            public int score;
            public string link;
            public Source(string name, int score, string link){
                this.link = link;
                this.name = name;
                this.score = score;
            }
            public override string ToString(){
                return "{"+$"\"link\": \"{this.link}\", \"name\": \"{this.name}\", \"score\": {this.score}"+"}";
            }
        }
    }
}