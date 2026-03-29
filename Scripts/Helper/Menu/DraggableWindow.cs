using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DraggableWindow : Window {
    private protected Image drag_area;

    private protected virtual IEnumerator Drag(){
        bool is_moving = false;
        while(true){
            bool is_holding = Actions.Cursor.Click() > 0f;
            if(!is_moving && is_holding && Utils.IsHovering(drag_area.gameObject)) {
                if(DraggableWindow.current == null) {
                    DraggableWindow.current = this;
                    is_moving = true;
                }
            }

            if(is_moving) {
                if (is_holding) {
                    Vector2 move = Actions.Cursor.Move();//*Time.unscaledDeltaTime;
                    this.transform.position += (Vector3.right * move.x) + (Vector3.up * move.y);
                }
                else {
                    is_moving = false;
                    DraggableWindow.current = null;
                }
            }
            yield return null;
        }
    }

    void OnDisable(){
        if(current==this) current = null;
    }

    private static DraggableWindow current = null;
}
