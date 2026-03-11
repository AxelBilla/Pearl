using UnityEngine;
using UnityEngine.InputSystem;

public static class Actions
{
    public static class Cursor{
        public static Vector3 Position(){ return Mouse.current.position.ReadValue();}
        public static Vector2 Move() {
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.Player).FindAction(Actions.Values.Cursor.Move).ReadValue<Vector2>();
        }
        public static float Click() {
            return InputSystem.actions.FindActionMap(Actions.Values.Maps.UI).FindAction(Actions.Values.Cursor.Click).ReadValue<float>();
        }
    }

    public static class Values{
        public static class Cursor{
            public static string Move = "Look";
            public static string Click = "Click";
        }

        public static class Maps{
            public static string Player = "Player";
            public static string UI = "UI";
        }
    }
}
