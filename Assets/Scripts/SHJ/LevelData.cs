using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    [Serializable]
    public struct RecipeData
    {
        public string DisgName;
        public Sprite FinishedDishImage;
        public List<IngreDientData> Ingredients;
        public int BaseScore;
        public GameObject model;
    }

    [CreateAssetMenu(fileName = "Level_", menuName = "Overcooked/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Header("스테이지 설정")]
        public string LevelName;
        public float GamePlayTime;

        [Header("스테이지 UI 이미지")]
        public Sprite LoadingImage;
        public Sprite TutorialImage;

        [Header("팁 설정")]
        [SerializeField] public int BaseTipAmount = 8;

        [Header("주문 관리")]
        public int MaxOrderCount = 2;

        [Header("레시피 설정")]
        public List<RecipeData> Recipes;
    }
}
