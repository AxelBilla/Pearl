using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : Window, IClickable {
    // Singleton
    public static Helper Manager = null;
    void Awake(){
        if(Helper.Manager==null) Helper.Manager = this;
        else Destroy(this);
    }


    private Canvas canva;
    private Button icon;
    private Selector options;

    void Start(){
        SetUI();

        options.Hide();
    }

    public override void SetUI(){
        this.canva = this.transform.GetChild(0).GetComponent<Canvas>();

        this.icon = this.canva.transform.GetChild(0).GetComponent<Button>();
        this.options = this.canva.transform.GetChild(1).GetComponent<Selector>();

        this.icon.onClick.AddListener(()=>{OnClick();});
    }

    public void OnClick(){
        if(Window.active_windows.Count>1 && Window.active_windows.Contains(this)) {
            Window[] windows = new Window[Window.active_windows.Count];
            Window.active_windows.CopyTo(windows);
            foreach (Window window in windows) {
                if(window!=this) window.Hide();
            }
        }
        else {
            options.Show();
            options.transform.position = icon.transform.position + (Vector3.left*this.icon.image.sprite.bounds.max.x*1000f*2f);
        }
    }

}