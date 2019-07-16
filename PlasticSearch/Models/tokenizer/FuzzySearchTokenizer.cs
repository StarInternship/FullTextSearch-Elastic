﻿using PlasticSearch.Models.search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlasticSearch.Models.tokenizer
{
    class FuzzySearchTokenizer : Tokenizer
    {
        private static readonly int FUZZINESS = 2;
        private static readonly IList<char> characters = new List<char>();

        static FuzzySearchTokenizer()
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                characters.Add(c);
            }
            for (char c = 'a'; c <= 'z'; c++)
            {
                characters.Add(c);
            }
        }

        public override ISet<string> TokenizeQuery(string text) => new HashSet<string>();
        public override List<string> Develope(string token)
        {
            List<String> developedTokens = new List<string>(new string[] { token }.ToList());
            for (int i = 0; i < FUZZINESS; i++)
            {
                List<String> tempDevelopedTokens = new List<string>();
                foreach (string developedToken in developedTokens)
                {
                    tempDevelopedTokens.AddRange(Insert(developedToken));
                    tempDevelopedTokens.AddRange(Delete(developedToken));
                    tempDevelopedTokens.AddRange(Substitute(developedToken));
                }
                developedTokens.AddRange(tempDevelopedTokens);
                developedTokens.Remove(token);
            }
            return developedTokens;
        }
        private LinkedList<String> Delete(String token)
        {
            LinkedList<String> deletes = new LinkedList<string>();

            for (int i = 0; i < token.Length; i++)
            {
                deletes.AddLast(token.Substring(0, i) + token.Substring(i + 1));
            }
            return deletes;
        }

        private LinkedList<String> Insert(String token)
        {
            LinkedList<String> inserts = new LinkedList<string>();

            for (int i = 0; i <= token.Length; i++)
            {
                foreach (char c in characters)
                {
                    if (i < token.Length && c == token[i]) continue;
                    inserts.AddLast(token.Substring(0, i) + c + token.Substring(i));
                }
            }
            return inserts;
        }

        private LinkedList<String> Substitute(String token)
        {
            LinkedList<String> substitutes = new LinkedList<string>();

            for (int i = 0; i < token.Length; i++)
            {
                foreach (char c in characters)
                {
                    if (c == token[i]) continue;
                    substitutes.AddLast(token.Substring(0, i) + c + token.Substring(i + 1));
                }
            }
            return substitutes;
        }

        public override void TokenizeData(string filePath, string text, IDictionary<string, InvertedIndex> data)
        {
        }
    }
}