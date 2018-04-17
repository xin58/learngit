using System.Collections;
using System.Collections.Generic;
using System;
using BestHTTP;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;
using System.Text;

public class Index : MonoBehaviour
{
    //UI
    public const float BarLength = 802;
    public static Transform Bar;

    int Cnt = 0;
    long sum = 0, now = 0;
    bool First = false, Ok = false;
    List<string> ToDownload = new List<string>();

    DownLoad DL, Sd;
    Url URL;
    // Use this for initialization
    void Start()
    {
        URL = new Url();
        URL.GetUrl();
        //创建临时文件夹
        Directory.CreateDirectory(URL.Temp_path);
        Bar = GetComponent<Transform>();

        //获取服务器version文件
        Sd = new DownLoad(URL.Server_version, URL.Download_version, true);
        Sd.StartDownload();
        CompareVersion();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Ok)
        {
            bool flag = CompareVersion();
            if (flag)
            {
                Bar.Translate(Vector3.right * BarLength, Space.Self);
                DL = new DownLoad();
                GameObject.Find("Canvas/Panel/ProgressBar/Text").GetComponent<Text>().text = "100.0%";
                File.Delete(URL.Download_version);
            }
            return;
        }
        if (DL.Begin && !First)
        {
            //获取到流数据，异步下载数据
            StartCoroutine(DL.download(DL.downloadedFragments));
            DL.Begin = false;
            First = true;
        }
        Debug.Log("now : " + now + "  sum : " + sum);
        Debug.Log("DL.now : " + DL.now + "  downloadlength : " + DL.downloadlength);
        if (DL.temp != 0)
        {
            //更新UI
            float p = DL.temp / (float)sum;
            Bar.Translate(Vector3.right * p * BarLength, Space.Self);
            now += DL.temp;
            p = 100f * now / (float)sum;
            GameObject.Find("Canvas/Panel/ProgressBar/Text").GetComponent<Text>().text = p.ToString("f1") + "%";
            DL.temp = 0;
        }
        else if(DL.now == DL.downloadlength)
        {
            if (Cnt + 1 < ToDownload.Count && DL.now != 0)
            {
                Debug.Log("Cnt : " + Cnt);
                //继续下载剩余的文件
                ++Cnt;
                DL = new DownLoad(URL.Server_path + ToDownload[Cnt], URL.Temp_path + ToDownload[Cnt], URL.Local_path + ToDownload[Cnt]);
                DL.StartDownload();
                First = false;
            } else if (Cnt + 1 == ToDownload.Count)
            {
                Debug.Log("Finish");
                //更新本地version文件
                Cnt++;
                File.Copy(URL.Download_version, URL.Local_version, true);
                File.Delete(URL.Download_version);
            }
        }
    }

    //对比version文件，获取需要更新的资源
    bool CompareVersion()  
    {
        if(Sd.Begin)
        {
            Ok = true;
            string[] Local_line = File.ReadAllLines(URL.Local_version);
            string[] Server_line = File.ReadAllLines(URL.Download_version);
            if (Local_line[0] == Server_line[0])
            {
                return true;
            }
            for (int i = 1; i < Server_line.Length - 1; i++)
            {
                bool flag = true;
                int id1 = Server_line[i].IndexOf(' ');
                string u1 = new string(Server_line[i].ToCharArray(), 0, id1);
                int id2 = Server_line[i].IndexOf(' ', id1 + 1);
                string u2 = new string(Server_line[i].ToCharArray(), id1 + 1, id2 - id1 - 1);
                string u3 = new string(Server_line[i].ToCharArray(), id2 + 1, Server_line[i].Length - id2 - 1);
                for (int j = 1; j < Local_line.Length - 1; j++)
                {
                    id1 = Local_line[j].IndexOf(' ');
                    string v1 = new string(Local_line[j].ToCharArray(), 0, id1);
                    if (u1 == v1)
                    {
                        id1 = Local_line[j].IndexOf(' ', id1 + 1);
                        string v3 = new string(Local_line[j].ToCharArray(), id1 + 1, Local_line[j].Length - id1 - 1);
                        if (u3 == v3)
                        {
                            flag = false;
                        }
                        break;
                    }
                }
                if (flag)
                {
                    ToDownload.Add(u1);
                    int temp = 0;
                    int.TryParse(u2, out temp);
                    sum += temp;
                }
            }
            if (ToDownload.Count != 0)
            {
                DL = new DownLoad(URL.Server_path + ToDownload[0], URL.Temp_path + ToDownload[0], URL.Local_path + ToDownload[0]);
                DL.StartDownload();
                return false;
            }
            return true;
        }
        return false;
    }
}