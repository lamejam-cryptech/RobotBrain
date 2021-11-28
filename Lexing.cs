
using System;
using System.Collections;
using System.Collections.Generic;

namespace RobotBrain {
    
    public enum Keyword {
        KeywordQuit,
        KeywordStop,
        KeywordHelp,
        KeywordLet,
        KeywordEcho,
        KeywordLandmark,
        // Movement keywords
        KeywordRotate,
        KeywordMove,
        // Merchant keywords
        KeywordBuy,
        KeywordSell,
        KeywordInventory,
        KeywordMarketPrices,
        KeywordCityPrices
    }

    public enum Symbol {
        SymSingleEq,
        SymDoubleAnd,
        SymOpenParen,
        SymCloseParen
    }

    public enum TokenKind
    {
        TokKeyword,
        TokSymbol,
        TokInteger,
        TokIdentifier,
        TokLiteral,
        TokEOL
    }

    public class Token {
        

        public readonly TokenKind kind;

        public Token (TokenKind kind) {
            this.kind = kind;
        }

        public bool isEOL () {
            return this.kind == TokenKind.TokEOL;
        }

        public readonly int     pos;
        public readonly char    lit;

        public Token (int pos, char lit) {
            this.kind = TokenKind.TokLiteral;
            this.pos = pos;
            this.lit = lit;
        }


        public readonly Keyword keyword;

        public Token (Keyword kw) {
            this.kind = TokenKind.TokKeyword;
            this.keyword = kw;
        }

        public bool isKeyword () {
            return this.kind == TokenKind.TokKeyword;
        }

        public bool match (Keyword kw) {
            if (kind == TokenKind.TokKeyword)
                return this.keyword == kw;
            else
                return false;
        }


        public readonly Symbol sym;

        public Token (Symbol sym) {
            this.kind = TokenKind.TokSymbol;
            this.sym = sym;
        }

        public bool isSymbol () {
            return this.kind == TokenKind.TokSymbol;
        }

        public bool match (Symbol sym) {
            if (kind == TokenKind.TokSymbol)
                return this.sym == sym;
            else
                return false;
        }


        public readonly int intVal;

        public Token (int val) {
            this.kind = TokenKind.TokInteger;
            this.intVal = val;
        }

        public bool isInteger () {
            return this.kind == TokenKind.TokInteger;
        }


        public readonly Identifier ident;

        public Token (Identifier id) {
            this.kind = TokenKind.TokIdentifier;
            this.ident = id;
        }

        public bool isIdentifier () {
            return this.kind == TokenKind.TokIdentifier;
        }
    }


    public class Lexer {
        private Dictionary<string, Keyword> keywords;

        private string  source;
        private int     position;

        public Lexer (string source) {
            keywords = new Dictionary<string, Keyword> ();
            keywords.Add ("quit"        , Keyword.KeywordQuit);
            keywords.Add ("stop"        , Keyword.KeywordStop);
            keywords.Add ("help"        , Keyword.KeywordHelp);
            keywords.Add ("let"         , Keyword.KeywordLet);
            keywords.Add ("echo"        , Keyword.KeywordEcho);
            keywords.Add ("landmark"    , Keyword.KeywordLandmark);
            // Movement keywords
            keywords.Add ("rotate"      , Keyword.KeywordRotate);
            keywords.Add ("move"        , Keyword.KeywordMove);
            // Merchant keywords
            keywords.Add ("buy"         , Keyword.KeywordBuy);
            keywords.Add ("sell"        , Keyword.KeywordSell);
            keywords.Add ("inventory"   , Keyword.KeywordInventory);
            keywords.Add ("marketprices", Keyword.KeywordMarketPrices);
            keywords.Add ("cityprices"  , Keyword.KeywordCityPrices);

            this.source = source;
            this.position = 0;
        }

        public Token nextToken () {
            // Skip any whitespace before the token
            while (  source[position] == ' '
                  || source[position] == '\t' )
                position ++;

            // Read the token
            Token tok = null;

            if (  source[position] == '\r'
               || source[position] == '\n' )
                tok = new Token (TokenKind.TokEOL);

            else if (isLetter (source[position])) {
                Identifier ident = lexIdentifier();

                if (keywords.ContainsKey (ident.name))
                    tok = new Token (keywords[ident.name]);
                else
                    tok = new Token (ident);
            }

            else if (isNumber (source[position]) || source[position] == '-')
                tok = lexInteger ();

            else if (source[position] == '=') {
                position ++;
                tok = new Token (Symbol.SymSingleEq);
            }

            else if (source[position] == '&') {
                position ++;
                if (source[position] == '&') {
                    tok = new Token (Symbol.SymDoubleAnd);
                    position ++;
                }
            }

            else if (source[position] == '(') {
                position ++;
                tok = new Token (Symbol.SymOpenParen);
            }

            else if (source[position] == ')') {
                position ++;
                tok = new Token (Symbol.SymCloseParen);
            }

            else {
                position ++;
                tok = new Token (position, source[position]);
            }

            return tok;
        }

        public static bool isLetter (char c) {
            return Char.IsLetter (c);
        }

        public Identifier lexIdentifier () {
            string text = "";

            text += source[position];
            position ++;

            while (  isLetter (source[position])
                  || isNumber (source[position]) )
            {
                text += source[position];
                position ++;
            }

            return new Identifier (text);
        }

        public static bool isNumber (char c) {
            return Char.IsNumber (c);
        }

        public Token lexInteger () {
            string text = "";

            int sign = source[position] == '-' ? -1 : 1;
            if (sign < 0)
                position ++;

            while (isNumber (source[position])) {
                text += source[position];
                position ++;
            }

            int val = sign * int.Parse (text);
            return new Token (val);
        }
    }
    
}
