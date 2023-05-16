using System;
using UnityEngine;

namespace SVS
{
    public interface IInput
    {
        Action<Vector3> OnMovementDirectionInput { get; set; }
        Action<Vector2> OnMovementInput { get; set; }
    }
}