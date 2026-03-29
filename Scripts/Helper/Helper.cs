using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        StartCoroutine(CanClick());
    }

    public override void SetUI(){
        this.canva = this.transform.GetChild(0).GetComponent<Canvas>();

        this.icon = this.canva.transform.GetChild(0).GetComponent<Button>();
        this.options = this.canva.transform.GetChild(1).GetComponent<Selector>();

        this.icon.onClick.AddListener(()=>{StartCoroutine(OnClick());});
    }

    public IEnumerator OnClick(){
        if(Window.Contains(typeof(Selector))) {
            Window.Clear<Selector>(this);
        }
        else {
            options.Show();
            options.transform.position = icon.transform.position + (Vector3.left*this.icon.GetComponent<RectTransform>().sizeDelta.x);
        }
        yield break;
    }

    private IEnumerator CanClick(){
        while(true) {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Actions.Cursor.Position();
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            #if !UNITY_EDITOR
            if(results.Count>0) Desktopia.Windows.Main.SetClickThrough(false);
            else {
                if(Actions.Cursor.Click()>0f) {
                    Window.Clear<Selector>(this);
                }
                Desktopia.Windows.Main.SetClickThrough(true);
            }
            #else
            if(results.Count<=0){
                if(Actions.Cursor.Click()>0f) Window.Clear<Selector>(this);
            }
            #endif

            yield return new WaitForEndOfFrame();
        }
    }

}