using Overcooked.Interfaces;
using UnityEngine;

namespace Overcooked
{
    public class PlayerInput : IPlayerInputService
    {
        public Vector2 GetMovementInput()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            return new Vector2(x, y).normalized;
        }

        public bool IsInteractButtonDown()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        public bool IsActionButtonDown()
        {
            return Input.GetKeyDown(KeyCode.LeftAlt);
        }
    }
}
