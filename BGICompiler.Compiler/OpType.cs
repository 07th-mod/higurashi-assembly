using Assets.Scripts.Core.Buriko;

namespace BGICompiler.Compiler
{
	public class OpType
	{
		public BurikoOperations OpCode;

		public string Parameters;

		public OpType(BurikoOperations op, string param)
		{
			OpCode = op;
			Parameters = param;
		}
	}
}
