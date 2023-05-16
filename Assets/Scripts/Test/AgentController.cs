using UnityEngine;

namespace SVS
{
	public class AgentController : MonoBehaviour
	{
		IInput input;
		AgentMovement movement;

		private void OnEnable()
		{
			input = GetComponent<IInput>();
			movement = GetComponent<AgentMovement>();
			input.OnMovementDirectionInput += movement.HandleMovementDirection;
			input.OnMovementInput += movement.HandleMovement;
		}

		private void OnDisable()
		{
			input.OnMovementDirectionInput -= movement.HandleMovementDirection;
			input.OnMovementInput -= movement.HandleMovement;
		}
	}
}
