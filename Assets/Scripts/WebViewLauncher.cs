using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebViewLauncher : MonoBehaviour
{
    /* 外部公開している HTML の RAW URL */
    const string RemoteHtmlUrl = "https://raw.githubusercontent.com/NMMNari/html_testgame1/main/invaders_game.html";

    WebViewObject _webView;

    IEnumerator Start()
    {
        /* ① HTML をダウンロード */
        string html = null;
        using (UnityWebRequest www = UnityWebRequest.Get(RemoteHtmlUrl))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[WebInvaders] HTML download failed: " + www.error);
                yield break;   // 失敗時にローカルへフォールバックしたい場合はここで呼ぶ
            }
            html = www.downloadHandler.text;
        }

        /* ② WebView を生成 */
        _webView = gameObject.AddComponent<WebViewObject>();
        _webView.Init(
            transparent: false,
            enableWKWebView: true,
            ld: msg => Debug.Log("Loaded (external): " + msg)
        );
        _webView.SetMargins(0, 0, 0, 0);

#if UNITY_ANDROID && !UNITY_EDITOR
        // 旧バージョンとの互換を考慮
        var m = _webView.GetType().GetMethod("RequestFocus");
        if (m != null) m.Invoke(_webView, null);
#endif

        /* ③ 文字列で HTML を流し込む */
        // 第2引数 baseUrl を渡しておくと、HTML 内の相対パス解決や LocalStorage が機能する
        _webView.LoadHTML(html, RemoteHtmlUrl.Substring(0, RemoteHtmlUrl.LastIndexOf('/') + 1));

        _webView.SetVisibility(true);
        _webView.EvaluateJS("window.focus();");
    }

    void OnApplicationPause(bool paused) => _webView?.SetVisibility(!paused);
    void OnDestroy()                     => Destroy(_webView?.gameObject);
}