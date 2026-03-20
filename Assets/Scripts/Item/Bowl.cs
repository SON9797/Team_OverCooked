using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    [SerializeField]int maxCount = 2;
    List<Ingredient> items=new List<Ingredient>();
    
    public void AddIngredient(Ingredient ingredient)
    {
        // 재료를 보울에 추가
        if (maxCount >= items.Count)
            return;
        // 중복방지
        if (items.Contains(ingredient))
            return;
        items.Add(ingredient);
    }

    public void MixerOn()
    {
        // 대충 믹서기와 연동하는 코드
    }


}
