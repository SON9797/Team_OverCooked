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

        public bool DashInput => Input.GetKeyDown(KeyCode.LeftAlt);
        public bool SwitchingInput => Input.GetKeyDown(KeyCode.LeftShift);

        // 스페이스 : 재료 상호작용
        public bool InteractionIngredientInput => Input.GetKeyDown(KeyCode.Space);

        // 레프트컨트롤 : 조리 상호작용
        public bool InteractionCookInput => Input.GetKeyDown(KeyCode.LeftControl);
    }
}
