using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CookingState
{
    Empty,      // 비어있음
    Cooking,    // 조리 중
    Done,       // 완성됨 (이때 접시에 담을 수 있음)
    Overheated, // 타기 직전 (경고 알람)
    OnFire      // 불남
}

public class CookingPot : ItemPlaceAndTake
{
    
    [SerializeField] private float _cookTime = 5f;      // 조리 완료 시간
    [SerializeField] private float _burnTime = 5f;      // 완료 후 불나기까지 시간

    private CookingState _currentState = CookingState.Empty;
    private float _timer = 0f;
    private Coroutine _cookCoroutine;

    public override bool PlaceItem(GameObject item)
    {
        // 1. 이미 조리 중이거나 불난 상태면 재료를 더 못 넣음
        if (_currentState != CookingState.Empty) return false;

        // 2. 넣은 아이템이 쌀(재료)인지 확인
        Ingredient ing = item.GetComponent<Ingredient>();
        if (ing != null && ing.GetIngredientData().kind == IngreDientKind.rice) // 쌀이라고 가정
        {
            base.PlaceItem(item);
            StartCooking();
            return true;
        }
        return false;
    }

    private void StartCooking()
    {
        _currentState = CookingState.Cooking;
        _cookCoroutine = StartCoroutine(Co_Cooking());
    }

    private IEnumerator Co_Cooking()
    {
        Debug.Log("조리 시작...");
        _timer = 0;

        // [조리 단계]
        while (_timer < _cookTime)
        {
            _timer += Time.deltaTime;
            // 여기서 게이지(UI) 업데이트 로직을 넣으면 좋습니다.
            yield return null;
        }

        // [완성 단계]
        _currentState = CookingState.Done;
        Debug.Log("밥 완성! 빨리 가져가세요.");
        ChangeModelColor(Color.green); // 시각적 피드백 (임시)

        // [과조리 대기 단계]
        _timer = 0;
        while (_timer < _burnTime)
        {
            _timer += Time.deltaTime;
            if (_timer > _burnTime * 0.7f) // 70% 진행 시 경고
            {
                _currentState = CookingState.Overheated;
                Debug.Log("삐-삐- 타기 일보직전!");
            }
            yield return null;
        }

        // [화재 단계]
        _currentState = CookingState.OnFire;
        Debug.Log("불이 났습니다! 소화기 가져오세요!");
        ChangeModelColor(Color.red);
    }

    public override GameObject TakeItem()
    {
        // 완성된 상태(Done)나 타기 직전(Overheated)에만 음식을 꺼낼 수 있게 설정
        if (_currentState == CookingState.Done || _currentState == CookingState.Overheated)
        {
            StopCoroutine(_cookCoroutine);
            _currentState = CookingState.Empty;
            // 실제로는 밥(Result) 오브젝트를 반환하거나 
            // 접시를 냄비에 대면 냄비가 비워지는 로직으로 확장해야 합니다.
            return base.TakeItem();
        }
        return null; // 조리 중이거나 불났을 땐 못 가져감
    }

    private void ChangeModelColor(Color col) // 상태 확인용 임시 코드
    {
        GetComponentInChildren<Renderer>().material.color = col;
    }
}
