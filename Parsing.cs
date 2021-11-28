
using System;

namespace RobotBrain {
 
    public class Parser {
        public static SyntaxTree parseLine (string line) {
            Parser parser = new Parser (line);

            return parseTopLevel (parser);
        }


        private Lexer lexer;
        private Token curTok;

        public Parser (string line) {
            this.lexer = new Lexer (line);
            this.curTok = lexer.nextToken ();
        }


        public Token current () {
            return curTok;
        }

        public Token nextToken  () {
            curTok = lexer.nextToken ();
            return curTok;
        }

        public void consume () {
            curTok = lexer.nextToken ();
        }



        public static SyntaxTree parseTopLevel (Parser parser) {
            Token tok = parser.current ();
            if (tok.isIdentifier () || tok.isKeyword ())
                return parseExpression (parser);
            else
                return null;

        }

        public static SyntaxTree parseExpression1 (Parser parser)
        {
            Token tok = parser.current ();
            parser.consume ();

            // If the token is a keyword, parse the appropriate keyword
            // expression.
            if (tok.match (Keyword.KeywordQuit))
                return new SyntaxTree.QuitExpr ();
            else if (tok.match (Keyword.KeywordStop))
                return new SyntaxTree.StopExpr ();

            else if (tok.match (Keyword.KeywordHelp))
                return new SyntaxTree.HelpExpr ();
            else if (tok.match (Keyword.KeywordLet))
                return parseLet (parser);
            else if (tok.match (Keyword.KeywordEcho))
                return parseEcho (parser);
            else if (tok.match (Keyword.KeywordLandmark))
                return new SyntaxTree.LandmarkExpr ();

            else if (tok.match (Keyword.KeywordRotate))
                return parseRotate (parser);
            else if (tok.match (Keyword.KeywordMove))
                return parseMove (parser);

            else if (tok.match (Keyword.KeywordBuy))
                return parseBuy (parser);
            else if (tok.match (Keyword.KeywordSell))
                return parseSell (parser);
            else if (tok.match (Keyword.KeywordInventory))
                return new SyntaxTree.InventoryExpr ();
            else if (tok.match (Keyword.KeywordMarketPrices))
                return new SyntaxTree.MarketPricesExpr ();
            else if (tok.match (Keyword.KeywordCityPrices))
                return new SyntaxTree.CityPricesExpr ();

            // If the token is an identifier but not a keyword, return it
            // as a variable.
            else if (tok.isIdentifier ())
                return new SyntaxTree.IdentifierExpr (tok.ident);

            else if (tok.isInteger ())
                return new SyntaxTree.IntLiteralExpr (tok.intVal);

            // If the token is an open-paren, parse a multi-expression.
            else if (tok.match (Symbol.SymOpenParen)) {
                SyntaxTree expr = parseExpression (parser);

                if (!parser.current ().match (Symbol.SymCloseParen))
                    return new SyntaxTree.SyntaxError
                        ("expected ')'");
                parser.consume ();

                return expr;
            }

            else
                return new SyntaxTree.SyntaxError
                    ("expected keyword or identifier");
        }

        public static SyntaxTree parseExpression (Parser parser)
        {
            SyntaxTree lexpr = parseExpression1 (parser);

            // Look at the next token.
            // If it's an '&&', then we continue the expression evaluation.
            // Otherwise, we're done.
            Token tok1 = parser.current ();

            if (tok1.match (Symbol.SymDoubleAnd)) {
                parser.consume ();
                SyntaxTree cexpr = parseExpression (parser);

                return new SyntaxTree.ContinueExpr (lexpr, cexpr);
            }

            else
                return lexpr;
        }

        public static SyntaxTree parseLet (Parser parser) {
            Token tokIdent = parser.current();
            if (!tokIdent.isIdentifier())
                return new SyntaxTree.SyntaxError("expected identifier");

            Token tokEqual = parser.nextToken();
            if (!tokEqual.match(Symbol.SymSingleEq))
                return new SyntaxTree.SyntaxError("expected '='");
            parser.consume();

            return new SyntaxTree.LetExpr
                (tokIdent.ident, parseExpression1 (parser));
        }

        public static SyntaxTree parseEcho (Parser parser) {
            Token tokIdent = parser.current ();
            if (!tokIdent.isIdentifier ())
                return new SyntaxTree.SyntaxError ("expected identifier");

            return new SyntaxTree.EchoExpr (tokIdent.ident);
        }

        public static SyntaxTree parseRotate (Parser parser) {
            Token tok = parser.current ();

            if (!(tok.isIdentifier () || tok.isInteger ()))
                return new SyntaxTree.SyntaxError
                    ("expected variable name or literal");

            return new SyntaxTree.RotateExpr
                (parseExpression1 (parser));
        }

        public static SyntaxTree  parseMove (Parser parser) {
            Token tok = parser.current ();

            if (!(tok.isIdentifier () || tok.isInteger ()))
                return new SyntaxTree.SyntaxError
                    ("expected variable name or literal");

            return new SyntaxTree.MoveExpr
                (parseExpression1(parser));
        }


        public static SyntaxTree parseBuy (Parser parser) {
            Token nameTok = parser.current ();
            if (!nameTok.isIdentifier ())
                return new SyntaxTree.SyntaxError
                    ("expected identifier");
            parser.consume ();
            
            return new SyntaxTree.BuyExpr
                (nameTok.ident, parseExpression1 (parser));
        }

        public static SyntaxTree parseSell (Parser parser) {
            Token nameTok = parser.current ();
            if (!nameTok.isIdentifier ())
                return new SyntaxTree.SyntaxError
                    ("expected identifier");
            parser.consume ();

            return new SyntaxTree.SellExpr
                (nameTok.ident, parseExpression1 (parser));
        }
    }
    
}
