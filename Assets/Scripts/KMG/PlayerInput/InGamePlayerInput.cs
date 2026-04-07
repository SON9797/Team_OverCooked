using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Overcooked
{
    /// <summary>
    /// 인게임 플레이어 인풋 적용
    /// IInGamePlayerInput 상속받음.
    /// </summary>
    public class InGamePlayerInput : IInGamePlayerInput
    {
        public Vector2 Move
        {
            get
            {
                float x = 0f;
                float y = 0f;

                if (Input.GetKey(KeyCode.W)) y += 1f;
                if (Input.GetKey(KeyCode.S)) y -= 1f;
                if (Input.GetKey(KeyCode.D)) x += 1f;
                if (Input.GetKey(KeyCode.A)) x -= 1f;

                return new Vector2(x, y).normalized;
            }
        }

        public bool PauseInput => Input.GetKeyDown(KeyCode.Escape);
        public bool DashInput => Input.GetKeyDown(KeyCode.LeftAlt);
        public bool SwitchingInput => Input.GetKeyDown(KeyCode.LeftShift);

        // 스페이스 : 재료 상호작용
        public bool InteractionIngredientInput => Input.GetKeyDown(KeyCode.Space);

        // 레프트컨트롤 : 손에 아이템 있으면 홀드 조준 / 떼면 던지기, 없으면 조리 상호작용
        public bool InteractionCookPressed => Input.GetKeyDown(KeyCode.LeftControl);
        public bool InteractionCookHeld => Input.GetKey(KeyCode.LeftControl);
        public bool InteractionCookReleased => Input.GetKeyUp(KeyCode.LeftControl);
    }
}
