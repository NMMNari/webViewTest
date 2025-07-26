using System.Collections;
using System.IO;
using UnityEngine;

public class WebViewLauncher : MonoBehaviour
{
    WebViewObject _webView;

    IEnumerator Start()
    {
        yield return StreamingAssetsCopier.CopyFolderToPersistent("WebInvaders");

        _webView = gameObject.AddComponent<WebViewObject>();
        _webView.Init(transparent: false, enableWKWebView: true,
            ld: msg => Debug.Log("Loaded: " + msg));

        _webView.SetMargins(0, 0, 0, 0);

#if UNITY_ANDROID && !UNITY_EDITOR
        // 旧バージョン互換：RequestFocus がある時だけ呼ぶ
        var m = _webView.GetType().GetMethod("RequestFocus");
        if (m != null) m.Invoke(_webView, null);
#endif

        string url =
#if UNITY_EDITOR
            Path.Combine(Application.streamingAssetsPath, "WebInvaders/invaders_game.html");
#else
            Path.Combine(Application.persistentDataPath,  "WebInvaders/invaders_game.html");
#endif
        _webView.LoadURL(new System.Uri(url).AbsoluteUri);
        _webView.SetVisibility(true);
        _webView.EvaluateJS("window.focus();");
    }

    void OnApplicationPause(bool paused) => _webView?.SetVisibility(!paused);
    void OnDestroy()                     => Destroy(_webView?.gameObject);
}