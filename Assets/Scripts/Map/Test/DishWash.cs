using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Overcooked;
using Overcooked.Interfaces;

public class DishWash : ItemPlaceAndTake
{
    private static readonly int IsWashingHash = Animator.StringToHash("IsWashing");

    [Header("설거지 시간 설정")]
    [SerializeField] private float _washTimePerPlate = 3f;

    [Header("진행 UI")]
    [SerializeField] private Slider _progressBar;
    [SerializeField] private GameObject _canvasObj;

    [Header("설거지 유지 판정")]
    [SerializeField] private float _pauseCheckDistance = 2.5f;
    [SerializeField, Range(-1f, 1f)] private float _pauseCheckDot = 0.5f;

    [Header("설거지통 안 더러운 접시 표시 오브젝트 (최대 3개)")]
    [SerializeField] private GameObject[] _inDishWasher;

    [Header("완료 접시 스택 높이")]
    [SerializeField] private float _cleanPlateHeightInterval = 0.2f;

    [Inject] private PlateFactory _plateFactory;

    private IInGameSoundManager _inGameSoundManager;

    [Inject]
    public void Construct(IInGameSoundManager inGameSoundManager)
    {
        _inGameSoundManager = inGameSoundManager;
    }

    private int _dirtyPlateCount = 0;
    private float _currentWashProgress = 0f;

    private bool _isWashing = false;
    private PlayerItemController _currentWashingPlayer;

    private readonly List<GameObject> _cleanPlates = new List<GameObject>();

    // 시작 시 UI와 표시용 접시를 초기화
    protected override void Start()
    {
        base.Start();

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        RefreshDirtyPlateVisuals();
        RefreshTopCleanPlate();
    }

    // 매 프레임 설거지 진행과 유지 판정을 처리
    protected override void Update()
    {
        base.Update();

        _cleanPlates.RemoveAll(item => item == null);
        RefreshTopCleanPlate();

        if (!_isWashing)
        {
            return;
        }

        if (_dirtyPlateCount <= 0)
        {
            StopWashingCompletely();
            return;
        }

        if (!CanKeepWashing())
        {
            PauseWashing();
            return;
        }

        _currentWashProgress += Time.deltaTime;

        if (_progressBar != null && _washTimePerPlate > 0f)
        {
            _progressBar.value = _currentWashProgress / _washTimePerPlate;
        }

        if (_currentWashProgress >= _washTimePerPlate)
        {
            FinishOnePlateWash();
        }
    }

    // 더러운 접시를 설거지통에 넣음
    public override bool PlaceItem(GameObject item)
    {
        if (item == null)
        {
            return false;
        }

        if (!item.CompareTag("Dirty"))
        {
            return false;
        }

        _dirtyPlateCount++;
        RefreshDirtyPlateVisuals();
        RefreshWashUi();

        Destroy(item);
        return true;
    }

    // 플레이어가 CTRL로 설거지를 시작/재개/일시정지
    public bool ToggleWash(PlayerItemController player)
    {
        if (_dirtyPlateCount <= 0)
        {
            StopWashingCompletely();
            return false;
        }

        if (player == null)
        {
            PauseWashing();
            return false;
        }

        if (_isWashing)
        {
            PauseWashing();
            return false;
        }

        _currentWashingPlayer = player;
        _isWashing = true;

        SetPlayerWashAnimation(true);
        ShowPlayerProps();

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(true);
        }

        if (_progressBar != null && _washTimePerPlate > 0f)
        {
            _progressBar.value = _currentWashProgress / _washTimePerPlate;
        }

        return true;
    }

    // 설거지를 계속 유지할 수 있는지 확인
    private bool CanKeepWashing()
    {
        if (_currentWashingPlayer == null)
        {
            return false;
        }

        Transform rayPoint = _currentWashingPlayer.GetRayPoint();
        if (rayPoint == null)
        {
            return false;
        }

        Vector3 toWash = transform.position - rayPoint.position;
        toWash.y = 0f;

        float sqrDistance = toWash.sqrMagnitude;
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
        toWash.Normalize();

        float dot = Vector3.Dot(forward, toWash);
        return dot >= _pauseCheckDot;
    }

    // 설거지통 안의 더러운 접시 표시 개수를 갱신
    private void RefreshDirtyPlateVisuals()
    {
        if (_inDishWasher == null || _inDishWasher.Length == 0)
        {
            return;
        }

        int showCount = Mathf.Min(_dirtyPlateCount, 3);

        for (int i = 0; i < _inDishWasher.Length; i++)
        {
            if (_inDishWasher[i] == null)
            {
                continue;
            }

            _inDishWasher[i].SetActive(i < showCount);
        }
    }

    // 현재 설거지 UI 상태를 갱신
    private void RefreshWashUi()
    {
        bool shouldShow = _dirtyPlateCount > 0 && (_isWashing || _currentWashProgress > 0f);

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(shouldShow);
        }

        if (_progressBar != null)
        {
            if (_dirtyPlateCount <= 0 || _washTimePerPlate <= 0f)
            {
                _progressBar.value = 0f;
            }
            else
            {
                _progressBar.value = _currentWashProgress / _washTimePerPlate;
            }
        }
    }

    // 플레이어 설거지 애니메이션 on/off
    private void SetPlayerWashAnimation(bool isWash)
    {
        if (_currentWashingPlayer == null)
        {
            return;
        }

        Animator animator = _currentWashingPlayer.GetComponent<Animator>();
        if (animator != null)
        {
            _inGameSoundManager.PlaySFX(OverCooked.SFXType.Washing);
            animator.SetBool(IsWashingHash, isWash);
        }
    }

    // 현재 설거지 플레이어의 Props만 보이게 설정
    private void ShowPlayerProps()
    {
        if (_currentWashingPlayer == null)
        {
            return;
        }

        PlayerToolVisualController toolVisual = _currentWashingPlayer.GetComponent<PlayerToolVisualController>();
        if (toolVisual != null)
        {
            toolVisual.ShowPropsOnly();
        }
    }

    // 현재 설거지 플레이어의 도구를 전부 숨김
    private void HidePlayerTools()
    {
        if (_currentWashingPlayer == null)
        {
            return;
        }

        PlayerToolVisualController toolVisual = _currentWashingPlayer.GetComponent<PlayerToolVisualController>();
        if (toolVisual != null)
        {
            toolVisual.HideAllTools();
        }
    }

    // 설거지를 일시정지
    private void PauseWashing()
    {
        _isWashing = false;
        SetPlayerWashAnimation(false);
        HidePlayerTools();

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(_dirtyPlateCount > 0);
        }

        if (_progressBar != null && _washTimePerPlate > 0f)
        {
            _progressBar.value = _currentWashProgress / _washTimePerPlate;
        }
    }

    // 설거지를 완전히 종료하고 진행률을 초기화
    private void StopWashingCompletely()
    {
        _isWashing = false;
        SetPlayerWashAnimation(false);
        HidePlayerTools();

        _currentWashProgress = 0f;
        _currentWashingPlayer = null;

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }
    }

    // 접시 한 장 설거지를 완료
    private void FinishOnePlateWash()
    {
        _currentWashProgress = 0f;

        if (_dirtyPlateCount > 0)
        {
            _dirtyPlateCount--;
        }

        RefreshDirtyPlateVisuals();
        SpawnCleanPlate();

        if (_dirtyPlateCount <= 0)
        {
            StopWashingCompletely();
            return;
        }

        if (_progressBar != null)
        {
            _progressBar.value = 0f;
        }

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(true);
        }
    }

    // 완료된 깨끗한 접시를 스택에 생성
    private void SpawnCleanPlate()
    {
        int stackCount = _cleanPlates.Count;
        float yOffset = stackCount * _cleanPlateHeightInterval;
        Vector3 spawnPosition = _snapPoint.position + new Vector3(0f, yOffset, 0f);

        GameObject cleanPlate = _plateFactory.CreateClean(spawnPosition);

        cleanPlate.transform.SetParent(_snapPoint);
        cleanPlate.transform.position = spawnPosition;
        cleanPlate.transform.localRotation = Quaternion.identity;

        if (cleanPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        _cleanPlates.Add(cleanPlate);
        RefreshTopCleanPlate();
    }

    // 가장 위의 깨끗한 접시를 _onCounterItem으로 맞춤
    private void RefreshTopCleanPlate()
    {
        _cleanPlates.RemoveAll(item => item == null);

        if (_cleanPlates.Count > 0)
        {
            _onCounterItem = _cleanPlates[_cleanPlates.Count - 1];
        }
        else
        {
            _onCounterItem = null;
        }
    }

    // 완료된 깨끗한 접시를 가져감
    public override GameObject TakeItem()
    {
        _cleanPlates.RemoveAll(item => item == null);

        if (_cleanPlates.Count == 0)
        {
            return null;
        }

        int lastIndex = _cleanPlates.Count - 1;
        GameObject topPlate = _cleanPlates[lastIndex];
        _cleanPlates.RemoveAt(lastIndex);

        topPlate.transform.SetParent(null);

        if (topPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        RefreshTopCleanPlate();
        return topPlate;
    }

    // 설거지통에는 항상 더러운 접시를 넣을 수 있음
    public override bool CanPlaceItem()
    {
        return true;
    }

    // 현재 완료된 깨끗한 접시가 있는지 확인
    public new bool HasItem => _cleanPlates.Count > 0;

    // 현재 더러운 접시가 하나 이상 있는지 확인
    public bool HasDirtyPlate()
    {
        return _dirtyPlateCount > 0;
    }
}