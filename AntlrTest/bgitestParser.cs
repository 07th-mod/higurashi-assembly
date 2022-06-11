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
			public static readonly BitSet _directive_in_program151 = new BitSet(new ulong[1]
			{
				1310722uL
			});

			public static readonly BitSet _block_in_program153 = new BitSet(new ulong[1]
			{
				1310722uL
			});

			public static readonly BitSet _HASH_in_directive166 = new BitSet(new ulong[2]
			{
				0uL,
				8uL
			});

			public static readonly BitSet _67_in_directive168 = new BitSet(new ulong[1]
			{
				35184372088832uL
			});

			public static readonly BitSet _STRING_in_directive170 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_block184 = new BitSet(new ulong[1]
			{
				1048576uL
			});

			public static readonly BitSet _ID_in_block188 = new BitSet(new ulong[1]
			{
				134217728uL
			});

			public static readonly BitSet _LPAREN_in_block190 = new BitSet(new ulong[1]
			{
				1099511627776uL
			});

			public static readonly BitSet _RPAREN_in_block192 = new BitSet(new ulong[1]
			{
				33554432uL
			});

			public static readonly BitSet _LCURL_in_block194 = new BitSet(new ulong[2]
			{
				549756862464uL,
				4uL
			});

			public static readonly BitSet _operations_in_block196 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RCURL_in_block199 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_operations223 = new BitSet(new ulong[2]
			{
				1048578uL,
				4uL
			});

			public static readonly BitSet _declaration_in_operation237 = new BitSet(new ulong[1]
			{
				4398046511104uL
			});

			public static readonly BitSet _SEMICOLON_in_operation239 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _if_in_operation246 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _function_in_operation252 = new BitSet(new ulong[1]
			{
				4398046511104uL
			});

			public static readonly BitSet _SEMICOLON_in_operation254 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _assignment_in_operation261 = new BitSet(new ulong[1]
			{
				4398046511104uL
			});

			public static readonly BitSet _SEMICOLON_in_operation263 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_declaration275 = new BitSet(new ulong[1]
			{
				1048576uL
			});

			public static readonly BitSet _variable_in_declaration277 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _ID_in_function298 = new BitSet(new ulong[1]
			{
				134217728uL
			});

			public static readonly BitSet _LPAREN_in_function300 = new BitSet(new ulong[2]
			{
				13835094341465931776uL,
				1uL
			});

			public static readonly BitSet _parameters_in_function302 = new BitSet(new ulong[1]
			{
				1099511627776uL
			});

			public static readonly BitSet _RPAREN_in_function305 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _variable_in_assignment327 = new BitSet(new ulong[1]
			{
				32uL
			});

			public static readonly BitSet _ASSIGN_in_assignment329 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _obj_in_assignment331 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _obj_in_parameters353 = new BitSet(new ulong[1]
			{
				1026uL
			});

			public static readonly BitSet _COMMA_in_parameters356 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _obj_in_parameters358 = new BitSet(new ulong[1]
			{
				1026uL
			});

			public static readonly BitSet _ID_in_variable383 = new BitSet(new ulong[1]
			{
				2305843009482129410uL
			});

			public static readonly BitSet _LSQ_in_variable386 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _value_in_variable390 = new BitSet(new ulong[1]
			{
				2199023255552uL
			});

			public static readonly BitSet _RSQ_in_variable392 = new BitSet(new ulong[1]
			{
				2305843009213693954uL
			});

			public static readonly BitSet _61_in_variable397 = new BitSet(new ulong[1]
			{
				1048576uL
			});

			public static readonly BitSet _variable_in_variable401 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _op_compare_in_obj439 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _value_in_obj445 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _MINUS_in_value456 = new BitSet(new ulong[1]
			{
				16777216uL
			});

			public static readonly BitSet _INT_in_value458 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _INT_in_value472 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _HEXINT_in_value488 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _STRING_in_value502 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _63_in_value516 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _64_in_value528 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _62_in_value542 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _variable_in_value556 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _function_in_value570 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _LPAREN_in_value584 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_compare_in_value586 = new BitSet(new ulong[1]
			{
				1099511627776uL
			});

			public static readonly BitSet _RPAREN_in_value588 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _value_in_op_mult607 = new BitSet(new ulong[1]
			{
				140737488359426uL
			});

			public static readonly BitSet _TIMES_in_op_mult614 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _value_in_op_mult617 = new BitSet(new ulong[1]
			{
				140737488359426uL
			});

			public static readonly BitSet _DIVIDE_in_op_mult625 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _value_in_op_mult628 = new BitSet(new ulong[1]
			{
				140737488359426uL
			});

			public static readonly BitSet _op_mult_in_op_add645 = new BitSet(new ulong[1]
			{
				277025390594uL
			});

			public static readonly BitSet _PLUS_in_op_add652 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_mult_in_op_add655 = new BitSet(new ulong[1]
			{
				277025390594uL
			});

			public static readonly BitSet _MINUS_in_op_add663 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_mult_in_op_add666 = new BitSet(new ulong[1]
			{
				277025390594uL
			});

			public static readonly BitSet _op_add_in_op_compare683 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _AND_in_op_compare690 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare693 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _OR_in_op_compare701 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare704 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _LEQ_in_op_compare712 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare715 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _GEQ_in_op_compare723 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare726 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _LTHAN_in_op_compare734 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare737 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _GTHAN_in_op_compare745 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare748 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _EQ_in_op_compare756 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _op_add_in_op_compare759 = new BitSet(new ulong[1]
			{
				69323669522uL
			});

			public static readonly BitSet _66_in_if778 = new BitSet(new ulong[1]
			{
				134217728uL
			});

			public static readonly BitSet _LPAREN_in_if780 = new BitSet(new ulong[2]
			{
				13835093241954304000uL,
				1uL
			});

			public static readonly BitSet _obj_in_if782 = new BitSet(new ulong[1]
			{
				1099511627776uL
			});

			public static readonly BitSet _RPAREN_in_if784 = new BitSet(new ulong[2]
			{
				34603008uL,
				4uL
			});

			public static readonly BitSet _LCURL_in_if788 = new BitSet(new ulong[2]
			{
				549756862464uL,
				4uL
			});

			public static readonly BitSet _operations_in_if792 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RCURL_in_if794 = new BitSet(new ulong[2]
			{
				2uL,
				2uL
			});

			public static readonly BitSet _operation_in_if801 = new BitSet(new ulong[2]
			{
				2uL,
				2uL
			});

			public static readonly BitSet _65_in_if805 = new BitSet(new ulong[2]
			{
				34603008uL,
				4uL
			});

			public static readonly BitSet _LCURL_in_if809 = new BitSet(new ulong[2]
			{
				549756862464uL,
				4uL
			});

			public static readonly BitSet _operations_in_if813 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RCURL_in_if815 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_if822 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operations_in_synpred3_bgitest196 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _parameters_in_synpred8_bgitest302 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _op_compare_in_synpred13_bgitest439 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _65_in_synpred36_bgitest805 = new BitSet(new ulong[2]
			{
				34603008uL,
				4uL
			});

			public static readonly BitSet _LCURL_in_synpred36_bgitest809 = new BitSet(new ulong[2]
			{
				549756862464uL,
				4uL
			});

			public static readonly BitSet _operations_in_synpred36_bgitest813 = new BitSet(new ulong[1]
			{
				549755813888uL
			});

			public static readonly BitSet _RCURL_in_synpred36_bgitest815 = new BitSet(new ulong[1]
			{
				2uL
			});

			public static readonly BitSet _operation_in_synpred36_bgitest822 = new BitSet(new ulong[1]
			{
				2uL
			});
		}

		internal static readonly string[] tokenNames = new string[68]
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
			"HEXINT",
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
			"TYPEHEX",
			"TYPEINT",
			"TYPEMATH",
			"TYPENULL",
			"TYPESTRING",
			"TYPEUNARY",
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

		public const int HEXINT = 19;

		public const int ID = 20;

		public const int IF = 21;

		public const int INCLUDE = 22;

		public const int INDEX = 23;

		public const int INT = 24;

		public const int LCURL = 25;

		public const int LEQ = 26;

		public const int LPAREN = 27;

		public const int LSQ = 28;

		public const int LTHAN = 29;

		public const int MEMBER = 30;

		public const int MINUS = 31;

		public const int MOD = 32;

		public const int NEQ = 33;

		public const int NOT = 34;

		public const int OPERATION = 35;

		public const int OR = 36;

		public const int PARAMETERS = 37;

		public const int PLUS = 38;

		public const int RCURL = 39;

		public const int RPAREN = 40;

		public const int RSQ = 41;

		public const int SEMICOLON = 42;

		public const int SHIFT_LEFT = 43;

		public const int SHIFT_RIGHT = 44;

		public const int STRING = 45;

		public const int THEN = 46;

		public const int TIMES = 47;

		public const int TRUE = 48;

		public const int TYPEBOOL = 49;

		public const int TYPEFUNCTION = 50;

		public const int TYPEHEX = 51;

		public const int TYPEINT = 52;

		public const int TYPEMATH = 53;

		public const int TYPENULL = 54;

		public const int TYPESTRING = 55;

		public const int TYPEUNARY = 56;

		public const int TYPEVARIABLE = 57;

		public const int VAR = 58;

		public const int VARDECL = 59;

		public const int WS = 60;

		public const int T__61 = 61;

		public const int T__62 = 62;

		public const int T__63 = 63;

		public const int T__64 = 64;

		public const int T__65 = 65;

		public const int T__66 = 66;

		public const int T__67 = 67;

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

		public override string GrammarFileName => "D:\\Projects\\higurashi-ch-6-tsumihoroboshi\\Assets\\Scripts\\Compiler\\BGICompiler\\Grammar\\bgitest.g";

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
								case 20:
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
								PushFollow(Follow._directive_in_program151);
								astParserRuleReturnScope2 = directive();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope2.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 2:
								PushFollow(Follow._block_in_program153);
								astParserRuleReturnScope3 = block();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
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
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token HASH");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token 67");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token STRING");
			try
			{
				try
				{
					token = (IToken)Match(input, 18, Follow._HASH_in_directive166);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 67, Follow._67_in_directive168);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token2);
					}
					token3 = (IToken)Match(input, 45, Follow._STRING_in_directive170);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
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
					token2 = (IToken)Match(input, 20, Follow._ID_in_block184);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token2);
					}
					token = (IToken)Match(input, 20, Follow._ID_in_block188);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token3 = (IToken)Match(input, 27, Follow._LPAREN_in_block190);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token3);
					}
					token4 = (IToken)Match(input, 40, Follow._RPAREN_in_block192);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream3.Add(token4);
					}
					token5 = (IToken)Match(input, 25, Follow._LCURL_in_block194);
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
							case 20:
							case 66:
								num = 1;
								break;
							case 39:
								input.LA(2);
								if (EvaluatePredicate(synpred3_bgitest_fragment))
								{
									num = 1;
								}
								break;
							}
						}
						finally
						{
						}
						if (num == 1)
						{
							PushFollow(Follow._operations_in_block196);
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
					token6 = (IToken)Match(input, 39, Follow._RCURL_in_block199);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
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
								if (num2 == 20 || num2 == 66)
								{
									num = 1;
								}
							}
							finally
							{
							}
							if (num != 1)
							{
								break;
							}
							PushFollow(Follow._operation_in_operations223);
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
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope5 = null;
			try
			{
				try
				{
					int num = 4;
					try
					{
						switch (input.LA(1))
						{
						case 20:
							switch (input.LA(2))
							{
							case 27:
								num = 3;
								break;
							case 20:
								num = 1;
								break;
							case 5:
							case 28:
							case 61:
								num = 4;
								break;
							default:
								if (state.backtracking <= 0)
								{
									throw new NoViableAltException("", 4, 1, input, 2);
								}
								state.failed = true;
								return astParserRuleReturnScope;
							}
							break;
						case 66:
							num = 2;
							break;
						default:
							if (state.backtracking <= 0)
							{
								throw new NoViableAltException("", 4, 0, input, 1);
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
						PushFollow(Follow._declaration_in_operation237);
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
						_ = (IToken)Match(input, 42, Follow._SEMICOLON_in_operation239);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						break;
					case 2:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._if_in_operation246);
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
						PushFollow(Follow._function_in_operation252);
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
						_ = (IToken)Match(input, 42, Follow._SEMICOLON_in_operation254);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						break;
					case 4:
						commonTree = (CommonTree)adaptor.Nil();
						PushFollow(Follow._assignment_in_operation261);
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
						_ = (IToken)Match(input, 42, Follow._SEMICOLON_in_operation263);
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

		[GrammarRule("declaration")]
		private AstParserRuleReturnScope<CommonTree, IToken> declaration()
		{
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope = new AstParserRuleReturnScope<CommonTree, IToken>();
			astParserRuleReturnScope.Start = input.LT(1);
			CommonTree commonTree = null;
			IToken token = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			try
			{
				try
				{
					token = (IToken)Match(input, 20, Follow._ID_in_declaration275);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(Follow._variable_in_declaration277);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(59, "VARDECL"), oldRoot);
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
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule parameters");
			try
			{
				try
				{
					token = (IToken)Match(input, 20, Follow._ID_in_function298);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 27, Follow._LPAREN_in_function300);
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
							if (num2 < 19 || num2 > 20)
							{
								switch (num2)
								{
								case 24:
								case 27:
								case 31:
								case 45:
								case 62:
								case 63:
								case 64:
									break;
								default:
									if (num2 == 40)
									{
										input.LA(2);
										if (EvaluatePredicate(synpred8_bgitest_fragment))
										{
											num = 1;
										}
									}
									goto end_IL_00f5;
								}
							}
							num = 1;
							end_IL_00f5:;
						}
						finally
						{
						}
						if (num == 1)
						{
							PushFollow(Follow._parameters_in_function302);
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
					token3 = (IToken)Match(input, 40, Follow._RPAREN_in_function305);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(35, "OPERATION"), oldRoot);
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
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ASSIGN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule obj");
			try
			{
				try
				{
					PushFollow(Follow._variable_in_assignment327);
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
					token = (IToken)Match(input, 5, Follow._ASSIGN_in_assignment329);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					PushFollow(Follow._obj_in_assignment331);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
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
							if (num2 < 19 || num2 > 20)
							{
								switch (num2)
								{
								default:
									goto end_IL_0048;
								case 24:
								case 27:
								case 31:
								case 45:
								case 62:
								case 63:
								case 64:
									break;
								}
							}
							num = 1;
							end_IL_0048:;
						}
						finally
						{
						}
						if (num == 1)
						{
							PushFollow(Follow._obj_in_parameters353);
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
									int num3 = 2;
									try
									{
										if (input.LA(1) == 10)
										{
											num3 = 1;
										}
									}
									finally
									{
									}
									if (num3 != 1)
									{
										break;
									}
									token = (IToken)Match(input, 10, Follow._COMMA_in_parameters356);
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleTokenStream.Add(token);
									}
									PushFollow(Follow._obj_in_parameters358);
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
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						if (rewriteRuleSubtreeStream.HasNext)
						{
							CommonTree oldRoot = (CommonTree)adaptor.Nil();
							oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(37, "PARAMETERS"), oldRoot);
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
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token ID");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LSQ");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RSQ");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token 61");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule value");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			try
			{
				try
				{
					token = (IToken)Match(input, 20, Follow._ID_in_variable383);
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
							if (input.LA(1) == 28)
							{
								num = 1;
							}
						}
						finally
						{
						}
						if (num == 1)
						{
							token2 = (IToken)Match(input, 28, Follow._LSQ_in_variable386);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream2.Add(token2);
							}
							PushFollow(Follow._value_in_variable390);
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
							token3 = (IToken)Match(input, 41, Follow._RSQ_in_variable392);
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
					int num2 = 2;
					try
					{
						try
						{
							if (input.LA(1) == 61)
							{
								num2 = 1;
							}
						}
						finally
						{
						}
						if (num2 == 1)
						{
							token4 = (IToken)Match(input, 61, Follow._61_in_variable397);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream4.Add(token4);
							}
							PushFollow(Follow._variable_in_variable401);
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
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule id", astParserRuleReturnScope2?.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule mem", astParserRuleReturnScope3?.Tree);
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(58, "VAR"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleTokenStream.NextNode());
						if (rewriteRuleSubtreeStream3.HasNext)
						{
							CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
							oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(23, "INDEX"), oldRoot2);
							adaptor.AddChild(oldRoot2, rewriteRuleSubtreeStream3.NextTree());
							adaptor.AddChild(oldRoot, oldRoot2);
						}
						rewriteRuleSubtreeStream3.Reset();
						if (rewriteRuleSubtreeStream4.HasNext)
						{
							CommonTree oldRoot3 = (CommonTree)adaptor.Nil();
							oldRoot3 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(30, "MEMBER"), oldRoot3);
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
						case 31:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 24:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 19:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 45:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 63:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 64:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 62:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 20:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						case 27:
							input.LA(2);
							num = (EvaluatePredicate(synpred13_bgitest_fragment) ? 1 : 2);
							break;
						default:
							if (state.backtracking <= 0)
							{
								throw new NoViableAltException("", 10, 0, input, 1);
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
						PushFollow(Follow._op_compare_in_obj439);
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
						PushFollow(Follow._value_in_obj445);
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
			IToken token8 = null;
			IToken token9 = null;
			IToken token10 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope2 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope3 = null;
			AstParserRuleReturnScope<CommonTree, IToken> astParserRuleReturnScope4 = null;
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token MINUS");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token INT");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token HEXINT");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token STRING");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token 63");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token 64");
			RewriteRuleTokenStream rewriteRuleTokenStream7 = new RewriteRuleTokenStream(adaptor, "token 62");
			RewriteRuleTokenStream rewriteRuleTokenStream8 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream9 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule variable");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule function");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule op_compare");
			try
			{
				try
				{
					int num = 10;
					try
					{
						switch (input.LA(1))
						{
						case 31:
							num = 1;
							break;
						case 24:
							num = 2;
							break;
						case 19:
							num = 3;
							break;
						case 45:
							num = 4;
							break;
						case 63:
							num = 5;
							break;
						case 64:
							num = 6;
							break;
						case 62:
							num = 7;
							break;
						case 20:
						{
							int num2 = input.LA(2);
							switch (num2)
							{
							case 27:
								num = 9;
								goto end_IL_011b;
							default:
								switch (num2)
								{
								default:
									switch (num2)
									{
									default:
										if (num2 != 47 && num2 != 61)
										{
											if (state.backtracking <= 0)
											{
												throw new NoViableAltException("", 11, 8, input, 2);
											}
											state.failed = true;
											return astParserRuleReturnScope;
										}
										break;
									case 31:
									case 36:
									case 38:
									case 40:
									case 41:
									case 42:
										break;
									}
									break;
								case 26:
								case 28:
								case 29:
									break;
								}
								break;
							case -1:
							case 4:
							case 10:
							case 12:
							case 14:
							case 16:
							case 17:
								break;
							}
							num = 8;
							break;
						}
						case 27:
							num = 10;
							break;
						default:
							if (state.backtracking <= 0)
							{
								throw new NoViableAltException("", 11, 0, input, 1);
							}
							state.failed = true;
							return astParserRuleReturnScope;
						}
						end_IL_011b:;
					}
					finally
					{
					}
					switch (num)
					{
					case 1:
						token = (IToken)Match(input, 31, Follow._MINUS_in_value456);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream.Add(token);
						}
						token2 = (IToken)Match(input, 24, Follow._INT_in_value458);
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
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot9 = (CommonTree)adaptor.Nil();
							oldRoot9 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(56, "TYPEUNARY"), oldRoot9);
							adaptor.AddChild(oldRoot9, rewriteRuleTokenStream2.NextNode());
							adaptor.AddChild(commonTree, oldRoot9);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 2:
						token3 = (IToken)Match(input, 24, Follow._INT_in_value472);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream2.Add(token3);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot7 = (CommonTree)adaptor.Nil();
							oldRoot7 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(52, "TYPEINT"), oldRoot7);
							adaptor.AddChild(oldRoot7, rewriteRuleTokenStream2.NextNode());
							adaptor.AddChild(commonTree, oldRoot7);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 3:
						token4 = (IToken)Match(input, 19, Follow._HEXINT_in_value488);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream3.Add(token4);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot6 = (CommonTree)adaptor.Nil();
							oldRoot6 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(51, "TYPEHEX"), oldRoot6);
							adaptor.AddChild(oldRoot6, rewriteRuleTokenStream3.NextNode());
							adaptor.AddChild(commonTree, oldRoot6);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 4:
						token5 = (IToken)Match(input, 45, Follow._STRING_in_value502);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream4.Add(token5);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot4 = (CommonTree)adaptor.Nil();
							oldRoot4 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(55, "TYPESTRING"), oldRoot4);
							adaptor.AddChild(oldRoot4, rewriteRuleTokenStream4.NextNode());
							adaptor.AddChild(commonTree, oldRoot4);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 5:
						token6 = (IToken)Match(input, 63, Follow._63_in_value516);
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
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot5 = (CommonTree)adaptor.Nil();
							oldRoot5 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(54, "TYPENULL"), oldRoot5);
							adaptor.AddChild(commonTree, oldRoot5);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 6:
						token7 = (IToken)Match(input, 64, Follow._64_in_value528);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream6.Add(token7);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
							oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(49, "TYPEBOOL"), oldRoot2);
							adaptor.AddChild(oldRoot2, (CommonTree)adaptor.Create(48, "TRUE"));
							adaptor.AddChild(commonTree, oldRoot2);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 7:
						token8 = (IToken)Match(input, 62, Follow._62_in_value542);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream7.Add(token8);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot3 = (CommonTree)adaptor.Nil();
							oldRoot3 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(49, "TYPEBOOL"), oldRoot3);
							adaptor.AddChild(oldRoot3, (CommonTree)adaptor.Create(15, "FALSE"));
							adaptor.AddChild(commonTree, oldRoot3);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 8:
						PushFollow(Follow._variable_in_value556);
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
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot10 = (CommonTree)adaptor.Nil();
							oldRoot10 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(57, "TYPEVARIABLE"), oldRoot10);
							adaptor.AddChild(oldRoot10, rewriteRuleSubtreeStream.NextTree());
							adaptor.AddChild(commonTree, oldRoot10);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 9:
						PushFollow(Follow._function_in_value570);
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
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot8 = (CommonTree)adaptor.Nil();
							oldRoot8 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(50, "TYPEFUNCTION"), oldRoot8);
							adaptor.AddChild(oldRoot8, rewriteRuleSubtreeStream2.NextTree());
							adaptor.AddChild(commonTree, oldRoot8);
							astParserRuleReturnScope.Tree = commonTree;
						}
						break;
					case 10:
						token9 = (IToken)Match(input, 27, Follow._LPAREN_in_value584);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream8.Add(token9);
						}
						PushFollow(Follow._op_compare_in_value586);
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
						token10 = (IToken)Match(input, 40, Follow._RPAREN_in_value588);
						if (state.failed)
						{
							return astParserRuleReturnScope;
						}
						if (state.backtracking == 0)
						{
							rewriteRuleTokenStream9.Add(token10);
						}
						if (state.backtracking == 0)
						{
							astParserRuleReturnScope.Tree = commonTree;
							new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
							commonTree = (CommonTree)adaptor.Nil();
							CommonTree oldRoot = (CommonTree)adaptor.Nil();
							oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(53, "TYPEMATH"), oldRoot);
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
					PushFollow(Follow._value_in_op_mult607);
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
								case 47:
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
								token = (IToken)Match(input, 47, Follow._TIMES_in_op_mult614);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._value_in_op_mult617);
								astParserRuleReturnScope3 = value();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 2:
								token2 = (IToken)Match(input, 12, Follow._DIVIDE_in_op_mult625);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._value_in_op_mult628);
								astParserRuleReturnScope4 = value();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
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
					PushFollow(Follow._op_mult_in_op_add645);
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
								case 38:
									num = 1;
									break;
								case 31:
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
								token = (IToken)Match(input, 38, Follow._PLUS_in_op_add652);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._op_mult_in_op_add655);
								astParserRuleReturnScope3 = op_mult();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 2:
								token2 = (IToken)Match(input, 31, Follow._MINUS_in_op_add663);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._op_mult_in_op_add666);
								astParserRuleReturnScope4 = op_mult();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
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
					PushFollow(Follow._op_add_in_op_compare683);
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
								case 36:
									num = 2;
									break;
								case 26:
									num = 3;
									break;
								case 16:
									num = 4;
									break;
								case 29:
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
								token = (IToken)Match(input, 4, Follow._AND_in_op_compare690);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree2 = (CommonTree)adaptor.Create(token);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree2, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare693);
								astParserRuleReturnScope3 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope3.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 2:
								token2 = (IToken)Match(input, 36, Follow._OR_in_op_compare701);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree3 = (CommonTree)adaptor.Create(token2);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree3, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare704);
								astParserRuleReturnScope4 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope4.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 3:
								token3 = (IToken)Match(input, 26, Follow._LEQ_in_op_compare712);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree4 = (CommonTree)adaptor.Create(token3);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree4, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare715);
								astParserRuleReturnScope5 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope5.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 4:
								token4 = (IToken)Match(input, 16, Follow._GEQ_in_op_compare723);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree5 = (CommonTree)adaptor.Create(token4);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree5, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare726);
								astParserRuleReturnScope6 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope6.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 5:
								token5 = (IToken)Match(input, 29, Follow._LTHAN_in_op_compare734);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree6 = (CommonTree)adaptor.Create(token5);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree6, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare737);
								astParserRuleReturnScope7 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope7.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 6:
								token6 = (IToken)Match(input, 17, Follow._GTHAN_in_op_compare745);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree7 = (CommonTree)adaptor.Create(token6);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree7, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare748);
								astParserRuleReturnScope8 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope8.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
							case 7:
								token7 = (IToken)Match(input, 14, Follow._EQ_in_op_compare756);
								if (state.failed)
								{
									return astParserRuleReturnScope;
								}
								if (state.backtracking == 0)
								{
									commonTree8 = (CommonTree)adaptor.Create(token7);
									commonTree = (CommonTree)adaptor.BecomeRoot(commonTree8, commonTree);
								}
								PushFollow(Follow._op_add_in_op_compare759);
								astParserRuleReturnScope9 = op_add();
								PopFollow();
								if (!state.failed)
								{
									if (state.backtracking == 0)
									{
										adaptor.AddChild(commonTree, astParserRuleReturnScope9.Tree);
									}
									continue;
								}
								return astParserRuleReturnScope;
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
			RewriteRuleTokenStream rewriteRuleTokenStream = new RewriteRuleTokenStream(adaptor, "token 66");
			RewriteRuleTokenStream rewriteRuleTokenStream2 = new RewriteRuleTokenStream(adaptor, "token LPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream3 = new RewriteRuleTokenStream(adaptor, "token RPAREN");
			RewriteRuleTokenStream rewriteRuleTokenStream4 = new RewriteRuleTokenStream(adaptor, "token LCURL");
			RewriteRuleTokenStream rewriteRuleTokenStream5 = new RewriteRuleTokenStream(adaptor, "token RCURL");
			RewriteRuleTokenStream rewriteRuleTokenStream6 = new RewriteRuleTokenStream(adaptor, "token 65");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream = new RewriteRuleSubtreeStream(adaptor, "rule obj");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream2 = new RewriteRuleSubtreeStream(adaptor, "rule operations");
			RewriteRuleSubtreeStream rewriteRuleSubtreeStream3 = new RewriteRuleSubtreeStream(adaptor, "rule operation");
			try
			{
				try
				{
					token = (IToken)Match(input, 66, Follow._66_in_if778);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream.Add(token);
					}
					token2 = (IToken)Match(input, 27, Follow._LPAREN_in_if780);
					if (state.failed)
					{
						return astParserRuleReturnScope;
					}
					if (state.backtracking == 0)
					{
						rewriteRuleTokenStream2.Add(token2);
					}
					PushFollow(Follow._obj_in_if782);
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
					token3 = (IToken)Match(input, 40, Follow._RPAREN_in_if784);
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
							case 25:
								num = 1;
								break;
							case 20:
							case 66:
								num = 2;
								break;
							default:
								if (state.backtracking <= 0)
								{
									throw new NoViableAltException("", 15, 0, input, 1);
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
							token4 = (IToken)Match(input, 25, Follow._LCURL_in_if788);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream4.Add(token4);
							}
							PushFollow(Follow._operations_in_if792);
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
							token5 = (IToken)Match(input, 39, Follow._RCURL_in_if794);
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
							PushFollow(Follow._operation_in_if801);
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
							if (input.LA(1) == 65)
							{
								input.LA(2);
								if (EvaluatePredicate(synpred36_bgitest_fragment))
								{
									num2 = 1;
								}
							}
						}
						finally
						{
						}
						if (num2 == 1)
						{
							token6 = (IToken)Match(input, 65, Follow._65_in_if805);
							if (state.failed)
							{
								return astParserRuleReturnScope;
							}
							if (state.backtracking == 0)
							{
								rewriteRuleTokenStream6.Add(token6);
							}
							int num3 = 2;
							try
							{
								try
								{
									switch (input.LA(1))
									{
									case 25:
										num3 = 1;
										break;
									case 20:
									case 66:
										num3 = 2;
										break;
									default:
										if (state.backtracking <= 0)
										{
											throw new NoViableAltException("", 16, 0, input, 1);
										}
										state.failed = true;
										return astParserRuleReturnScope;
									}
								}
								finally
								{
								}
								switch (num3)
								{
								case 1:
									token7 = (IToken)Match(input, 25, Follow._LCURL_in_if809);
									if (state.failed)
									{
										return astParserRuleReturnScope;
									}
									if (state.backtracking == 0)
									{
										rewriteRuleTokenStream4.Add(token7);
									}
									PushFollow(Follow._operations_in_if813);
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
									token8 = (IToken)Match(input, 39, Follow._RCURL_in_if815);
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
									PushFollow(Follow._operation_in_if822);
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
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream4 = new RewriteRuleSubtreeStream(adaptor, "rule ops1", astParserRuleReturnScope2?.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream5 = new RewriteRuleSubtreeStream(adaptor, "rule op1", astParserRuleReturnScope3?.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream6 = new RewriteRuleSubtreeStream(adaptor, "rule ops2", astParserRuleReturnScope4?.Tree);
						RewriteRuleSubtreeStream rewriteRuleSubtreeStream7 = new RewriteRuleSubtreeStream(adaptor, "rule op2", astParserRuleReturnScope5?.Tree);
						new RewriteRuleSubtreeStream(adaptor, "rule retval", astParserRuleReturnScope?.Tree);
						commonTree = (CommonTree)adaptor.Nil();
						CommonTree oldRoot = (CommonTree)adaptor.Nil();
						oldRoot = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(21, "IF"), oldRoot);
						adaptor.AddChild(oldRoot, rewriteRuleSubtreeStream.NextTree());
						CommonTree oldRoot2 = (CommonTree)adaptor.Nil();
						oldRoot2 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(46, "THEN"), oldRoot2);
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

		private void synpred3_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._operations_in_synpred3_bgitest196);
				operations();
				PopFollow();
				_ = state.failed;
			}
			finally
			{
			}
		}

		private void synpred8_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._parameters_in_synpred8_bgitest302);
				parameters();
				PopFollow();
				_ = state.failed;
			}
			finally
			{
			}
		}

		private void synpred13_bgitest_fragment()
		{
			try
			{
				PushFollow(Follow._op_compare_in_synpred13_bgitest439);
				op_compare();
				PopFollow();
				_ = state.failed;
			}
			finally
			{
			}
		}

		private void synpred36_bgitest_fragment()
		{
			try
			{
				Match(input, 65, Follow._65_in_synpred36_bgitest805);
				if (!state.failed)
				{
					int num = 2;
					try
					{
						try
						{
							switch (input.LA(1))
							{
							case 25:
								num = 1;
								break;
							case 20:
							case 66:
								num = 2;
								break;
							default:
								if (state.backtracking > 0)
								{
									state.failed = true;
									return;
								}
								throw new NoViableAltException("", 19, 0, input, 1);
							}
						}
						finally
						{
						}
						switch (num)
						{
						case 1:
							Match(input, 25, Follow._LCURL_in_synpred36_bgitest809);
							if (!state.failed)
							{
								PushFollow(Follow._operations_in_synpred36_bgitest813);
								operations();
								PopFollow();
								if (!state.failed)
								{
									Match(input, 39, Follow._RCURL_in_synpred36_bgitest815);
									if (!state.failed)
									{
									}
								}
							}
							break;
						case 2:
							PushFollow(Follow._operation_in_synpred36_bgitest822);
							operation();
							PopFollow();
							_ = state.failed;
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
