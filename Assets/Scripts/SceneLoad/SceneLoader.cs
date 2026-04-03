using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        //페이드 연출 코드 추가 가능

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
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
    IEnumerator LoadSceneAndData(string sceneName, IEnumerator dataLoad)
    {
        //페이드 연출 코드 추가 가능
        bool dataDone = false;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        StartCoroutine(Wrap(dataLoad, () => dataDone = true));

        while (op.progress < 0.9f && dataDone == false)
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
