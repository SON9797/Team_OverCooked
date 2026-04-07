using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class InGamePlayerIndicators : MonoBehaviour
    {
        [Header("평소 선택 표시 원")]
        [SerializeField] private GameObject _normalRing;

        [Header("던지기 조준 방향 표시 원")]
        [SerializeField] private GameObject _throwAimRing;

        private bool _isSelected;
        private bool _isThrowAiming;
        private bool _isHiddenByThrow;

        private void Awake()
        {
            RefreshVisible();
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;

            // 선택 해제되면 투척 관련 상태도 같이 초기화
            if (!_isSelected)
            {
                _isThrowAiming = false;
                _isHiddenByThrow = false;
            }

            RefreshVisible();
        }

        public void SetThrowAiming(bool isThrowAiming)
        {
            _isThrowAiming = isThrowAiming;
            RefreshVisible();
        }

        public void HideByThrow()
        {
            _isHiddenByThrow = true;
            _isThrowAiming = false;
            RefreshVisible();
        }

        public void ShowAfterThrow()
        {
            _isHiddenByThrow = false;
            _isThrowAiming = false;
            RefreshVisible();
        }

        private void RefreshVisible()
        {
            bool showNormal = _isSelected && !_isThrowAiming && !_isHiddenByThrow;
            bool showThrowAim = _isSelected && _isThrowAiming && !_isHiddenByThrow;

            if (_normalRing != null)
            {
                _normalRing.SetActive(showNormal);
            }

            if (_throwAimRing != null)
            {
                _throwAimRing.SetActive(showThrowAim);
            }
        }
    }
}


