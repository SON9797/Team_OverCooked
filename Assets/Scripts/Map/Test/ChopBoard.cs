using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopBoard : ItemPlaceAndTake
{
    [SerializeField] private float _chopTimeMax = 3f;
    private float _currentChopProgress = 0f;

    // 현재 다지기가 진행 중인지 나타내는 상태 변수
    private bool _isChopping = false;

    // 플레이어가 컨트롤 키를 눌렀을 때 호출
    public void ToggleChop()
    {
        if (_onCounterItem == null)
        {
            _isChopping = false;
            return;
        }

        Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();
        if (ingredient == null || !ingredient.CanStatusAdd(CookBehaivior.chop))
        {
            _isChopping = false;
            return;
        }

        // 상태 반전
        _isChopping = !_isChopping;
        Debug.Log(_isChopping ? "다지기 시작!" : "다지기 일시정지");
    }

    private void Update()
    {
        if (_onCounterItem != null)
        {
            // [추가] 매 프레임 아이템이 낙하하지 않도록 물리 설정을 강제 고정
            if (_onCounterItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.useGravity = false;   // 중력 끄기
                rb.isKinematic = true;  // 물리 연산 중단 (고정)
            }
        }

        if (!_isChopping || _onCounterItem == null)
        {
            return;
        }

        if (_isChopping && _onCounterItem != null)
        {
            Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();

            // (중복 실행 방지)
            if (!ingredient.CanStatusAdd(CookBehaivior.chop))
            {
                _isChopping = false;
                _currentChopProgress = 0f;
                return;
            }

            _currentChopProgress += Time.deltaTime;

            if (_currentChopProgress >= _chopTimeMax)
            {
                // 상태 변경
                ingredient.AddStatus(CookBehaivior.chop);

                // 즉시 모든 변수 초기화
                _currentChopProgress = 0f;
                _isChopping = false;

                Debug.Log("다지기 완료 및 정지");
            }
        }
    }

    // 아이템을 집어가면 무조건 다지기 중단 및 게이지 리셋
    public override GameObject TakeItem()
    {
        _isChopping = false;
        _currentChopProgress = 0f;
        return base.TakeItem();
    }
}
