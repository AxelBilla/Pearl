using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Path{
    public static class Prefabs{
        public static string Folder = "Prefabs/";
        public static string Button = Path.Prefabs.Folder+"Button";

        public static class Messaging{
            public static string Folder = Path.Prefabs.Folder+"Messaging/";
            public static string Message = Path.Prefabs.Messaging.Folder+"Message";
        }

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

    public static bool isFacing(Transform obj, Transform target, float angle = 0.7f)
    {
        Vector3 forwardPos = obj.forward;
        Vector3 distanceToTarget = (target.position - obj.position).normalized;

        return (Vector3.Dot(forwardPos, distanceToTarget) >= angle); // returns True or False, depending on if the target is in front or not
    }

    public static float GetDistance(Vector3 start, Vector3 end){
        Vector3 distance = (start-end);
        float result = (distance.x>0) ? distance.x : -distance.x;
        result += (distance.y>0) ? distance.y : -distance.y;
        result += (distance.z>0) ? distance.z : -distance.z;

        return result;
    }

    public static bool IsIndexValid(IList array, int index){
        if(array==null) return false;
        return IsIndexValid(array.Count, index);
    }
    public static bool IsIndexValid(int max, int index){
        return (index >= 0 && index < max);
    }
}