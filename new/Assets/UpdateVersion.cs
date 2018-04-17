using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class UpdateVersion : MonoBehaviour
{

    public string Spath, Lpath, version;


    // Use this for initialization
    void Start()
    {
        Lpath = @"C:\Users\kingsoft\Desktop\Local\server_version.txt";
        version = "1.1.0";
        WriteFile(ReadFiles(new DirectoryInfo(@"C:\Users\kingsoft\Desktop\Local\2")));
        Debug.Log("Yes");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<string[]> ReadFiles(DirectoryInfo directory)
    {
        List<string[]> ans = new List<string[]>();
        if (directory.Exists)
        {
            foreach (FileInfo info in directory.GetFiles())
            {
                string[] temp = new string[] { info.Name.ToString(),
                    info.Length.ToString(),
                    new MD5().PaseFile(info.FullName)
                };
                ans.Add(temp);
            }
        }
        return ans;
    }

    public void WriteFile(List<string[]> ans)
    {
        StreamWriter Sw = new StreamWriter(Lpath);
        Sw.WriteLine(version);
        foreach (string[] str in ans)
        {
            Sw.Write(str[0] + " ");
            Sw.Write(str[1] + " ");
            Sw.WriteLine(str[2]);
        }
        Sw.Write("END");
        Sw.Close();
    }


}