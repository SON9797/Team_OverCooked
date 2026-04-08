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

        private void Awake()
        {
            RefreshVisible();
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;

            // 선택 해제되면 조준 상태도 같이 초기화
            if (!_isSelected)
            {
                _isThrowAiming = false;
            }

            RefreshVisible();
        }

        public void SetThrowAiming(bool isThrowAiming)
        {
            _isThrowAiming = isThrowAiming;
            RefreshVisible();
        }

        private void RefreshVisible()
        {
            // 선택되어 있고 조준 중이 아니면 일반 원 표시
            bool showNormal = _isSelected && !_isThrowAiming;

            // 선택되어 있고 조준 중이면 방향 원 표시
            bool showThrowAim = _isSelected && _isThrowAiming;

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


