using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Overcooked/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Header("스테이지 설정")]
        public string LevelName;
        public float GamePlayTime;

        [Header("스테이지 UI 이미지")]
        public Sprite LoadingImage;
        public Sprite TutorialImage;

        // 레시피 리스트 추가 예정
    }
}
