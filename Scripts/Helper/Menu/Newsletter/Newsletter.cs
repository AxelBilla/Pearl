using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Newsletter : Menu {

    private Button config;
    private Window config_window;
    private TMP_InputField config_address;
    private TMP_InputField config_port;
    private TMP_InputField config_user;
    private TMP_InputField config_password;


    private ScrollRect article_tabs;

    public Window options;
    public Article.Data options_current;

    private Article.Data[] articles;
    [SerializeField] public API.Information API_ACCESS;

    void Start(){
        Hide();
    }

    public override async void SetUI(){
        Button[] buttons = this.GetComponentsInChildren<Button>();
        this.config = buttons[0];
        this.config.onClick.AddListener(()=>{this.config_window.Alternate();});

        this.exit = buttons[1];
        this.exit.onClick.AddListener(Hide);

        this.article_tabs = this.GetComponentInChildren<ScrollRect>();
        TMP_InputField[] inputs = this.GetComponentsInChildren<TMP_InputField>();

        this.drag_area = this.GetComponentInChildren<Image>();

        this.config_window = this.GetComponentsInChildren<Window>()[1];

        this.options = this.GetComponentsInChildren<Window>()[2];
        Button[] option_buttons = this.options.GetComponentsInChildren<Button>();
        option_buttons[0].onClick.AddListener(Copy);
        this.options.Hide();

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

        this.articles = await Request.Read.Articles(API_ACCESS);
    }

    private IEnumerator WaitForUpdate(){
        while(true){
            Task<Article.Data[]> req = Request.Read.Articles(API_ACCESS);
            while(!req.IsCompleted) yield return null;
            Article.Data[] newest = req.Result;

            if(newest!=null) {
                if (newest != articles) {
                    this.articles = newest;
                    yield return StartCoroutine(LoadTabs(true));
                }
            }


            yield return new WaitForSecondsRealtime(10f);
        }
    }

    private bool HasTabs(){
        if(this.article_tabs ==null) return false;
        return (this.article_tabs.content.childCount>0);
    }

    private void ClearTabs(){
        if(HasTabs()) {
            foreach (Transform child in this.article_tabs.content) {
                RemoveTab(child.gameObject, this.article_tabs);
            }
        }
    }

    private IEnumerator LoadTabs(bool reload = false){
        if(this.articles == null) yield break;
        float t = article_tabs.verticalNormalizedPosition;
        bool is_add = this.article_tabs.content.childCount<this.articles.Length;

        if(reload) ClearTabs();

        if(this.articles[0].timestamp>this.articles[this.articles.Length-1].timestamp) Array.Reverse(this.articles);
        foreach (Article.Data article in this.articles){
            AddTab(article, this.article_tabs);
        }
        yield return null;

        if(is_add && t<=0.01f) {
            yield return new WaitForEndOfFrame();
            article_tabs.verticalNormalizedPosition = (t);
        }
    }

    public int GetTabIndex(Article.Data tab){
        for(int i = 0; i<this.articles.Length; i++){
            if(this.articles[i] == tab) return i;
        }
        return -1;
    }

    private GameObject AddTab(Article.Data tab_article, ScrollRect tab_list){
        GameObject prefab = Utils.getPrefab(Path.Prefabs.Newsletter.Article);
        GameObject new_tab = Instantiate(prefab, this.article_tabs.content);

        // Set article data
        new_tab.GetComponent<Article>().Set(tab_article, this);
        return new_tab;
    }

    private void RemoveTab(GameObject tab, ScrollRect tab_list){
        for(int i = 0; i<this.article_tabs.content.childCount; i++){
            if(this.article_tabs.content.GetChild(i).gameObject == tab) RemoveTab(i, tab_list);
        }
    }

    public void RemoveTab(int index, ScrollRect tab_list){
        Destroy(tab_list.content.GetChild(index).gameObject);
    }


    public void Copy(){
        OnOptionsOpen();
        GUIUtility.systemCopyBuffer = options_current.content;
    }

    private void OnOptionsClose(){
        this.options_current = null;
    }
    private void OnOptionsOpen(){
        if(this.options.IsVisible()) this.options.Hide();
    }

    public override void Show_ExtendedBehaviour(){
        StartCoroutine(WaitForUpdate());
            StartCoroutine(Drag());
    }
    public override void Hide_ExtendedBehaviour(){
        this.config_window.Hide();
    }




    private static class Request {

        public static class Read {
            public static async Task<Article.Data[]> Articles(API.Information info) {
                string res = await API.Request.Get(info, "newsletter");
                Article.Data[] articles = JSON.Get<Article.Data[]>(res);
                foreach (Article.Data t in articles){
                    Debug.Log(t);
                }

                if(articles==null) articles = new Article.Data[]{new Article.Data("ERROR", res, -1, "",null)};
                else if(articles.Length==0) articles = new Article.Data[]{new Article.Data("", "Nothing to see here, yet...", -1, "",null)};

                return articles;
            }
        }

    }
}