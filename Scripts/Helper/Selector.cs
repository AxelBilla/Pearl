
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Selector : Window {
    private static HashSet<Selector> current_selectors = new HashSet<Selector>();

    [SerializeField] List<Window> windows = new List<Window>();
    GameObject buttons;

    void Start(){
        Hide();
    }
    public override void SetUI(){
        this.buttons = this.transform.GetChild(0).gameObject;
        AddButtons();
    }
    private Vector3 button_size = default;
    private void AddButtons(){
        GameObject button_prefab = Utils.getPrefab(Path.Prefabs.Button);

        windows.Reverse();
        foreach (Window window in windows){
            Button new_button = Instantiate(button_prefab, this.buttons.transform).GetComponent<Button>();
            if(button_size==default) this.button_size = new_button.image.sprite.bounds.max*1000f;

            new_button.transform.localPosition += (Vector3.up*this.button_size.y)*((float)buttons.transform.childCount);

            new_button.GetComponentInChildren<TMP_Text>().text = window.name;
            new_button.onClick.AddListener(()=>{OnSelect(window);});
        }
    }

    private void OnSelect(Window window) {
        if(!current_selectors.Contains(this)) current_selectors.Add(this);
        if(window == this) return;

        window.Show();
        if (window is Selector) {
            current_selectors.Add((Selector)window);
            window.transform.position = this.transform.position + (Vector3.left * (button_size.x) * (float)current_selectors.Count);
        }
        else {
            foreach (Selector open_selector in current_selectors) open_selector.Hide();
            current_selectors.Clear();
        }
    }
}
