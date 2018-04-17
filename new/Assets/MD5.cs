using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MD5 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string PaseFile(string fileName)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        if (System.IO.File.Exists(filePath))
        {
            using (System.IO.FileStream stream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
            {
                string md5 = GetMd5FromStream(stream);
                return md5;
            }

        }
        return "";
    }

    public string GetMd5FromStream(FileStream stream)
    {
        byte[] buff;
        using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
        {
            buff = md5.ComputeHash(stream);

        }
        StringBuilder builder = new StringBuilder();
        foreach (var item in buff)
        {
            builder.Append(item.ToString("x2").ToLower());
        }
        string res = builder.ToString();
        return res;
    }
}
