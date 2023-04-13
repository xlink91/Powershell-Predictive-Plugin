using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowershellPlugin.Predictor
{
    public class PreffixTree
    {
        private Node _root; 

        public PreffixTree()
        {
            _root = new Node(' ');
        }

        public void Insert(string text)
        {
            var current = _root;
            var prevNode = current;
            for(int i=0; i<text.Length; ++i)
            {
                if (!current.ContainsChildren(text[i]))
                {
                    current.Children.Add(text[i], new Node(text[i]));
                }
                prevNode.IsWord = text[i] == ' ';
                prevNode = current;
                current = current.Children[text[i]];
            }
        }

        /// <summary>
        /// White multiple white spaces will be sanitized
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string[] GetPreffixPaths(string text)
        {
            return GetSuffixPaths(text);
        }

        public string[] GetPredictionForEmptyLine()
        {
             return GetWords(_root, 25);
        }

        private string[] GetSuffixPaths(string text)
        {
            var current = _root;
            for(int i=0; i<text.Length; ++i)
            {
                var c = text[i];
                if(current.ContainsChildren(c))
                {
                    current = current.Children[c];
                } else
                {
                    return Array.Empty<string>();
                }
            }
            if(current == _root)
            {
                return Array.Empty<string>();
            }
            return GetWords(current).Select(x => text + x).ToArray();
        }

        private string[] GetWords(Node node, int maxSize = 100)
        {
            var wordList = new List<string>();
            var qt = new Queue<(StringBuilder, Node, bool final)>();
            qt.Enqueue((new StringBuilder(), node, false));
            while(qt.Count > 0 && wordList.Count < maxSize)
            {
                var (prevText, nd, final) = qt.Dequeue();
                if(final)
                {
                    wordList.Add(prevText.ToString());
                    continue;
                }
                foreach (var (k, v) in nd.Children)
                {
                    var cp = new StringBuilder(prevText.Length);
                    cp.Append(prevText);
                    if(IsThereOnlyOneBranch(nd, out string branch))
                    {
                        qt.Enqueue((cp.Append(branch), v, true));
                    } else
                    {
                        cp.Append(k);
                        qt.Enqueue((cp, v, nd.IsWord));
                    }
                }
            }
            return new HashSet<string>(wordList).ToArray();
        }

        private bool IsThereOnlyOneBranch(Node node, out string text)
        {
            text = null;
            if(node == null || node.Children.Count == 0)
            {
                return false;
            }
            var builder = new StringBuilder();
            bool oneBranch = true;
            while(node != null && node.Children.Count > 0 && oneBranch)
            {
                builder.Append(node.Value);
                oneBranch = node.Children.Count == 1;
                node = node.Children.First().Value;
            }
            if(node != null)
            {
                builder.Append(node.Value);
            }
            text = builder.ToString().Substring(1);
            return oneBranch;
        }
    }
    
    public class Node
    {
        public char Value { get; set; }
        public bool IsWord { get; set; }
        public Dictionary<char, Node> Children { get; set; }
        public Node(char value)
        {
            Value = value;
            Children = new Dictionary<char, Node>();
        }

        public bool ContainsChildren(char ch)
        {
            return Children.ContainsKey(ch);
        }
    }
}
