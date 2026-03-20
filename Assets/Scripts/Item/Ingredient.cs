using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Flags]
public enum CookBehaivior
{
    None = 0,
    chop = 1 << 0,
    roast = 1 << 1,
    boil = 1 << 2,

}
public class Ingredient : MonoBehaviour
{
    /*   순서가 중요하면 쓰려고 했던거.
    [Serializable]
    protected class StatusModelInfo
    {
        public Status from;
        public Status interect;
        public Status to;
        public GameObject result;

    }
    */

    
    [Serializable]
    public class StatusModelInfo
    {
        public CookBehaivior status;
        public GameObject obj;
    }




    [SerializeField]private List<StatusModelInfo> statusModelinfos;
    [SerializeField]private IngreDientKind kind;
    [SerializeField]private float cookDuration = 5f;

    private Dictionary<CookBehaivior, GameObject> statusModels = new Dictionary<CookBehaivior, GameObject>();

    CookBehaivior currentStat=CookBehaivior.None;

    private void Start()
    {
        for (int i = 0; i < statusModelinfos.Count; i++)
        {
            statusModels[statusModelinfos[i].status]=statusModelinfos[i].obj;
        }
    }

    
    /*
    private void DoCook(CookBehaivior addStat)
    {
        
    }
    IEnumerator CookingDelay(float duration,CookBehaivior addStat)
    {
        float accTime = 0;
        while (accTime < duration)
        {
            accTime += Time.deltaTime;
            // 대충 딜레이 연출
            // 대충 딜레이 연출
            yield return null;
        }
        AddStatus(addStat);

    }
    */
   

    public bool CanStatusAdd(CookBehaivior cook)
    {
        //이미 있는 상태면 안됨
        if ((cook & currentStat) == cook)
            return false;
        
        CookBehaivior nextStatus = currentStat | cook;

        //없는 상태면 안됨
        if (!statusModels.ContainsKey(nextStatus))
        {
            print($"{currentStat} to {nextStatus} is not founded");
            return false;
        }

        return true;
    }
    public void AddStatus(CookBehaivior cook)
    {
        CookBehaivior nextStatus = currentStat | cook;
        currentStat = nextStatus;

        //먼저 원래 있던 모델 삭제
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //status에 맞는 모델로 추가
        ModelUpdate();
        

    }
    private void ModelUpdate()
    {
        FoodModel model = Instantiate(statusModels[currentStat]).GetComponent<FoodModel>();
        model.GotoPosWithRoot(transform.position);
        model.transform.SetParent(transform);
    }

    public IngreDientData GetIngredientData()
    {
        IngreDientData data = new IngreDientData();
        data.kind = kind;
        data.stat = currentStat;
        return data;
    }
}
