using Assets.Scripts.Core.State;

namespace Assets.Scripts.Core
{
	internal class StateEntry
	{
		public MGHelper.InputHandler InputHandler;

		public GameState State;

		public IGameState StateObject;

		public StateEntry(MGHelper.InputHandler inputHandler, GameState state)
		{
			InputHandler = inputHandler;
			State = state;
		}

		public StateEntry(IGameState stateobj, GameState state)
		{
			StateObject = stateobj;
			InputHandler = stateobj.InputHandler;
			State = state;
		}
	}
}
