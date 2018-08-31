using Antlr.Runtime;
using Antlr.Runtime.Tree;
using System;
using System.CodeDom.Compiler;

namespace AntlrTest
{
	[CLSCompliant(false)]
	[GeneratedCode("ANTLR", "3.5.0.1")]
	public class bgitestParser : Parser
	{
		private static class Follow
		{
			public static readonly BitSet _directive_in_program143 = new BitSet(new ulong[1]
			{
				786434uL
			});

			public static readonly BitSet _block_in_program145 = new BitSet(new ulong[1]
			{
				786434uL
			});

			public static readonly BitSet _HASH_in_directive158 = new BitSet(new ulong[2]
			{
				0uL,
				1uL
			});

			public static readonly BitSet _64_in_directive160 = new BitSet(new ulong[1]
			{
				17592186044416uL
			});

			public static readonly BitSet _STRING_in_directive162 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_block176 = new BitSet(new ulong[1]
			{
				524288uL
			});

			public static readonly BitSet _ID_in_block180 = new BitSet(new ulong[1]
			{
				67108864uL
			});

			public static readonly BitSet _LPAREN_in_block182 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RPAREN_in_block184 = new BitSet(new ulong[1]
			{
				16777216uL
			});

			public static readonly BitSet _LCURL_in_block186 = new BitSet(new ulong[1]
			{
				9223372311733207040uL
			});

			public static readonly BitSet _operations_in_block188 = new BitSet(new ulong[1]
			{
				274877906944uL
			});

			public static readonly BitSet _RCURL_in_block191 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_operations215 = new BitSet(new ulong[1]
			{
				9223372036855300098uL
			});

			public static readonly BitSet _declaration_in_operation229 = new BitSet(new ulong[1]
			{
				2199023255552uL
			});

			public static readonly BitSet _SEMICOLON_in_operation231 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _if_in_operation238 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _function_in_operation244 = new BitSet(new ulong[1]
			{
				2199023255552uL
			});

			public static readonly BitSet _SEMICOLON_in_operation246 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _assignment_in_operation253 = new BitSet(new ulong[1]
			{
				2199023255552uL
			});

			public static readonly BitSet _SEMICOLON_in_operation255 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_declaration267 = new BitSet(new ulong[1]
			{
				524288uL
			});

			public static readonly BitSet _variable_in_declaration269 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_function290 = new BitSet(new ulong[1]
			{
				67108864uL
			});

			public static readonly BitSet _LPAREN_in_function292 = new BitSet(new ulong[1]
			{
				4035243408141844480uL
			});

			public static readonly BitSet _parameters_in_function294 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RPAREN_in_function297 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _variable_in_assignment319 = new BitSet(new ulong[1]
			{
				32uL
			});

			public static readonly BitSet _ASSIGN_in_assignment321 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _obj_in_assignment323 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _obj_in_parameters345 = new BitSet(new ulong[1]
			{
				1026uL
			});

			public static readonly BitSet _COMMA_in_parameters348 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _obj_in_parameters350 = new BitSet(new ulong[1]
			{
				1026uL
			});

			public static readonly BitSet _ID_in_variable375 = new BitSet(new ulong[1]
			{
				288230376285929474uL
			});

			public static readonly BitSet _LSQ_in_variable378 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _value_in_variable382 = new BitSet(new ulong[1]
			{
				1099511627776uL
			});

			public static readonly BitSet _RSQ_in_variable384 = new BitSet(new ulong[1]
			{
				288230376151711746uL
			});

			public static readonly BitSet _58_in_variable389 = new BitSet(new ulong[1]
			{
				524288uL
			});

			public static readonly BitSet _variable_in_variable393 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _op_compare_in_obj431 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _value_in_obj437 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _INT_in_value448 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _STRING_in_value462 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _60_in_value476 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _61_in_value488 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _59_in_value502 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _variable_in_value516 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _function_in_value530 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _LPAREN_in_value544 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_compare_in_value546 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RPAREN_in_value548 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _value_in_op_mult567 = new BitSet(new ulong[1]
			{
				70368744181762uL
			});

			public static readonly BitSet _TIMES_in_op_mult574 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _value_in_op_mult577 = new BitSet(new ulong[1]
			{
				70368744181762uL
			});

			public static readonly BitSet _DIVIDE_in_op_mult585 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _value_in_op_mult588 = new BitSet(new ulong[1]
			{
				70368744181762uL
			});

			public static readonly BitSet _op_mult_in_op_add605 = new BitSet(new ulong[1]
			{
				138512695298uL
			});

			public static readonly BitSet _PLUS_in_op_add612 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_mult_in_op_add615 = new BitSet(new ulong[1]
			{
				138512695298uL
			});

			public static readonly BitSet _MINUS_in_op_add623 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_mult_in_op_add626 = new BitSet(new ulong[1]
			{
				138512695298uL
			});

			public static readonly BitSet _op_add_in_op_compare643 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _AND_in_op_compare650 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare653 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _OR_in_op_compare661 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare664 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _LEQ_in_op_compare672 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare675 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _GEQ_in_op_compare683 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare686 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _LTHAN_in_op_compare694 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare697 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _GTHAN_in_op_compare705 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare708 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _EQ_in_op_compare716 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _op_add_in_op_compare719 = new BitSet(new ulong[1]
			{
				34661941266uL
			});

			public static readonly BitSet _63_in_if737 = new BitSet(new ulong[1]
			{
				67108864uL
			});

			public static readonly BitSet _LPAREN_in_if739 = new BitSet(new ulong[1]
			{
				4035242858386030592uL
			});

			public static readonly BitSet _obj_in_if741 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RPAREN_in_if743 = new BitSet(new ulong[1]
			{
				9223372036872077312uL
			});

			public static readonly BitSet _LCURL_in_if747 = new BitSet(new ulong[1]
			{
				9223372311733207040uL
			});

			public static readonly BitSet _operations_in_if751 = new BitSet(new ulong[1]
			{
				274877906944uL
			});

			public static readonly BitSet _RCURL_in_if753 = new BitSet(new ulong[1]
			{
				4611686018427387906uL
			});

			public static readonly BitSet _operation_in_if760 = new BitSet(new ulong[1]
			{
				4611686018427387906uL
			});

			public static readonly BitSet _62_in_if764 = new BitSet(new ulong[1]
			{
				9223372036872077312uL
			});

			public static readonly BitSet _LCURL_in_if768 = new BitSet(new ulong[1]
			{
				9223372311733207040uL
			});

			public static readonly BitSet _operations_in_if772 = new BitSet(new ulong[1]
			{
				274877906944uL
			});

			public static readonly BitSet _RCURL_in_if774 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_if781 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operations_in_synpred3_bgitest188 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _parameters_in_synpred8_bgitest294 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _op_compare_in_synpred13_bgitest431 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _62_in_synpred34_bgitest764 = new BitSet(new ulong[1]
			{
				9223372036872077312uL
			});

			public static readonly BitSet _LCURL_in_synpred34_bgitest768 = new BitSet(new ulong[1]
			{
				9223372311733207040uL
			});

			public static readonly BitSet _operations_in_synpred34_bgitest772 = new BitSet(new ulong[1]
			{
				274877906944uL
			});

			public static readonly BitSet _RCURL_in_synpred34_bgitest774 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_synpred34_bgitest781 = new BitSet(new ulong[1]
			{
				2uL
			});
		}

		public const int EOF = -1;

		public const int AND = 4;

		public const int ASSIGN = 5;

		public const int BLOCK = 6;

		public const int BWAND = 7;

		public const int BWOR = 8;

		public const int CHAR = 9;

		public const int COMMA = 10;

		public const int COMMENT = 11;

		public const int DIVIDE = 12;

		public const int ELSE = 13;

		public const int EQ = 14;

		public const int FALSE = 15;

		public const int GEQ = 16;

		public const int GTHAN = 17;

		public const int HASH = 18;

		public const int ID = 19;

		public const int IF = 20;

		public const int INCLUDE = 21;

		public const int INDEX = 22;

		public const int INT = 23;

		public const int LCURL = 24;

		public const int LEQ = 25;

		public const int LPAREN = 26;

		public const int LSQ = 27;

		public const int LTHAN = 28;

		public const int MEMBER = 29;

		public const int MINUS = 30;

		public const int MOD = 31;

		public const int NEQ = 32;

		public const int NOT = 33;

		public const int OPERATION = 34;

		public const int OR = 35;

		public const int PARAMETERS = 36;

		public const int PLUS = 37;

		public const int RCURL = 38;

		public const int RPAREN = 39;

		public const int RSQ = 40;

		public const int SEMICOLON = 41;

		public const int SHIFT_LEFT = 42;

		public const int SHIFT_RIGHT = 43;

		public const int STRING = 44;

		public const int THEN = 45;

		public const int TIMES = 46;

		public const int TRUE = 47;

		public const int TYPEBOOL = 48;

		public const int TYPEFUNCTION = 49;

		public const int TYPEINT = 50;

		public const int TYPEMATH = 51;

		public const int TYPENULL = 52;

		public const int TYPESTRING = 53;

		public const int TYPEVARIABLE = 54;

		public const int VAR = 55;

		public const int VARDECL = 56;

		public const int WS = 57;

		public const int T__58 = 58;

		public const int T__59 = 59;

		public const int T__60 = 60;

		public const int T__61 = 61;

		public const int T__62 = 62;

		public const int T__63 = 63;

		public const int T__64 = 64;

		internal static readonly string[] tokenNames = new string[65]
		{
			"<invalid>",
			"<EOR>",
			"<DOWN>",
			"<UP>",
			"AND",
			"ASSIGN",
			"BLOCK",
			"BWAND",
			"BWOR",
			"CHAR",
			"COMMA",
			"COMMENT",
			"DIVIDE",
			"ELSE",
			"EQ",
			"FALSE",
			"GEQ",
			"GTHAN",
			"HASH",
			"ID",
			"IF",
			"INCLUDE",
			"INDEX",
			"INT",
			"LCURL",
			"LEQ",
			"LPAREN",
			"LSQ",
			"LTHAN",
			"MEMBER",
			"MINUS",
			"MOD",
			"NEQ",
			"NOT",
			"OPERATION",
			"OR",
			"PARAMETERS",
			"PLUS",
			"RCURL",
			"RPAREN",
			"RSQ",
			"SEMICOLON",
			"SHIFT_LEFT",
			"SHIFT_RIGHT",
			"STRING",
			"THEN",
			"TIMES",
			"TRUE",
			"TYPEBOOL",
			"TYPEFUNCTION",
			"TYPEINT",
			"TYPEMATH",
			"TYPENULL",
			"TYPESTRING",
			"TYPEVARIABLE",
			"VAR",
			"VARDECL",
			"WS",
			"'.'",
			"'FALSE'",
			"'NULL'",
			"'TRUE'",
			"'else'",
			"'if'",
			"'include'"
		};

		private ITreeAdaptor adaptor;

		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return adaptor;
			}
			set
			{
				adaptor = value;
			}
		}

		public override string[] TokenNames => tokenNames;

		public override string GrammarFileName => "D:\\Projects\\d2bvsdd\\Assets\\Scripts\\Editor\\BGICompiler\\Grammar\\bgitest.g";

		public bgitestParser(ITokenStream input)
			: this(input, new RecognizerSharedState())
		{
		}

		public bgitestParser(ITokenStream input, RecognizerSharedState state)
			: base(input, state)
		{
			ITreeAdaptor treeAdaptor = null;
			TreeAdaptor = (treeAdaptor ?? new CommonTreeAdaptor());
		}

		[GrammarRule("program")]
		public AstParserRuleReturnScope<CommonTree, IToken> program()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			try
			{
				try
				{
					commonTree = (CommonTree)adaptor.Nil();
					try
					{
						while (true)
						{
							int num = 3;
							try
							{
								switch (input.LA(1))
								{
								case 18:
									num = 1;
									break;
								case 19:
									num = 2;
									break;
								}
							}
							finally
							{
							}
							switch (num)
							{
							case 1:
								PushFollow(Follow._directive_in_program143);
								astParserRuleReturnScope2 = directive();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
								}
								continue;
							case 2:
								PushFollow(Follow._block_in_program145);
								astParserRuleReturnScope3 = block();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
								}
								continue;
							}
							break;
						}
					}
					finally
					{
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("directive")]
		private AstParserRuleReturnScope<CommonTree, IToken> directive()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token HASH");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token 64");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token STRING");
			try
			{
				try
				{
					token = (IToken)Match(input, 18, Follow._HASH_in_directive158);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 64, Follow._64_in_directive160);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token2);
					}
					token3 = (IToken)Match(input, 44, Follow._STRING_in_directive162);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token3);
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						commonTree = (astParserRuleReturnScope.Tree = null);
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("block")]
		private AstParserRuleReturnScope<CommonTree, IToken> block()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			IToken token4 = null;
			IToken token5 = null;
			IToken token6 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			CommonTree commonTree5 = null;
			CommonTree commonTree6 = null;
			CommonTree commonTree7 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token LCURL");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token RCURL");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule operations");
			try
			{
				try
				{
					token2 = (IToken)Match(input, 19, Follow._ID_in_block176);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token2);
					}
					token = (IToken)Match(input, 19, Follow._ID_in_block180);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token3 = (IToken)Match(input, 26, Follow._LPAREN_in_block182);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token3);
					}
					token4 = (IToken)Match(input, 39, Follow._RPAREN_in_block184);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token4);
					}
					token5 = (IToken)Match(input, 24, Follow._LCURL_in_block186);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream4.Add(token5);
					}
					int num = 2;
					try
					{
						try
						{
							switch (input.LA(1))
							{
							case 19:
							case 63:
								num = 1;
								break;
							case 38:
							{
								int num2 = input.LA(2);
								if (EvaluatePredicate(synpred3_bgitest_fragment))
								{
									num = 1;
								}
								break;
							}
							}
						}
						finally
						{
						}
						int num3 = num;
						if (num3 == 1)
						{
							PushFollow(Follow._operations_in_block188);
							astParserRuleReturnScope2 = operations();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
							}
						}
					}
					finally
					{
					}
					token6 = (IToken)Match(input, 38, Follow._RCURL_in_block191);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream5.Add(token6);
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token id1", token);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(6, "BLOCK"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleTokenStream6.NextNode());
						if (rewriteRuleSubtreeStream.HasNext)
						{
							adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						}
						rewriteRuleSubtreeStream.Reset();
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("operations")]
		private AstParserRuleReturnScope<CommonTree, IToken> operations()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			try
			{
				try
				{
					commonTree = (CommonTree)adaptor.Nil();
					try
					{
						while (true)
						{
							int num = 2;
							try
							{
								int num2 = input.LA(1);
								if (num2 == 19 || num2 == 63)
								{
									num = 1;
								}
							}
							finally
							{
							}
							int num3 = num;
							if (num3 != 1)
							{
								break;
							}
							PushFollow(Follow._operation_in_operations215);
							astParserRuleReturnScope2 = operation();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
							}
						}
					}
					finally
					{
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("operation")]
		private AstParserRuleReturnScope<CommonTree, IToken> operation()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope5 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			try
			{
				try
				{
					int num = 4;
					try
					{
						switch (input.LA(1))
						{
						case 19:
							switch (input.LA(2))
							{
							case 26:
								num = 3;
								break;
							case 19:
								num = 1;
								break;
							case 5:
							case 27:
							case 58:
								num = 4;
								break;
							default:
								if (state.backtracking <= 0)
								{
									NoViableAltException ex2 = new NoViableAltException(string.Empty, 4, 1, input, 2);
									throw ex2;
								}
								state.failed = true;
								return astParserRuleReturnScope;
							}
							break;
						case 63:
							num = 2;
							break;
						default:
							if (state.backtracking <= 0)
							{
								NoViableAltException ex = new NoViableAltException(string.Empty, 4, 0, input, 1);
								throw ex;
							}
							state.failed = true;
							return astParserRuleReturnScope;
						}
					}
					finally
					{
					}
					switch (num)
					{
					case 1:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._declaration_in_operation229);
						astParserRuleReturnScope2 = declaration();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
						}
						token = (IToken)Match(input, 41, Follow._SEMICOLON_in_operation231);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						break;
					case 2:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._if_in_operation238);
						astParserRuleReturnScope3 = @if();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
						}
						break;
					case 3:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._function_in_operation244);
						astParserRuleReturnScope4 = function();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
						}
						token2 = (IToken)Match(input, 41, Follow._SEMICOLON_in_operation246);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						break;
					case 4:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._assignment_in_operation253);
						astParserRuleReturnScope5 = assignment();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope5.Tree);
						}
						token3 = (IToken)Match(input, 41, Follow._SEMICOLON_in_operation255);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						break;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex3)
				{
					ReportError(ex3);
					Recover(input, ex3);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex3);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("declaration")]
		private AstParserRuleReturnScope<CommonTree, IToken> declaration()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			CommonTree commonTree2 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			try
			{
				try
				{
					token = (IToken)Match(input, 19, Follow._ID_in_declaration267);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(Follow._variable_in_declaration269);
					astParserRuleReturnScope2 = variable();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(56, "VARDECL"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleTokenStream.NextNode());
						adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("function")]
		private AstParserRuleReturnScope<CommonTree, IToken> function()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule parameters");
			try
			{
				try
				{
					token = (IToken)Match(input, 19, Follow._ID_in_function290);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 26, Follow._LPAREN_in_function292);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token2);
					}
					int num = 2;
					try
					{
						try
						{
							int num2 = input.LA(1);
							if (num2 == 19 || num2 == 23 || num2 == 26 || num2 == 44 || (num2 >= 59 && num2 <= 61))
							{
								num = 1;
							}
							else if (num2 == 39)
							{
								int num3 = input.LA(2);
								if (EvaluatePredicate(synpred8_bgitest_fragment))
								{
									num = 1;
								}
							}
						}
						finally
						{
						}
						int num4 = num;
						if (num4 == 1)
						{
							PushFollow(Follow._parameters_in_function294);
							astParserRuleReturnScope2 = parameters();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
							}
						}
					}
					finally
					{
					}
					token3 = (IToken)Match(input, 39, Follow._RPAREN_in_function297);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token3);
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(34, "OPERATION"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleTokenStream.NextNode());
						if (rewriteRuleSubtreeStream.HasNext)
						{
							adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						}
						rewriteRuleSubtreeStream.Reset();
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("assignment")]
		private AstParserRuleReturnScope<CommonTree, IToken> assignment()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			CommonTree commonTree2 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ASSIGN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule obj");
			try
			{
				try
				{
					PushFollow(Follow._variable_in_assignment319);
					astParserRuleReturnScope2 = variable();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
					}
					token = (IToken)Match(input, 5, Follow._ASSIGN_in_assignment321);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(Follow._obj_in_assignment323);
					astParserRuleReturnScope3 = obj();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream2.Add(astParserRuleReturnScope3.Tree);
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(5, "ASSIGN"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream2.NextTree());
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("parameters")]
		private AstParserRuleReturnScope<CommonTree, IToken> parameters()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			CommonTree commonTree2 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token COMMA");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule obj");
			try
			{
				try
				{
					int num = 2;
					try
					{
						try
						{
							int num2 = input.LA(1);
							if (num2 == 19 || num2 == 23 || num2 == 26 || num2 == 44 || (num2 >= 59 && num2 <= 61))
							{
								num = 1;
							}
						}
						finally
						{
						}
						int num3 = num;
						if (num3 == 1)
						{
							PushFollow(Follow._obj_in_parameters345);
							astParserRuleReturnScope2 = obj();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
							}
							try
							{
								while (true)
								{
									int num4 = 2;
									try
									{
										int num5 = input.LA(1);
										if (num5 == 10)
										{
											num4 = 1;
										}
									}
									finally
									{
									}
									int num6 = num4;
									if (num6 != 1)
									{
										break;
									}
									token = (IToken)Match(input, 10, Follow._COMMA_in_parameters348);
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleTokenStream.Add(token);
									}
									PushFollow(Follow._obj_in_parameters350);
									astParserRuleReturnScope3 = obj();
									PopFollow();
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleSubtreeStream.Add(astParserRuleReturnScope3.Tree);
									}
								}
							}
							finally
							{
							}
						}
					}
					finally
					{
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						if (rewriteRuleSubtreeStream.HasNext)
						{
							CommonTree oldRoot = (CommonTree)adaptor.Nil();
							oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(36, "PARAMETERS"), oldRoot);
							while (rewriteRuleSubtreeStream.HasNext)
							{
								adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
							}
							rewriteRuleSubtreeStream.Reset();
							adaptor.AddChild(commonTree, oldRoot);
						}
						rewriteRuleSubtreeStream.Reset();
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("variable")]
		private AstParserRuleReturnScope<CommonTree, IToken> variable()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			IToken token4 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			CommonTree commonTree5 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LSQ");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RSQ");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token 58");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule value");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			try
			{
				try
				{
					token = (IToken)Match(input, 19, Follow._ID_in_variable375);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					int num = 2;
					try
					{
						try
						{
							int num2 = input.LA(1);
							if (num2 == 27)
							{
								num = 1;
							}
						}
						finally
						{
						}
						int num3 = num;
						if (num3 == 1)
						{
							token2 = (IToken)Match(input, 27, Follow._LSQ_in_variable378);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream2.Add(token2);
							}
							PushFollow(Follow._value_in_variable382);
							astParserRuleReturnScope2 = value();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
							}
							token3 = (IToken)Match(input, 40, Follow._RSQ_in_variable384);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream3.Add(token3);
							}
						}
					}
					finally
					{
					}
					int num4 = 2;
					try
					{
						try
						{
							int num5 = input.LA(1);
							if (num5 == 58)
							{
								num4 = 1;
							}
						}
						finally
						{
						}
						int num3 = num4;
						if (num3 == 1)
						{
							token4 = (IToken)Match(input, 58, Follow._58_in_variable389);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream4.Add(token4);
							}
							PushFollow(Follow._variable_in_variable393);
							astParserRuleReturnScope3 = variable();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream2.Add(astParserRuleReturnScope3.Tree);
							}
						}
					}
					finally
					{
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule id", (astParserRuleReturnScope2 == null) ? null : astParserRuleReturnScope2.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule mem", (astParserRuleReturnScope3 == null) ? null : astParserRuleReturnScope3.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(55, "VAR"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleTokenStream.NextNode());
						if (rewriteRuleSubtreeStream3.HasNext)
						{
							CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
							oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(22, "INDEX"), oldRoot2);
							adaptor.AddChild(oldRoot2, rewriteRuleSubtreeStream3.NextTree());
							adaptor.AddChild(oldRoot, oldRoot2);
						}
						rewriteRuleSubtreeStream3.Reset();
						if (rewriteRuleSubtreeStream4.HasNext)
						{
							CommonTree oldRoot3 = (CommonTree)adaptor.Nil();
							oldRoot3 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(29, "MEMBER"), oldRoot3);
							adaptor.AddChild(oldRoot3, rewriteRuleSubtreeStream4.NextTree());
							adaptor.AddChild(oldRoot, oldRoot3);
						}
						rewriteRuleSubtreeStream4.Reset();
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("obj")]
		private AstParserRuleReturnScope<CommonTree, IToken> obj()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			try
			{
				try
				{
					int num = 2;
					try
					{
						switch (input.LA(1))
						{
						case 23:
						{
							int num5 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 44:
						{
							int num2 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 60:
						{
							int num6 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 61:
						{
							int num8 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 59:
						{
							int num3 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 19:
						{
							int num7 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						case 26:
						{
							int num4 = input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						}
						default:
							if (state.backtracking <= 0)
							{
								NoViableAltException ex = new NoViableAltException(string.Empty, 10, 0, input, 1);
								throw ex;
							}
							state.failed = true;
							return astParserRuleReturnScope;
						}
					}
					finally
					{
					}
					switch (num)
					{
					case 1:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._op_compare_in_obj431);
						astParserRuleReturnScope2 = op_compare();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
						}
						break;
					case 2:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._value_in_obj437);
						astParserRuleReturnScope3 = value();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
						}
						break;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex2)
				{
					ReportError(ex2);
					Recover(input, ex2);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex2);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("value")]
		private AstParserRuleReturnScope<CommonTree, IToken> value()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			IToken token4 = null;
			IToken token5 = null;
			IToken token6 = null;
			IToken token7 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			CommonTree commonTree5 = null;
			CommonTree commonTree6 = null;
			CommonTree commonTree7 = null;
			CommonTree commonTree8 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token INT");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token STRING");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token 60");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token 61");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token 59");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule function");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule op_compare");
			try
			{
				try
				{
					int num = 8;
					try
					{
						switch (input.LA(1))
						{
						case 23:
							num = 1;
							break;
						case 44:
							num = 2;
							break;
						case 60:
							num = 3;
							break;
						case 61:
							num = 4;
							break;
						case 59:
							num = 5;
							break;
						case 19:
						{
							int num2 = input.LA(2);
							switch (num2)
							{
							case 26:
								num = 7;
								break;
							default:
								if (num2 != 25 && (num2 < 27 || num2 > 28) && num2 != 30 && num2 != 35 && num2 != 37 && (num2 < 39 || num2 > 41) && num2 != 46 && num2 != 58)
								{
									if (state.backtracking <= 0)
									{
										NoViableAltException ex2 = new NoViableAltException(string.Empty, 11, 6, input, 2);
										throw ex2;
									}
									state.failed = true;
									return astParserRuleReturnScope;
								}
								goto case -1;
							case -1:
							case 4:
							case 10:
							case 12:
							case 14:
							case 16:
							case 17:
								num = 6;
								break;
							}
							break;
						}
						case 26:
							num = 8;
							break;
						default:
							if (state.backtracking <= 0)
							{
								NoViableAltException ex = new NoViableAltException(string.Empty, 11, 0, input, 1);
								throw ex;
							}
							state.failed = true;
							return astParserRuleReturnScope;
						}
					}
					finally
					{
					}
					switch (num)
					{
					case 1:
						token = (IToken)Match(input, 23, Follow._INT_in_value448);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream.Add(token);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
							oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(50, "TYPEINT"), oldRoot2);
							adaptor.AddChild(oldRoot2, rewriteRuleTokenStream.NextNode());
							adaptor.AddChild(commonTree, oldRoot2);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 2:
						token2 = (IToken)Match(input, 44, Follow._STRING_in_value462);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream2.Add(token2);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream10 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot7 = (CommonTree)adaptor.Nil();
							oldRoot7 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(53, "TYPESTRING"), oldRoot7);
							adaptor.AddChild(oldRoot7, rewriteRuleTokenStream2.NextNode());
							adaptor.AddChild(commonTree, oldRoot7);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 3:
						token3 = (IToken)Match(input, 60, Follow._60_in_value476);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream3.Add(token3);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream11 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot8 = (CommonTree)adaptor.Nil();
							oldRoot8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(52, "TYPENULL"), oldRoot8);
							adaptor.AddChild(commonTree, oldRoot8);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 4:
						token4 = (IToken)Match(input, 61, Follow._61_in_value488);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream4.Add(token4);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream9 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot6 = (CommonTree)adaptor.Nil();
							oldRoot6 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(48, "TYPEBOOL"), oldRoot6);
							adaptor.AddChild(oldRoot6, (CommonTree)adaptor.Create(47, "TRUE"));
							adaptor.AddChild(commonTree, oldRoot6);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 5:
						token5 = (IToken)Match(input, 59, Follow._59_in_value502);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream5.Add(token5);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot4 = (CommonTree)adaptor.Nil();
							oldRoot4 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(48, "TYPEBOOL"), oldRoot4);
							adaptor.AddChild(oldRoot4, (CommonTree)adaptor.Create(15, "FALSE"));
							adaptor.AddChild(commonTree, oldRoot4);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 6:
						PushFollow(Follow._variable_in_value516);
						astParserRuleReturnScope2 = variable();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream.Add(astParserRuleReturnScope2.Tree);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot3 = (CommonTree)adaptor.Nil();
							oldRoot3 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(54, "TYPEVARIABLE"), oldRoot3);
							adaptor.AddChild(oldRoot3, rewriteRuleSubtreeStream.NextTree());
							adaptor.AddChild(commonTree, oldRoot3);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 7:
						PushFollow(Follow._function_in_value530);
						astParserRuleReturnScope3 = function();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream2.Add(astParserRuleReturnScope3.Tree);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot5 = (CommonTree)adaptor.Nil();
							oldRoot5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(49, "TYPEFUNCTION"), oldRoot5);
							adaptor.AddChild(oldRoot5, rewriteRuleSubtreeStream2.NextTree());
							adaptor.AddChild(commonTree, oldRoot5);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 8:
						token6 = (IToken)Match(input, 26, Follow._LPAREN_in_value544);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream6.Add(token6);
						}
						PushFollow(Follow._op_compare_in_value546);
						astParserRuleReturnScope4 = op_compare();
						PopFollow();
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleSubtreeStream3.Add(astParserRuleReturnScope4.Tree);
						}
						token7 = (IToken)Match(input, 39, Follow._RPAREN_in_value548);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream7.Add(token7);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot = (CommonTree)adaptor.Nil();
							oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(51, "TYPEMATH"), oldRoot);
							adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream3.NextTree());
							adaptor.AddChild(commonTree, oldRoot);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex3)
				{
					ReportError(ex3);
					Recover(input, ex3);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex3);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("op_mult")]
		private AstParserRuleReturnScope<CommonTree, IToken> op_mult()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			try
			{
				try
				{
					commonTree = (CommonTree)adaptor.Nil();
					PushFollow(Follow._value_in_op_mult567);
					astParserRuleReturnScope2 = value();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
					}
					try
					{
						while (true)
						{
							int num = 3;
							try
							{
								switch (input.LA(1))
								{
								case 46:
									num = 1;
									break;
								case 12:
									num = 2;
									break;
								}
							}
							finally
							{
							}
							switch (num)
							{
							case 1:
								token = (IToken)Match(input, 46, Follow._TIMES_in_op_mult574);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._value_in_op_mult577);
								astParserRuleReturnScope3 = value();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
								}
								continue;
							case 2:
								token2 = (IToken)Match(input, 12, Follow._DIVIDE_in_op_mult585);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._value_in_op_mult588);
								astParserRuleReturnScope4 = value();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
								}
								continue;
							}
							break;
						}
					}
					finally
					{
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("op_add")]
		private AstParserRuleReturnScope<CommonTree, IToken> op_add()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			try
			{
				try
				{
					commonTree = (CommonTree)adaptor.Nil();
					PushFollow(Follow._op_mult_in_op_add605);
					astParserRuleReturnScope2 = op_mult();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
					}
					try
					{
						while (true)
						{
							int num = 3;
							try
							{
								switch (input.LA(1))
								{
								case 37:
									num = 1;
									break;
								case 30:
									num = 2;
									break;
								}
							}
							finally
							{
							}
							switch (num)
							{
							case 1:
								token = (IToken)Match(input, 37, Follow._PLUS_in_op_add612);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._op_mult_in_op_add615);
								astParserRuleReturnScope3 = op_mult();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
								}
								continue;
							case 2:
								token2 = (IToken)Match(input, 30, Follow._MINUS_in_op_add623);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._op_mult_in_op_add626);
								astParserRuleReturnScope4 = op_mult();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
								}
								continue;
							}
							break;
						}
					}
					finally
					{
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("op_compare")]
		private AstParserRuleReturnScope<CommonTree, IToken> op_compare()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			IToken token4 = null;
			IToken token5 = null;
			IToken token6 = null;
			IToken token7 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope5 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope6 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope7 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope8 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope9 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			CommonTree commonTree5 = null;
			CommonTree commonTree6 = null;
			CommonTree commonTree7 = null;
			CommonTree commonTree8 = null;
			try
			{
				try
				{
					commonTree = (CommonTree)adaptor.Nil();
					PushFollow(Follow._op_add_in_op_compare643);
					astParserRuleReturnScope2 = op_add();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
					}
					try
					{
						while (true)
						{
							int num = 8;
							try
							{
								switch (input.LA(1))
								{
								case 4:
									num = 1;
									break;
								case 35:
									num = 2;
									break;
								case 25:
									num = 3;
									break;
								case 16:
									num = 4;
									break;
								case 28:
									num = 5;
									break;
								case 17:
									num = 6;
									break;
								case 14:
									num = 7;
									break;
								}
							}
							finally
							{
							}
							switch (num)
							{
							case 1:
								token = (IToken)Match(input, 4, Follow._AND_in_op_compare650);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare653);
								astParserRuleReturnScope3 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
								}
								continue;
							case 2:
								token2 = (IToken)Match(input, 35, Follow._OR_in_op_compare661);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare664);
								astParserRuleReturnScope4 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
								}
								continue;
							case 3:
								token3 = (IToken)Match(input, 25, Follow._LEQ_in_op_compare672);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree4 = (CommonTree)adaptor.Create(token3);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree4, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare675);
								astParserRuleReturnScope5 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope5.Tree);
								}
								continue;
							case 4:
								token4 = (IToken)Match(input, 16, Follow._GEQ_in_op_compare683);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree5 = (CommonTree)adaptor.Create(token4);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree5, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare686);
								astParserRuleReturnScope6 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope6.Tree);
								}
								continue;
							case 5:
								token5 = (IToken)Match(input, 28, Follow._LTHAN_in_op_compare694);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree6 = (CommonTree)adaptor.Create(token5);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree6, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare697);
								astParserRuleReturnScope7 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope7.Tree);
								}
								continue;
							case 6:
								token6 = (IToken)Match(input, 17, Follow._GTHAN_in_op_compare705);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree7 = (CommonTree)adaptor.Create(token6);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree7, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare708);
								astParserRuleReturnScope8 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope8.Tree);
								}
								continue;
							case 7:
								token7 = (IToken)Match(input, 14, Follow._EQ_in_op_compare716);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree8 = (CommonTree)adaptor.Create(token7);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree8, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare719);
								astParserRuleReturnScope9 = op_add();
								PopFollow();
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									adaptor.AddChild(commonTree, astParserRuleReturnScope9.Tree);
								}
								continue;
							}
							break;
						}
					}
					finally
					{
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex)
				{
					ReportError(ex);
					Recover(input, ex);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		[GrammarRule("if")]
		private AstParserRuleReturnScope<CommonTree, IToken> @if()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			IToken token2 = null;
			IToken token3 = null;
			IToken token4 = null;
			IToken token5 = null;
			IToken token6 = null;
			IToken token7 = null;
			IToken token8 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope5 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope6 = null;
			CommonTree commonTree2 = null;
			CommonTree commonTree3 = null;
			CommonTree commonTree4 = null;
			CommonTree commonTree5 = null;
			CommonTree commonTree6 = null;
			CommonTree commonTree7 = null;
			CommonTree commonTree8 = null;
			CommonTree commonTree9 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token 63");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token LCURL");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token RCURL");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token 62");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule obj");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule operations");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule operation");
			try
			{
				try
				{
					token = (IToken)Match(input, 63, Follow._63_in_if737);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 26, Follow._LPAREN_in_if739);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token2);
					}
					PushFollow(Follow._obj_in_if741);
					astParserRuleReturnScope6 = obj();
					PopFollow();
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleSubtreeStream.Add(astParserRuleReturnScope6.Tree);
					}
					token3 = (IToken)Match(input, 39, Follow._RPAREN_in_if743);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token3);
					}
					int num = 2;
					try
					{
						try
						{
							switch (input.LA(1))
							{
							case 24:
								num = 1;
								break;
							case 19:
							case 63:
								num = 2;
								break;
							default:
								if (state.backtracking <= 0)
								{
									NoViableAltException ex = new NoViableAltException(string.Empty, 15, 0, input, 1);
									throw ex;
								}
								state.failed = true;
								return astParserRuleReturnScope;
							}
						}
						finally
						{
						}
						switch (num)
						{
						case 1:
							token4 = (IToken)Match(input, 24, Follow._LCURL_in_if747);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream4.Add(token4);
							}
							PushFollow(Follow._operations_in_if751);
							astParserRuleReturnScope2 = operations();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream2.Add(astParserRuleReturnScope2.Tree);
							}
							token5 = (IToken)Match(input, 38, Follow._RCURL_in_if753);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream5.Add(token5);
							}
							break;
						case 2:
							PushFollow(Follow._operation_in_if760);
							astParserRuleReturnScope3 = operation();
							PopFollow();
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleSubtreeStream3.Add(astParserRuleReturnScope3.Tree);
							}
							break;
						}
					}
					finally
					{
					}
					int num2 = 2;
					try
					{
						try
						{
							int num3 = input.LA(1);
							if (num3 == 62)
							{
								int num4 = input.LA(2);
								if (EvaluatePredicate(synpred34_bgitest_fragment))
								{
									num2 = 1;
								}
							}
						}
						finally
						{
						}
						int num5 = num2;
						if (num5 == 1)
						{
							token6 = (IToken)Match(input, 62, Follow._62_in_if764);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream6.Add(token6);
							}
							int num6 = 2;
							try
							{
								try
								{
									switch (input.LA(1))
									{
									case 24:
										num6 = 1;
										break;
									case 19:
									case 63:
										num6 = 2;
										break;
									default:
										if (state.backtracking <= 0)
										{
											NoViableAltException ex2 = new NoViableAltException(string.Empty, 16, 0, input, 1);
											throw ex2;
										}
										state.failed = true;
										return astParserRuleReturnScope;
									}
								}
								finally
								{
								}
								switch (num6)
								{
								case 1:
									token7 = (IToken)Match(input, 24, Follow._LCURL_in_if768);
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleTokenStream4.Add(token7);
									}
									PushFollow(Follow._operations_in_if772);
									astParserRuleReturnScope4 = operations();
									PopFollow();
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleSubtreeStream2.Add(astParserRuleReturnScope4.Tree);
									}
									token8 = (IToken)Match(input, 38, Follow._RCURL_in_if774);
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleTokenStream5.Add(token8);
									}
									break;
								case 2:
									PushFollow(Follow._operation_in_if781);
									astParserRuleReturnScope5 = operation();
									PopFollow();
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleSubtreeStream3.Add(astParserRuleReturnScope5.Tree);
									}
									break;
								}
							}
							finally
							{
							}
						}
					}
					finally
					{
					}
					if (state.backtracking == 0)
					{
						astParserRuleReturnScope.Tree = commonTree;
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule ops1", (astParserRuleReturnScope2 == null) ? null : astParserRuleReturnScope2.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule op1", (astParserRuleReturnScope3 == null) ? null : astParserRuleReturnScope3.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule ops2", (astParserRuleReturnScope4 == null) ? null : astParserRuleReturnScope4.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule op2", (astParserRuleReturnScope5 == null) ? null : astParserRuleReturnScope5.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream8 = new RewriteRuleSubtreeStream(adaptor, "rule retval", (astParserRuleReturnScope == null) ? null : astParserRuleReturnScope.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(20, "IF"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
						oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(45, "THEN"), oldRoot2);
						if (rewriteRuleSubtreeStream4.HasNext)
						{
							adaptor.AddChild(oldRoot2, rewriteRuleSubtreeStream4.NextTree());
						}
						rewriteRuleSubtreeStream4.Reset();
						if (rewriteRuleSubtreeStream5.HasNext)
						{
							adaptor.AddChild(oldRoot2, rewriteRuleSubtreeStream5.NextTree());
						}
						rewriteRuleSubtreeStream5.Reset();
						adaptor.AddChild(oldRoot, oldRoot2);
						CommonTree oldRoot3 = (CommonTree)adaptor.Nil();
						oldRoot3 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(13, "ELSE"), oldRoot3);
						if (rewriteRuleSubtreeStream6.HasNext)
						{
							adaptor.AddChild(oldRoot3, rewriteRuleSubtreeStream6.NextTree());
						}
						rewriteRuleSubtreeStream6.Reset();
						if (rewriteRuleSubtreeStream7.HasNext)
						{
							adaptor.AddChild(oldRoot3, rewriteRuleSubtreeStream7.NextTree());
						}
						rewriteRuleSubtreeStream7.Reset();
						adaptor.AddChild(oldRoot, oldRoot3);
						adaptor.AddChild(commonTree, oldRoot);
						astParserRuleReturnScope.Tree = commonTree;
					}
					astParserRuleReturnScope.Stop = input.LT(-1);
					if (state.backtracking != 0)
					{
						return astParserRuleReturnScope;
					}
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.RulePostProcessing(commonTree);
					adaptor.SetTokenBoundaries(astParserRuleReturnScope.Tree, astParserRuleReturnScope.Start, astParserRuleReturnScope.Stop);
					return astParserRuleReturnScope;
				}
				catch (RecognitionException ex3)
				{
					ReportError(ex3);
					Recover(input, ex3);
					astParserRuleReturnScope.Tree = (CommonTree)adaptor.ErrorNode(input, astParserRuleReturnScope.Start, input.LT(-1), ex3);
					return astParserRuleReturnScope;
				}
				finally
				{
				}
			}
			finally
			{
			}
		}

		private void synpred3_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._operations_in_synpred3_bgitest188);
				operations();
				PopFollow();
				if (!state.failed)
				{
				}
			}
			finally
			{
			}
		}

		private void synpred8_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._parameters_in_synpred8_bgitest294);
				parameters();
				PopFollow();
				if (!state.failed)
				{
				}
			}
			finally
			{
			}
		}

		private void synpred13_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._op_compare_in_synpred13_bgitest431);
				op_compare();
				PopFollow();
				if (!state.failed)
				{
				}
			}
			finally
			{
			}
		}

		private void synpred34_bgitest_fragment()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			try
			{
				Match(input, 62, Follow._62_in_synpred34_bgitest764);
				if (!state.failed)
				{
					int num = 2;
					try
					{
						try
						{
							switch (input.LA(1))
							{
							case 24:
								num = 1;
								break;
							case 19:
							case 63:
								num = 2;
								break;
							default:
								if (state.backtracking <= 0)
								{
									NoViableAltException ex = new NoViableAltException(string.Empty, 19, 0, input, 1);
									throw ex;
								}
								state.failed = true;
								return;
							}
						}
						finally
						{
						}
						switch (num)
						{
						case 1:
							Match(input, 24, Follow._LCURL_in_synpred34_bgitest768);
							if (!state.failed)
							{
								PushFollow(Follow._operations_in_synpred34_bgitest772);
								astParserRuleReturnScope = operations();
								PopFollow();
								if (!state.failed)
								{
									Match(input, 38, Follow._RCURL_in_synpred34_bgitest774);
									if (!state.failed)
									{
									}
								}
							}
							break;
						case 2:
							PushFollow(Follow._operation_in_synpred34_bgitest781);
							astParserRuleReturnScope2 = operation();
							PopFollow();
							if (!state.failed)
							{
							}
							break;
						}
					}
					finally
					{
					}
				}
			}
			finally
			{
			}
		}

		private bool EvaluatePredicate(Action fragment)
		{
			bool result = false;
			state.backtracking++;
			try
			{
				int marker = input.Mark();
				try
				{
					fragment();
				}
				catch (RecognitionException arg)
				{
					Console.Error.WriteLine("impossible: " + arg);
				}
				result = !state.failed;
				input.Rewind(marker);
			}
			finally
			{
			}
			state.backtracking--;
			state.failed = false;
			return result;
		}
	}
}
