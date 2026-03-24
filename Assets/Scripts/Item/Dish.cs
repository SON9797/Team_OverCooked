using Overcooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class Dish : MonoBehaviour
{
    [SerializeField] Transform foodPos;
    HashSet<IngreDientData> mix=new HashSet<IngreDientData>();
    private RecipeManager _recipeManager;

    [Inject] IObjectResolver _container;

    [Inject]
    public void Construct(RecipeManager recipeManager)
    {
        _recipeManager = recipeManager;
    }
    //해당 함수를 사용하면 접시에 매개변수의 재료를 추가한다.
    //만약 조합이 존재하지 않으면, 접시에 재료가 올라가지 않는다.
    public bool AddIngredient(Ingredient ingredient)
    {
        IngreDientData ingredientData = ingredient.GetIngredientData();
        
        if (mix.Contains(ingredientData))
        {
            //이미 가지고 있음.
            return false;
        }
        HashSet<IngreDientData> nextMix = new HashSet<IngreDientData>(mix);
        nextMix.Add(ingredientData);


        //레시피 불러와보기. (없으면 null 반환)
        GameObject recipyModel = _recipeManager.GetRecipyModel(nextMix); //여기서 레시피매니저를 불러와야됨;


        if (recipyModel==null)
        {
            // 존재하지 않는 조합이면 행동안함.
            print($"{string.Join(", ", nextMix)}는 없는 조합입니다");
            return false;

            
        }
        print("조합성공");
        // 진짜 조합함.
        mix.Add(ingredientData);
        
        //접시에 얹기
        VisualModel(recipyModel);

        //원래 모델 삭제
        Destroy(ingredient.gameObject);


        return true;
    }

    private void VisualModel(GameObject recipyModel)
    {
        //dish의 자식에 뭐가 있으면 삭제(모델 겹침 방지)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //food.position에 mix의 비트에 맞는 음식 조합 프리팹을 불러오기.. 
        GameObject model = Instantiate(recipyModel);
        model.transform.position = foodPos.position;


        //접시에 포지션 종속
        model.transform.SetParent(this.transform);
    }
   
}
