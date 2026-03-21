
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : Window {
    private static HashSet<Selector> current_selectors = new HashSet<Selector>();

    [SerializeField] public List<Window> windows = new List<Window>();
    [SerializeField] private bool showCustoms = false;

    [HideInInspector] public GameObject buttons;

    void Start(){
        Hide();
    }
    public override void SetUI(){
        this.buttons = this.transform.GetChild(0).gameObject;
        AddButtons();
    }
    private void AddButtons(){
        GameObject button_prefab = Utils.getPrefab(Path.Prefabs.Button);

        windows.Reverse();
        if(showCustoms) Custom.Load(this, Custom.Get());

        foreach (Window window in windows){
            Button new_button = Instantiate(button_prefab, this.buttons.transform).GetComponent<Button>();
            Vector2 button_size = new_button.GetComponent<RectTransform>().sizeDelta;

            new_button.transform.localPosition += (Vector3.up*button_size.y)*((float)buttons.transform.childCount);

            new_button.GetComponentInChildren<TMP_Text>().text = window.name;
            new_button.onClick.AddListener(()=>{OnSelect(window, new_button);});
        }
    }

    private void OnSelect(Window window, Button button) {
        RemoveFollowingOpenSelectors();
        if(!current_selectors.Contains(this)) current_selectors.Add(this);
        if(window == this) return;

        window.Show();
        if (window is Selector) {
            current_selectors.Add((Selector)window);
            Vector2 button_size = button.GetComponent<RectTransform>().sizeDelta;

            window.transform.position = this.transform.position + (Vector3.left * (button_size.x));
            window.transform.position = new Vector3(window.transform.position.x, window.transform.position.y+(button_size.y*button.transform.GetSiblingIndex()), window.transform.position.z);
        }
        else {
            foreach (Selector open_selector in current_selectors) open_selector.Hide();
            current_selectors.Clear();
        }
    }

    private void RemoveFollowingOpenSelectors(){
        bool has_reached_this = false;
        foreach(Selector s in current_selectors){
            if(has_reached_this) s.Hide();
            if(s==this) has_reached_this = true;
        }
    }
}
