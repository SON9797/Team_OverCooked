using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
   public void Onclick()
    {
        SaveLoad.instance.ContinueGame();
        MenuManager.instance.worldMap.OpenWindow();
    }
}
