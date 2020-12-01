using Assets.Scripts.Core;
using Assets.Scripts.Core.State;

namespace MOD.Scripts.Core.State
{
	// This state is used to disable most of the game's input, so that
	// you don't trigger the game's inputs while you're in the mod menu.
	// Note: certain buttons are still clickable even in this state, but
	// - you can use GameSystem.HideUI() to hide the normal game buttons
	// - you can register a LeaveConfigScreen() action to close the config screen
	class MODStateDisableInput : IGameState
	{
		public void RequestLeaveImmediate() => GameSystem.Instance.PopStateStack();
		public void RequestLeave() => GameSystem.Instance.PopStateStack();
		public void OnLeaveState() {}
		public void OnRestoreState() {}
		public bool InputHandler() => false;
		public GameState GetStateType() => GameState.MODDisableInput;
	}
}
