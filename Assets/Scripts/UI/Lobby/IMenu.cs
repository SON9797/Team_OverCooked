using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenu
{
    int sceneindex { get; set; }
    void ClickMenu();
    void CloseMenue();  //뒤로가기는 씬을 이동하는거면 팝업을 추가로 띄우고 할지 말지 결정하게 해야한다.
}
