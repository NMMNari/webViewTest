using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class StreamingAssetsCopier
{
    public static IEnumerator CopyFolderToPersistent(string folderName)
    {
#if UNITY_EDITOR
        yield break;   // Editor ではコピー不要（StreamingAssets を直読み）
#else
        string srcRoot = Path.Combine(Application.streamingAssetsPath, folderName);
        string dstRoot = Path.Combine(Application.persistentDataPath, folderName);
        Directory.CreateDirectory(dstRoot);

        string[] files = { "invaders_game.html" /* , "style.css", "main.js" ... */ };

        foreach (var f in files)
        {
            string dst = Path.Combine(dstRoot, f);
            if (File.Exists(dst)) continue;      // ファイル単位で存在チェック

            string src = Path.Combine(srcRoot, f);
#if UNITY_ANDROID
            using var www = UnityWebRequest.Get(src);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError(www.error);
            else
                File.WriteAllBytes(dst, www.downloadHandler.data);
#else
            File.Copy(src, dst, true);
            yield return null;
#endif
        }
#endif
    }
}