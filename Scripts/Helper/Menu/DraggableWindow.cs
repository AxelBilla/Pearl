using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DraggableWindow : Window {
    private protected Image drag_area;

    private protected virtual IEnumerator Drag(){
        bool is_moving = false;
        while(true){
            float is_holding = Actions.Cursor.Click();
            if(!is_moving && is_holding>0f && IsHovering(drag_area)) is_moving = true;
            if(is_moving) {
                if (is_holding > 0f) {
                    Vector2 move = Actions.Cursor.Move();//*Time.unscaledDeltaTime;
                    this.transform.position += (Vector3.right * move.x) + (Vector3.up * move.y);
                }
                else is_moving = false;
            }
            yield return null;
        }
    }
    private protected static bool IsHovering(Image hoverable){
        Vector3 cursor_pos = Actions.Cursor.Position();
        Vector3 object_size = (hoverable.sprite.bounds.extents/2f)*1000f;

        // corners
        float left = hoverable.transform.position.x-object_size.x;
        float right = hoverable.transform.position.x+object_size.x;

        float bottom = hoverable.transform.position.y-object_size.y;
        float top = hoverable.transform.position.y+object_size.y;

        return ((cursor_pos.x>=left && cursor_pos.x<=right) && (cursor_pos.y>=bottom && cursor_pos.y<=top));
    }
}
