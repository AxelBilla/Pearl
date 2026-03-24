using UnityEngine;
using UnityEngine.InputSystem;

public static class Actions
{
    public static class Cursor{
        public static Vector3 Position(){
            #if !UNITY_EDITOR
            Vector2 pos = Desktopia.Cursor.Position;
            return new Vector3(pos.x, Screen.height-pos.y, 0f);
            #else
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.UI).FindAction(Actions.Values.Cursor.Position).ReadValue<Vector2>();
            #endif
        }
        public static Vector2 Move() {
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.Player).FindAction(Actions.Values.Cursor.Move).ReadValue<Vector2>();
        }
        public static float Click() {
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.UI).FindAction(Actions.Values.Cursor.Click).ReadValue<float>();
        }
        public static float RClick() {
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.UI).FindAction(Actions.Values.Cursor.RClick).ReadValue<float>();
        }
    }

    public static class Values{
        public static class Cursor{
            public static string Move = "Look";
            public static string Click = "Click";
            public static string RClick = "RightClick";
            public static string Position = "Position";
        }

        public static class Maps{
            public static string Player = "Player";
            public static string UI = "UI";
        }
    }
}
