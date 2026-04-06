using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] Image progressBar;
    public void ShowContent()
    {

    }
    public void ProgressAdapt(float progress)
    {
        progressBar.fillAmount = progress;
    }
}
