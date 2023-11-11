using Antlr.Runtime;
using Antlr.Runtime.Tree;
using AntlrTest;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BGICompiler.Compiler
{
	public class BGItoMG
	{
		private readonly string filename;

		public MemoryStream Mstream;

		public BinaryWriter Output;

		private Dictionary<string, int> blockList = new Dictionary<string, int>();

		private Dictionary<int, int> lineLookup = new Dictionary<int, int>();

		public static BGItoMG Instance;

		private int cmdnum;

		public BGItoMG(string scriptname, string outname)
		{
			Instance = this;
			filename = scriptname;
			cmdnum = 0;
			string[] lines = LoadScriptLines();
			string script = Preprocessor(lines);
			Compile(script);
			OutputFile(outname);
			Output.Close();
			Mstream.Close();
		}

		public void OutputFile(string outname)
		{
			byte[] array = Mstream.ToArray();
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write("MGSC".ToCharArray(0, 4));
			binaryWriter.Write(1);
			binaryWriter.Write(blockList.Count);
			binaryWriter.Write(lineLookup.Count);
			binaryWriter.Write(array.Length);
			foreach (KeyValuePair<string, int> block in blockList)
			{
				binaryWriter.Write(block.Key);
				binaryWriter.Write(block.Value);
			}
			foreach (KeyValuePair<int, int> item in lineLookup)
			{
				binaryWriter.Write(item.Value);
			}
			binaryWriter.Write(array);
			File.WriteAllBytes(outname, memoryStream.ToArray());
		}

		public void OutputCmd(BurikoCommands cmd)
		{
			Output.Write((short)cmd);
		}

		public void Cmd_Return()
		{
			OutputCmd(BurikoCommands.Return);
		}

		public void Cmd_LineNum(int line)
		{
			OutputCmd(BurikoCommands.LineNum);
			Output.Write(cmdnum);
			if (!lineLookup.ContainsKey(cmdnum))
			{
				lineLookup.Add(cmdnum, (int)Mstream.Position);
			}
			cmdnum++;
		}

		public void OutputTree(ITree tree, int start = 0)
		{
			for (int i = start; i < tree.ChildCount; i++)
			{
				ITree child = tree.GetChild(i);
				try
				{
					switch (child.ToString())
					{
					case "IF":
						Cmd_LineNum(child.Line);
						ParseIf(child);
						break;
					case "OPERATION":
						Cmd_LineNum(child.Line);
						OutputCmd(BurikoCommands.Operation);
						new OperationHandler().ParseOperation(child);
						break;
					case "VARDECL":
						ParseVarDecl(child);
						break;
					case "ASSIGN":
						ParseAssignment(child);
						break;
					default:
						Debug.LogError("Unhandled type " + child.ToString());
						break;
					}
				}
				catch (Exception exception)
				{
					Debug.LogError($"{child.Line}: Failed to parse output! Tree type: {child.ToString()}");
					Debug.LogException(exception);
					throw;
				}
			}
		}

		public void ParseAssignment(ITree tree)
		{
			OutputCmd(BurikoCommands.Assignment);
			new BGIValue(tree.GetChild(0)).Output();
			new BGIValue(tree.GetChild(1)).Output();
		}

		public void ParseVarDecl(ITree tree)
		{
			OutputCmd(BurikoCommands.Declaration);
			string text = tree.GetChild(0).Text;
			ITree child = tree.GetChild(1);
			string text2 = child.GetChild(0).Text;
			Output.Write(text);
			Output.Write(text2);
			if (child.ChildCount > 1)
			{
				ITree child2 = child.GetChild(1);
				if (child2.Text != "INDEX")
				{
					throw new Exception("Invalid variable index");
				}
				new BGIValue(child2.GetChild(0)).Output();
			}
			else
			{
				Output.Write((short)1);
			}
			if (tree.ChildCount > 2)
			{
				BGIValue bGIValue = new BGIValue(child);
				BGIValue bGIValue2 = new BGIValue(tree.GetChild(2));
				OutputCmd(BurikoCommands.Assignment);
				bGIValue.Output();
				bGIValue2.Output();
			}
		}

		public void ParseIf(ITree tree)
		{
			BGIValue bGIValue = new BGIValue(tree.GetChild(0));
			Output.Write((short)3);
			bGIValue.Output();
			long position = Mstream.Position;
			Output.Write(0);
			OutputTree(tree.GetChild(1));
			Output.Write((short)6);
			long position2 = Mstream.Position;
			Output.Write(0);
			long position3 = Mstream.Position;
			Mstream.Seek(position, SeekOrigin.Begin);
			Output.Write((int)position3);
			Mstream.Seek(position3, SeekOrigin.Begin);
			OutputTree(tree.GetChild(2));
			long position4 = Mstream.Position;
			Mstream.Seek(position2, SeekOrigin.Begin);
			Output.Write((int)position4);
			Mstream.Seek(position4, SeekOrigin.Begin);
		}

		public void ParseBlock(CommonTree tree)
		{
			if (tree is CommonErrorNode)
			{
				CommonErrorNode commonErrorNode = (CommonErrorNode)tree;
				throw new Exception($"Failed to parse block! Line {commonErrorNode.stop.Line - 1}:{commonErrorNode.stop.CharPositionInLine}\nException: {commonErrorNode.trappedException}");
			}
			if (tree.Token.Text != "BLOCK")
			{
				throw new Exception($"Line {tree.Line}: Expected token type BLOCK (function declaration), got token {tree.Token.Text}");
			}
			string text = tree.Children[0].Text;
			if (blockList.ContainsKey(text))
			{
				throw new Exception($"Line {tree.Line}: Function name '{text}' already previously declared.");
			}
			blockList.Add(text, (int)Mstream.Position);
			OutputTree(tree, 1);
			Cmd_LineNum(0);
			Cmd_Return();
		}

		public void Compile(string script)
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new bgitestParser(new CommonTokenStream(new bgitestLexer(new ANTLRStringStream(script)))).program();
			Mstream = new MemoryStream();
			Output = new BinaryWriter(Mstream);
			CommonTree tree = astParserRuleReturnScope.Tree;
			if (tree.Token != null)
			{
				ParseBlock(tree);
			}
			else
			{
				foreach (CommonTree child in tree.Children)
				{
					ParseBlock(child);
				}
			}
		}

		public string[] LoadScriptLines()
		{
			if (!File.Exists(filename))
			{
				throw new FileNotFoundException(filename);
			}
			return File.ReadAllLines(filename);
		}

		public string Preprocessor(string[] lines)
		{
			string text = "";
			int num = 0;
			foreach (string text2 in lines)
			{
				text += "\r\n";
				if (text2 == "" || text2.StartsWith("//") || (num == 0 && text2.StartsWith("#")))
				{
					continue;
				}
				num += text2.Count((char f) => f == '{');
				num -= text2.Count((char f) => f == '}');
				if (num > 0 && text2.Length > 0 && !text2.StartsWith("\t") && !text2.StartsWith("{") && !text2.StartsWith("}") && !text2.StartsWith("//") && !text2.StartsWith(" "))
				{
					string text3 = text2.Replace("\"", "\\\"");
					text3 = text3.Replace(", )", " )");
					if (text3.StartsWith(".."))
					{
						text3 = text3.Substring(2);
					}
					BurikoTextModes burikoTextModes = BurikoTextModes.Normal;
					if (text3.EndsWith(">"))
					{
						burikoTextModes = BurikoTextModes.Continue;
						text3 = text3.Substring(0, text3.Length - 1);
					}
					if (text3.EndsWith("<"))
					{
						burikoTextModes = BurikoTextModes.WaitThenContinue;
						text3 = text3.Substring(0, text3.Length - 1);
					}
					if (text3.EndsWith("@") || text3.EndsWith("&"))
					{
						burikoTextModes = BurikoTextModes.WaitForInput;
						text3 = text3.Substring(0, text3.Length - 1);
					}
					bool flag = true;
					if (text3.Contains("\t") && string.IsNullOrEmpty(text3.Split('\t')[0].Trim()))
					{
						flag = false;
					}
					if (flag)
					{
						text += string.Format("\tOutputLine(NULL, \"{0}\", NULL, \"{0}\", {1});", text3, (int)burikoTextModes);
						continue;
					}
				}
				string text4 = text2.Replace(", )", " )");
				text4 = text4.Replace(",)", " )");
				text4 = text4.Replace("Line_Normal", "0");
				text4 = text4.Replace("Line_ContinueAfterTyping", "3");
				text4 = text4.Replace("Line_WaitForInput", "2");
				text4 = text4.Replace("Line_Continue", "1");
				text += text4;
			}
			return text;
		}
	}
}
