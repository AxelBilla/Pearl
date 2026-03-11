using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Messaging : Menu {

    private Button config;
    private ScrollRect message_tabs;
    private TMP_InputField input;
    private Button send;

    private Message.Data[] messages;

    public override void SetUI(){
        Button[] buttons = this.GetComponentsInChildren<Button>();
        this.config = buttons[0];
        this.exit = buttons[1];
        this.exit.onClick.AddListener(Hide);

        this.send = buttons[2];
        this.send.onClick.AddListener(Send);

        this.message_tabs = this.GetComponentInChildren<ScrollRect>();
        this.input = this.GetComponentInChildren<TMP_InputField>();

        this.messages = API.Read.Messages();
        LoadTabs();

        this.drag_area = this.GetComponentInChildren<Image>();
    }

    private IEnumerator WaitForUpdate(){
        while(true){
            Message.Data[] newest = API.Read.Messages();
            if(messages==null) messages = newest;

            if(messages!=null) {
                if (newest.Length != messages.Length) {
                    int diff = newest.Length - messages.Length;

                    if (diff > 0) {
                        for (int i = newest.Length - diff; i < newest.Length; i++)  AddTab(newest[i], this.message_tabs);
                    }
                    else {
                        for (int i = 0; i < newest.Length; i++) {
                            if (newest[i] != messages[i]) RemoveTab(this.message_tabs.transform.GetChild(i).gameObject, this.message_tabs);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private bool HasTabs(){
        if(this.message_tabs ==null) return false;
        return (this.message_tabs.content.childCount>0);
    }

    private void LoadTabs(bool reload = false){
        if(this.messages == null) return;

        if(HasTabs()) {
            if(!reload) return;
            else {
                foreach (Transform child in this.message_tabs.content) {
                    Destroy(child.gameObject);
                }
            }
        }

        foreach (Message.Data message in this.messages){
            AddTab(message, this.message_tabs);
        }
    }
    public int GetTabIndex(Message.Data tab){
        for(int i = 0; i<this.messages.Length; i++){
            if(this.messages[i] == tab) return i;
        }
        return -1;
    }

    private void AddTab(Message.Data tab_message, ScrollRect tab_list){
        GameObject prefab = Utils.getPrefab(Path.Prefabs.Messaging.Message);
        Button new_tab = Instantiate(prefab, tab_list.content).GetComponent<Button>();

        int count = tab_list.content.childCount;
        float spacing = 0f;
        float button_space = (new_tab.GetComponent<RectTransform>().rect.height*2)+spacing;

        RectTransform tab_list_rect = tab_list.content.GetComponent<RectTransform>();
        tab_list_rect.sizeDelta = new Vector2(tab_list_rect.sizeDelta.x, tab_list_rect.sizeDelta.y + (button_space));
        tab_list_rect.anchoredPosition = new Vector3(tab_list_rect.anchoredPosition.x, tab_list_rect.anchoredPosition.y - (button_space));

        // Set message data
        new_tab.GetComponent<Message>().Set(tab_message);
        new_tab.transform.localPosition = new Vector3(new_tab.transform.localPosition.x, -(button_space/2), new_tab.transform.localPosition.z);
    }

    private void RemoveTab(GameObject tab, ScrollRect tab_list){
        int count = tab_list.content.childCount;
        float spacing = 0f;
        float button_space = (tab.GetComponent<RectTransform>().rect.height*2)+spacing;

        Destroy(tab);

        // Update following tab's position to account for the removed tab
        for(int i = tab.transform.GetSiblingIndex(); i<tab_list.content.childCount; i++){
            Transform following_tab = tab_list.content.GetChild(i);
            following_tab.localPosition = new Vector3(following_tab.localPosition.x, following_tab.localPosition.y-(button_space/2), following_tab.localPosition.z);
        }

        RectTransform tab_list_rect = tab_list.content.GetComponent<RectTransform>();
        tab_list_rect.sizeDelta = new Vector2(tab_list_rect.sizeDelta.x, tab_list_rect.sizeDelta.y - (button_space));
        tab_list_rect.anchoredPosition = new Vector3(tab_list_rect.anchoredPosition.x, tab_list_rect.anchoredPosition.y + (button_space));
    }

    private void Send(){
        Add(this.input.text);
        this.input.text = "";
    }

    private void Add(string text){
        long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        Message.Data new_message = new Message.Data(API.user, timestamp, text);

        AddTab(new_message, this.message_tabs);
        API.Create.Message(new_message);
    }

    public override void Show_ExtendedBehaviour(){
        StartCoroutine(WaitForUpdate());
        StartCoroutine(Drag());
    }


}