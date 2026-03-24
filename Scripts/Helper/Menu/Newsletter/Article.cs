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
        this.timestamp.text = Utils.TimestampToDate(timestamp);
        this.content.text = content + ((sources.Length>0) ? "\n\n" : "");

        string link_hex = "BAE8FF";
        foreach (Article.Data.Source source in sources){
            this.content.text += $"\n[{source.name}]({source.score}): <color=#{link_hex}>{source.link}</color>";
        }

        this.content.ForceMeshUpdate();
        if(this.content.isTextOverflowing) {
            float height = this.content.textInfo.lineInfo[0].lineHeight*(this.content.textInfo.lineCount+1);
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
                    if(Utils.IsHovering(option.gameObject)){
                        is_hovering_options = true;
                        break;
                    }
                }
                if (Utils.IsHovering(this.context.gameObject) && (!is_hovering_options)) {
                    if(Actions.Cursor.RClick() > 0f){

                        bool is_same = (parent.options_current!=null && this.data!=null);
                        if(is_same) is_same = (parent.options_current.ToString()==this.data.ToString());

                        if (parent.options.IsVisible() && is_same) parent.options.Hide();
                        else {
                            parent.options.Show();
                            parent.options_current = this.data;
                            parent.options.transform.position = Actions.Cursor.Position();
                        }
                        timer = 2f;
                        yield break;
                    }
                }
            }
            yield return null;
        }
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