using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Path{
    public static class Prefabs{
        public static string Folder = "Prefabs/";
        public static string Button = Path.Prefabs.Folder+"Button";

        public static class Messaging{
            public static string Folder = Path.Prefabs.Folder+"Messaging/";
            public static string Message = Path.Prefabs.Messaging.Folder+"Message";
        }
        public static class Newsletter{
            public static string Folder = Path.Prefabs.Folder+"Newsletter/";
            public static string Article = Path.Prefabs.Newsletter.Folder+"Article";
        }

    }
    public static class Custom{
        public static string Folder = "./Apps/";
    }
}

public class Utils {
    public static object[] getFolder(string path){
        return Resources.LoadAll(path);
    }
    public static object getFile(string path){
        return Resources.Load(path);
    }
    public static GameObject getPrefab(string path){
        return (GameObject)Resources.Load(path, typeof(GameObject));
    }
    public static void LoadScene(string name){
        if(SceneManager.GetActiveScene().name == name) return;

        Debug.Log(name);
        SceneManager.LoadScene(name);
    }
    public static bool IsScene(string name){
        return (SceneManager.GetActiveScene().name == name);
    }

    public static void Rotate(GameObject target, Quaternion end_rotation, float speed = 0.2f){
        if(speed>0f) target.transform.localRotation = Quaternion.Lerp(target.transform.localRotation, end_rotation, Time.deltaTime/speed);
        else target.transform.localRotation = end_rotation;
    }
    public static void RotateX(GameObject target, float value, float speed = 0.2f){
        Quaternion end_rotation = Quaternion.Euler(value, target.transform.localRotation.eulerAngles.y, target.transform.localRotation.eulerAngles.z);
        Rotate(target, end_rotation, speed);
    }
    public static void RotateY(GameObject target, float value, float speed = 0.2f){
        Quaternion end_rotation = Quaternion.Euler(target.transform.localRotation.eulerAngles.x, value, target.transform.localRotation.eulerAngles.z);
        Rotate(target, end_rotation, speed);
    }
    public static void RotateZ(GameObject target, float value, float speed = 0.2f){
        Quaternion end_rotation = Quaternion.Euler(target.transform.localRotation.eulerAngles.x, target.transform.localRotation.eulerAngles.y, value);
        Rotate(target, end_rotation, speed);
    }

    public static bool IsIndexValid(IList array, int index){
        if(array==null) return false;
        return IsIndexValid(array.Count, index);
    }
    public static bool IsIndexValid(int max, int index){
        return (index >= 0 && index < max);
    }

    public static string TimestampToDate(long timestamp)
    {
        if(timestamp<0) return "";

        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds((double)timestamp).ToLocalTime();
        return dateTime.ToString("dd/mm/yyyy HH:mm:ss");
    }

    public static bool IsHovering(Button hoverable){
        if(!hoverable.IsActive()) return false;

        Vector3 cursor_pos = Actions.Cursor.Position();
        Vector2 object_size = (hoverable.GetComponent<RectTransform>().sizeDelta/2f);

        // corners
        float left = hoverable.transform.position.x-object_size.x;
        float right = hoverable.transform.position.x+object_size.x;

        float bottom = hoverable.transform.position.y-object_size.y;
        float top = hoverable.transform.position.y+object_size.y;

        return ((cursor_pos.x>=left && cursor_pos.x<=right) && (cursor_pos.y>=bottom && cursor_pos.y<=top));
    }
}