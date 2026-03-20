using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum IngreDientKind
{
    lettuce,
    carrot,
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
        
    }

    public bool RecipyExistCk(HashSet<IngreDientData> mix)
    {
        foreach(RecipyData r in recipyData)
        {
            if (mix.SetEquals(r.ingredients) && mix.Count==r.ingredients.Count)
            {
                return true;
            }
        }
        return false;
    }


}