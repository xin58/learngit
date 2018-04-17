using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BestHTTP;
using System.IO;

public class DownLoad : MonoBehaviour {

    public string Spath, Lpath, OldPath;
    public int downloadlength, downloadedBytes, Cnt = 0;
    public List<byte[]> downloadedFragments;
    public long now = 0, temp = 0;
    public bool Begin = false;
    public bool Download_now = false;

    public DownLoad()
    {

    }

    public DownLoad(string Spath, string Lpath, bool Download_now)
    {
        this.Spath = Spath;
        this.Lpath = Lpath;
        this.Download_now = Download_now;
    }

    public DownLoad(string Spath, string Lpath, string OldPath)
    {
        this.Spath = Spath;
        this.Lpath = Lpath;
        this.OldPath = OldPath;
    }

    public void StartDownload()
    {
        Debug.Log("Yes");
        var request = new HTTPRequest(new Uri(Spath), OnfragmentDownloaded);
        request.OnProgress = OnDownloadProgress;
        request.UseStreaming = true;
        request.StreamFragmentSize = 1024;
        HTTPManager.SendRequest(request);
    }

    void OnDownloadProgress(HTTPRequest request, int downloaded, int length)
    {
        downloadedBytes = downloaded;
        downloadlength = length;
    }

    void OnfragmentDownloaded(HTTPRequest req, HTTPResponse resp)
    {
        if (resp == null)
        {
            ++Cnt;
            if(Cnt < 3)
            {
                StartDownload();
            } else
                Debug.LogError("Download Failed!");
            return;
        }
        downloadedFragments = resp.GetStreamedFragments();
        if (downloadedFragments != null)
        {
            if (Download_now)
                download_now(downloadedFragments);
            Begin = true;
        }
        if (resp.IsStreamingFinished)
            Debug.Log("Download Finished!");
    }

    //同步下载
    public void download_now(List<byte[]> buffer)
    {
        FileStream fileStream = new FileStream(Lpath, FileMode.Create, FileAccess.Write);
        while (buffer.Count > 0)
        {
            fileStream.Write(buffer[0], 0, buffer[0].Length);
            now += buffer[0].Length;
            temp += buffer[0].Length;
            buffer.RemoveAt(0);
        }
        fileStream.Close();
    }

    //异步下载
    public IEnumerator download(List<byte[]> buffer)
    {
        FileStream fileStream = new FileStream(Lpath, FileMode.Create, FileAccess.Write);
        while (buffer.Count > 0)
        {
            fileStream.Write(buffer[0], 0, buffer[0].Length);
            now += buffer[0].Length;
            temp += buffer[0].Length;
            buffer.RemoveAt(0);
            yield return false;
        }
        fileStream.Close();
        //文件覆盖
        if (File.Exists(OldPath))
        {
            File.Copy(Lpath, OldPath, true);
            File.Delete(Lpath);
        } 
        else
            File.Move(Lpath, OldPath);
        Debug.Log("FragmentDownLoad ok");
        yield return false;
    }
}

