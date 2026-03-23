using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IngreDientKind
{
    lettuce,
    fish,
    carrot,
    parwn
}
[Serializable]
public struct IngreDientData
{
    public IngreDientKind kind;
    public CookBehaivior stat;
}
[Serializable]
public struct RecipyData
{
    public List<IngreDientData> ingredients;
    public GameObject model;
    public int score;


}
class RecipyTest : MonoBehaviour
{
    public static RecipyTest Instance;
    
    //[SerializeField] List<IngreDientData> data;
    [SerializeField] List<RecipyData> recipyData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        for (int i=0;i<recipyData.Count; i++)
        {
            RecipyData temp = recipyData[i];
            temp.score = temp.ingredients.Count;
            recipyData[i] = temp;
        }
        
    }

    //레시피에 해당하는 모델 반환
    public GameObject GetRecipyModel(HashSet<IngreDientData> mix)
    {
        foreach(RecipyData r in recipyData)
        {
            if (mix.SetEquals(r.ingredients) && mix.Count==r.ingredients.Count)
            {
                return r.model;
            }
        }
        return null;
    }
    


}