using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : MonoBehaviour
{
    [SerializeField] Transform foodPos;
    HashSet<IngreDientData> mix=new HashSet<IngreDientData>();
    public bool AddIngredient(IngreDientData ingredient)
    {
        if (mix.Contains(ingredient))
        {
            //이미 가지고 있음.
            return false;
        }
        HashSet<IngreDientData>nextMix=mix;
        nextMix.Add(ingredient);

        if (!RecipyTest.Instance.RecipyExistCk(nextMix))
        {
            // 존재하지 않는 조합이면 행동안함.
            return false;
        }

        // 진짜 조합함.
        mix.Add(ingredient);
        VisualModel();
        return true;
    }

    private void VisualModel()
    {
        //dish의 자식에 뭐가 있으면 삭제(모델 겹침 방지)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //food.position에 mix의 비트에 맞는 음식 조합 프리팹을 불러오기.. 
    }   
}
