using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class ChefVisualApplier : MonoBehaviour
    {
        [Header("적용 대상")]
        [SerializeField] private ChefVisualController _player1Visual;
        [SerializeField] private ChefVisualController _player2Visual;

        [Header("테스트 설정")]
        [SerializeField] private bool _useManualChefForTest = true;
        [SerializeField] private ChefType _manualChef = ChefType.Panda;

        private void Awake()
        {
            if (_useManualChefForTest)
            {
                SetChef(_manualChef);
                return;
            }

            ChefRuntimeStore.EnsureInitialized();
            ApplyCurrentChef();
        }

        public void ApplyCurrentChef()
        {
            ChefType currentChef = ChefRuntimeStore.CurrentChef;

            if (_player1Visual != null)
            {
                _player1Visual.ApplyChef(currentChef);
            }

            if (_player2Visual != null)
            {
                _player2Visual.ApplyChef(currentChef);
            }
        }

        public void SetChef(ChefType chefType)
        {
            ChefRuntimeStore.SetChef(chefType);

            if (_player1Visual != null)
            {
                _player1Visual.ApplyChef(chefType);
            }

            if (_player2Visual != null)
            {
                _player2Visual.ApplyChef(chefType);
            }
        }
    }
}
