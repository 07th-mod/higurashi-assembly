namespace Assets.Scripts.Core.Interfaces
{
	public interface IScriptInterpreter
	{
		void Initialize(GameSystem gameSystem);

		void Advance();

		void SaveQuickSave();

		void LoadQuickSave();

		void SaveGame(int slotnum);

		void LoadGame(int slotnum);

		int GetFlag(string name);

		void SetFlag(string name, int value);

		void ShutDown();
	}
}
