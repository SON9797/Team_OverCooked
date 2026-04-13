using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] LoadingScreen loadingscreenPrefab;
    [SerializeField] GameObject defaultContent;
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void LoadSceneAsync(string sceneName, GameObject content = null)
    {
        StartCoroutine(LoadRoutine(sceneName,content));
    }

    IEnumerator LoadRoutine(string sceneName, GameObject content = null)
    {
        //페이드 연출 코드 추가 가능
        float mintime = 1;
        float timer = 0;
        LoadingScreen loadingscreen=Instantiate(loadingscreenPrefab);
        if (content != null)
        {
            loadingscreen.ContentAdapt(content);
        }
        else
        {
            loadingscreen.ContentAdapt(defaultContent);
        }
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float displayed = 0f;
        while (op.progress< 0.9f|| timer<mintime)
        {
            
            float target = op.progress / 0.9f;
            displayed = Mathf.Lerp(displayed, target, Time.deltaTime * 5f);
            loadingscreen.ProgressAdapt(displayed);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        loadingscreen.ProgressAdapt(1);
        // 로딩 종료 후 씬 전환
        op.allowSceneActivation = true;

        // 씬 완전히 바뀔 때까지 대기
        while (!op.isDone)
        {
            yield return null;
        }

        //페이드 연출

    }
    IEnumerator LoadSceneAndData(string sceneName, IEnumerator dataLoad)
    {
        //페이드 연출 코드 추가 가능
        bool dataDone = false;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        StartCoroutine(Wrap(dataLoad, () => dataDone = true));

        while (!op.isDone || dataDone == false)
        {
            Debug.Log(op.progress); // 로딩 진행도
            yield return null;
        }


        // 로딩 종료 후 씬 전환
        op.allowSceneActivation = true;

        // 씬 완전히 바뀔 때까지 대기
        yield return null;

        //페이드 연출

    }
    IEnumerator Wrap(IEnumerator routine, System.Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
}
