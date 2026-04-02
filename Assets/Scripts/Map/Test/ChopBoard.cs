using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Overcooked;

public class ChopBoard : ItemPlaceAndTake
{
    [SerializeField] private float _chopTimeMax = 3f;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private GameObject _canvasObj;

    [Header("칼질 유지 판정")]
    [SerializeField] private float _pauseCheckDistance = 2.5f;
    [SerializeField, Range(-1f, 1f)] private float _pauseCheckDot = 0.5f;

    private float _currentChopProgress = 0f;

    // 현재 다지기가 진행 중인지 나타내는 변수
    private bool _isChopping = false;

    // 현재 칼질 중인 플레이어
    private PlayerItemController _currentChoppingPlayer;

    protected override void Start()
    {
        base.Start();

        if (_canvasObj != null) _canvasObj.SetActive(false);

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_onCounterItem != null)
        {
            // 매 프레임 아이템이 낙하하지 않도록 물리 설정을 강제 고정
            if (_onCounterItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.useGravity = false;   // 중력 끄기
                rb.isKinematic = true;   // 물리 연산 중단 (고정)
            }
        }

        if (!_isChopping || _onCounterItem == null)
        {
            return;
        }

        // 칼질 중인데 플레이어가 더 이상 도마를 바라보지 않으면 일시정지
        if (!CanKeepChopping())
        {
            PauseChopping();
            return;
        }

        Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();

        // (중복 실행 방지)
        if (ingredient == null || !ingredient.CanStatusAdd(CookBehaivior.chop))
        {
            StopChopping();
            return;
        }

        _currentChopProgress += Time.deltaTime;

        if (_progressBar != null)
        {
            _progressBar.value = _currentChopProgress / _chopTimeMax;
        }

        if (_currentChopProgress >= _chopTimeMax)
        {
            FinishChop();
        }
    }

    // 플레이어가 컨트롤 키를 눌렀을 때 호출
    public bool ToggleChop(PlayerItemController player)
    {
        if (_onCounterItem == null)
        {
            StopChopping();
            return false;
        }

        Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();
        if (ingredient == null || !ingredient.CanStatusAdd(CookBehaivior.chop))
        {
            StopChopping();
            return false;
        }

        if (player == null)
        {
            PauseChopping();
            return false;
        }

        // 이미 칼질 중이면 토글 off(일시정지)
        if (_isChopping)
        {
            PauseChopping();
            return false;
        }

        // 칼질 시작 / 재개
        _currentChoppingPlayer = player;
        _isChopping = true;

        if (_canvasObj != null) _canvasObj.SetActive(true);

        Debug.Log("다지기 시작!");
        return true;
    }

    private bool CanKeepChopping()
    {
        if (_currentChoppingPlayer == null)
        {
            return false;
        }

        if (!_currentChoppingPlayer.IsSelectedPlayer())
        {
            return false;
        }

        Transform rayPoint = _currentChoppingPlayer.GetRayPoint();
        if (rayPoint == null)
        {
            return false;
        }

        Vector3 toBoard = transform.position - rayPoint.position;
        toBoard.y = 0f;

        float sqrDistance = toBoard.sqrMagnitude;
        if (sqrDistance > _pauseCheckDistance * _pauseCheckDistance)
        {
            return false;
        }

        if (sqrDistance <= 0.0001f)
        {
            return true;
        }

        Vector3 forward = rayPoint.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude <= 0.0001f)
        {
            return false;
        }

        forward.Normalize();
        toBoard.Normalize();

        float dot = Vector3.Dot(forward, toBoard);
        return dot >= _pauseCheckDot;
    }

    private void PauseChopping()
    {
        _isChopping = false;

        Debug.Log("도마를 바라보지 않아 다지기 일시정지");
    }

    private void StopChopping()
    {
        _isChopping = false;
        _currentChopProgress = 0f;
        _currentChoppingPlayer = null;

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        if (_canvasObj != null) _canvasObj.SetActive(false);
    }

    private void FinishChop()
    {
        Ingredient ingredient = _onCounterItem.GetComponent<Ingredient>();
        if (ingredient != null)
        {
            ingredient.AddStatus(CookBehaivior.chop);
        }

        _currentChopProgress = 0f;
        _isChopping = false;
        _currentChoppingPlayer = null;

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        if (_canvasObj != null) _canvasObj.SetActive(false);
    }

    // 아이템을 집어가면 다지기 중단 및 게이지 리셋
    public override GameObject TakeItem()
    {
        // 게이지가 5% 이상 찼으면 다시 집을 수 없음
        if (_chopTimeMax > 0f && (_currentChopProgress / _chopTimeMax) >= 0.08f)
        {
            Debug.Log("칼질이 조금이라도 진행된 재료는 다시 집을 수 없습니다.");
            return null;
        }

        _isChopping = false;
        _currentChopProgress = 0f;
        _currentChoppingPlayer = null;

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        return base.TakeItem();
    }
}