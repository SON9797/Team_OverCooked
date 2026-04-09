using OverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Overcooked/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    {
        [Header("스테이지 설정")]
        public int Chapter;
        public int Stage;
        public float GamePlayTime;
        public string NewRecipe;

        [Header("스테이지 BGM")]
        public AudioClip LevelBGM;

        [Header("스테이지 UI 이미지")]
        public Sprite LoadingImage;
        public Sprite TutorialImage;

        [Header("팁 설정")]
        [SerializeField] public int BaseTipAmount = 8;

        [Header("주문 관리")]
        public int MaxOrderCount = 2;

        [Header("스테이지 Star 조건")]
        public int OneStar;
        public int TwoStar;
        public int ThreeStar;

        [Header("레시피 대기 시간")]
        public int recipeTimer = 120;

        [Header("주문용 레시피 (정답 판정, UI 출력")]
        public List<RecipeData> Recipes;

        [Header("조합 가능한 레시피")]
        public List<RecipeData> CombinableRecipes;
    }
}
