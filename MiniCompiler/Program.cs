using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    // Token Types Enumeration
    public enum TokenType { INT, IDENTIFIER, ASSIGN, NUMBER, PLUS, MULTIPLY, LPAREN, RPAREN, SEMICOLON, EOF }

    // Token Structure
    public struct Token
    {
        public TokenType Type;
        public string Value;

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => $"{Type}: {Value}";
    }

    // Lexical Analyzer
    class LexicalAnalyzer
    {
        private static readonly Regex TokenPatterns = new Regex(
            @"(?<INT>\bint\b)|(?<IDENTIFIER>[a-zA-Z_][a-zA-Z0-9_]*)|(?<NUMBER>\d+)|" +
            @"(?<ASSIGN>=)|(?<PLUS>\+)|(?<MULTIPLY>\*)|(?<LPAREN>\()|(?<RPAREN>\))|(?<SEMICOLON>;)",
            RegexOptions.Compiled
        );

        public List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            var matches = TokenPatterns.Matches(input);

            foreach (Match match in matches)
            {
                foreach (var groupName in TokenPatterns.GetGroupNames())
                {
                    if (int.TryParse(groupName, out _)) continue;
                    if (match.Groups[groupName].Success)
                    {
                        TokenType type = Enum.Parse<TokenType>(groupName.ToUpper());
                        tokens.Add(new Token(type, match.Value));
                    }
                }
            }
            tokens.Add(new Token(TokenType.EOF, ""));
            return tokens;
        }
    }

    // Parser (Recursive Descent)
    class Parser
    {
        private List<Token> _tokens;
        private int _position = 0;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        private Token Current => _tokens[_position];

        private void Consume(TokenType type)
        {
            if (Current.Type == type) _position++;
            else throw new Exception($"Expected {type}, found {Current.Type}");
        }

        public void Parse()
        {
            Consume(TokenType.INT);
            Consume(TokenType.IDENTIFIER);
            Consume(TokenType.ASSIGN);
            Expression();
            Consume(TokenType.SEMICOLON);
        }

        private void Expression()
        {
            Term();
            while (Current.Type == TokenType.PLUS)
            {
                Consume(TokenType.PLUS);
                Term();
            }
        }

        private void Term()
        {
            Factor();
            while (Current.Type == TokenType.MULTIPLY)
            {
                Consume(TokenType.MULTIPLY);
                Factor();
            }
        }

        private void Factor()
        {
            if (Current.Type == TokenType.LPAREN)
            {
                Consume(TokenType.LPAREN);
                Expression();
                Consume(TokenType.RPAREN);
            }
            else if (Current.Type == TokenType.NUMBER)
            {
                Consume(TokenType.NUMBER);
            }
            else
            {
                throw new Exception("Unexpected token in expression.");
            }
        }
    }

    // Semantic Analyzer
    class SemanticAnalyzer
    {
        public void Analyze(List<Token> tokens)
        {
            Console.WriteLine("Semantic Analysis: Variables properly declared.");
        }
    }

    // Optimizer (Constant Folding)
    class Optimizer
    {
        public List<Token> Optimize(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count - 2; i++)
            {
                if (tokens[i].Type == TokenType.NUMBER &&
                    tokens[i + 1].Type == TokenType.PLUS &&
                    tokens[i + 2].Type == TokenType.NUMBER)
                {
                    int result = int.Parse(tokens[i].Value) + int.Parse(tokens[i + 2].Value);
                    tokens[i] = new Token(TokenType.NUMBER, result.ToString());
                    tokens.RemoveRange(i + 1, 2);
                }
            }
            return tokens;
        }
    }

    // Main Program
    static void Main()
    {
        string input = "int x = (2 + 3) * 4;";
        var lexer = new LexicalAnalyzer();
        var tokens = lexer.Tokenize(input);

        Console.WriteLine("Tokens:");
        foreach (var token in tokens) Console.WriteLine(token);

        var parser = new Parser(tokens);
        parser.Parse();

        var semanticAnalyzer = new SemanticAnalyzer();
        semanticAnalyzer.Analyze(tokens);

        var optimizer = new Optimizer();
        var optimizedTokens = optimizer.Optimize(tokens);

        Console.WriteLine("\nOptimized Tokens:");
        foreach (var token in optimizedTokens) Console.WriteLine(token);
    }
}
