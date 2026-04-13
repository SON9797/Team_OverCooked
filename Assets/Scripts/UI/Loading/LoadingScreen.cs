using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] Image progressBar;
    GameObject content;
    public void ProgressAdapt(float progress)
    {
        progressBar.fillAmount = progress;
    }
    public void ContentAdapt(GameObject content)
    {
        this.content = Instantiate(content, contentRoot);
    }
}
