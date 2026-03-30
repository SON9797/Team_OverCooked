using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Overcooked
{
    /// <summary>
    /// 인게임 플레이어 인풋 키 인터페이스. 추가 필요 시 추가 후
    /// InGamePlayerInput(상속받음)에 구현해야함. 
    /// </summary>
    public interface IInGamePlayerInput
    {
        Vector2 Move { get; }
        bool DashInput { get; }
        bool SwitchingInput { get; }

        bool InteractionIngredientInput { get; }
        bool InteractionCookInput { get; }

        bool PauseInput { get; }
        
    }
}
