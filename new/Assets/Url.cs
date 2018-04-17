using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Url : MonoBehaviour {

    public string Local_version, Server_version, Download_version;
    public string Local_path, Server_path, Temp_path;
    public string root = @"C:\Users\kingsoft\Desktop\Local\url.txt";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetUrl()
    {
        string[] ans = File.ReadAllLines(root);
        for (int i = 0; i < ans.Length - 1; i++)
        {
            int id = ans[i].IndexOf(' ');
            string name = new string(ans[i].ToCharArray(), 0, id);
            string url = new string(ans[i].ToCharArray(), id + 1, ans[i].Length - id - 1);
            switch (name)
            {
                case "Local_version":
                    Local_version = url;
                    break;
                case "Server_version":
                    Server_version = url;
                    break;
                case "Download_version":
                    Download_version = url;
                    break;
                case "Local_path":
                    Local_path = url;
                    break;
                case "Server_path":
                    Server_path = url;
                    break;
                case "Temp_path":
                    Temp_path = url;
                    break;
                default:
                    break;
            }
        }
    }
}
