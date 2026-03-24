using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChopBoard : ItemPlaceAndTake
{
    [SerializeField] private float _chopTimeMax = 3f;
    [SerializeField] private Slider _progressBar;    
    [SerializeField] private GameObject _canvasObj;  

    private float _currentChopProgress = 0f;

    // 현재 다지기가 진행 중인지 나타내는 변수
    private bool _isChopping = false;

    private void Start()
    {
        if (_canvasObj != null) _canvasObj.SetActive(false);
    }

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

        if (_canvasObj != null) _canvasObj.SetActive(_isChopping);

        Debug.Log(_isChopping ? "다지기 시작!" : "다지기 일시정지");
    }

    private void Update()
    {
        if (_onCounterItem != null)
        {
            // 매 프레임 아이템이 낙하하지 않도록 물리 설정을 강제 고정
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

            if (_progressBar != null)
            {
                _progressBar.value = _currentChopProgress / _chopTimeMax;
            }


            if (_progressBar != null)
            {
                _progressBar.value = _currentChopProgress / _chopTimeMax;
            }

            if (_currentChopProgress >= _chopTimeMax)
            {
                FinishChop();
            }
        }
    
    }
    private void FinishChop()
    {
        Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();
        ingredient.AddStatus(CookBehaivior.chop);

        _currentChopProgress = 0f;
        _isChopping = false;

        if (_canvasObj != null) _canvasObj.SetActive(false);
    }

    // 아이템을 집어가면 다지기 중단 및 게이지 리셋
    public override GameObject TakeItem()
    {
        _isChopping = false;
        _currentChopProgress = 0f;
        if (_canvasObj != null) _canvasObj.SetActive(false);
        return base.TakeItem();
    }
}
