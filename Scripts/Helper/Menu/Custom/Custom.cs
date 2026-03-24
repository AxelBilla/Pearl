using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class Custom : Window{

    [SerializeField] private string external_exe_path = "";
    private System.Diagnostics.Process current_process = null;

    private protected void Set(Custom.Data data){
        this.name = data.name;
        this.external_exe_path = data.path;
    }

    public static Custom.Data[] Get(){

        List<Custom.Data> custom_windows = new List<Data>();

        if(!Directory.Exists(Path.Custom.Folder)) Directory.CreateDirectory(Path.Custom.Folder);
        else{
            foreach (string file_name in Directory.GetFiles(Path.Custom.Folder)){
                string[] file = File.ReadAllLines(file_name);
                if(file.Length>2) continue;

                string name = Regex.Match(file[0], @"(?<=name\s*=\s*)(\S).*").ToString();
                string path = Regex.Match(file[1], @"(?<=path\s*=\s*)(\S).*").ToString();

                if(name=="" || path=="") continue;
                else custom_windows.Add(new Custom.Data(name, path));
            }
        }

        return custom_windows.ToArray();
    }

    public static void Load(Selector owner, Custom.Data[] data){
        foreach (Custom.Data entry in data) {
            GameObject button = Utils.getPrefab(Path.Prefabs.Button);
            //DestroyImmediate(button.GetComponent<Window>(), true);

            Custom custom_button = button.AddComponent<Custom>();
            custom_button.Set(entry);

            owner.windows.Add(custom_button);
        }
    }

    public override void Show_ExtendedBehaviour(){
        if(external_exe_path!=""){
            current_process = System.Diagnostics.Process.Start(external_exe_path);
        }
    }

    public override void Hide_ExtendedBehaviour(){
        if(current_process!=null){
            if(!current_process.HasExited) {
                current_process.Kill();
            }
            else Helper.Manager.OnClick();
            current_process = null;
        }
    }


    public class Data{
        public string name;
        public string path;

        public Data(string name, string path){
            this.name = name;
            this.path = path;
        }
    }
}