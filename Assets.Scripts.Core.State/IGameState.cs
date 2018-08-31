namespace Assets.Scripts.Core.State
{
	public interface IGameState
	{
		void RequestLeaveImmediate();

		void RequestLeave();

		void OnLeaveState();

		void OnRestoreState();

		bool InputHandler();

		GameState GetStateType();
	}
}
