using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Window : MonoBehaviour {
    public static HashSet<Window> active_windows = new HashSet<Window>();

    void Awake(){
        SetUI();
    }

    public bool IsVisible(){
        return this.gameObject.activeSelf;
    }

    public void Alternate(){
        if(this.gameObject.activeSelf) Hide();
        else Show();
    }
    public void Show(){
        this.gameObject.SetActive(true);
        active_windows.Add(this);
        Show_ExtendedBehaviour();
    }
    public virtual void Show_ExtendedBehaviour(){return;}

    public void Hide(){
        Hide_ExtendedBehaviour();
        active_windows.Remove(this);
        this.gameObject.SetActive(false);
    }
    public virtual void Hide_ExtendedBehaviour(){return;}


    public static void Show(GameObject obj){
        obj.SetActive(true);
    }
    public static void Show(params GameObject[] objects){
        foreach (GameObject obj in objects){
            Window.Show(obj);
        }
    }

    public static void Hide(GameObject obj){
        obj.SetActive(false);
    }
    public static void Hide(params GameObject[] objects){
        foreach (GameObject obj in objects){
            Window.Hide(obj);
        }
    }

    private bool isTyping = false;
    public bool IsTyping(){
        return this.isTyping;
    }

    private protected IEnumerator StartTyping(TMP_Text text, string content, float speed = 0f, bool isRealtime = false, Func<bool> stop_key = null){
        isTyping = true;
        text.text = "";
        yield return new WaitForSecondsRealtime(0.2f);

        if(check_stop_routine!=null){
            StopCoroutine(check_stop_routine);
            check_stop_routine = null;
        }
        if(stop_key!=null) check_stop_routine = StartCoroutine(CheckForStop(text, content, stop_key));

        if(content!=null) {
            if (speed > 0f && content.Length > 0) {
                float letter_typing_speed = speed / content.Length;

                int frames_to_skip = (int)((1f / 60f) / letter_typing_speed);
                int frames_skipped = 0;

                string typed_content = "";

                foreach (char c in content) {
                    if(has_been_stopped) {
                        yield return new WaitForSecondsRealtime(0.2f);
                        break;
                    }

                    typed_content += c;

                    if (frames_skipped < frames_to_skip && typed_content.Length != content.Length) frames_skipped++;
                    else {
                        text.text = typed_content;
                        float rnd_speed = UnityEngine.Random.Range(0.8f, 1.2f);

                        if (frames_to_skip > 0) {
                            frames_skipped = 0;
                            yield return new WaitForEndOfFrame();
                        }

                        yield return StartCoroutine(WaitFor(letter_typing_speed * rnd_speed, isRealtime));
                    }
                }
            }
            text.text = content;
        }
        yield return new WaitForSecondsRealtime(0.2f);
        isTyping = false;
    }

    private bool has_been_stopped = false;
    private Coroutine check_stop_routine = null;
    private IEnumerator CheckForStop(TMP_Text text, string content, Func<bool> stop_key){
        has_been_stopped = false;
        while (true){
            if(stop_key.Invoke()) {
                has_been_stopped = true;
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitFor(float time, bool isRealtime = false){
        while(time>0){
            if(has_been_stopped) yield break;

            time -= (isRealtime) ? Time.unscaledDeltaTime: Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    public virtual void SetUI(){}
}