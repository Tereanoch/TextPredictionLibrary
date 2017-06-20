using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Prediction
{
    /// <summary>
    /// This class contains methods to handle the Ternary Search Tree Data Structure, to store the data dictionary efficiently. 
    /// </summary>
    class TernarySearchTree
    {
        /// <summary>
        /// A root node for the tree.
        /// </summary>
        public TSTNode root = null;
        Dictionary<String, int> dict = new Dictionary<String, int>();
        String pre;

        /// <summary>
        /// A method to initialize the root node.Call this method before inserting the dictionary into the tree.
        /// </summary>
        /// <param name="c">Inserting a character which is in the .middle of the language character set is recommended.</param>
        public void createRoot(char c)
        {
            root = new TSTNode(c);
        }

        /// <summary>
        /// Recursive search function. Not to be called explicitly
        /// </summary>
        /// <param name="node">TST node</param>
        /// <param name="word">A simple string</param>
        /// <param name="pos">pointer position in the word</param>
        /// <param name="frequency">frequency of the word</param>
        /// <returns></returns>
        private TSTNode insert(TSTNode node, string word, int pos, int frequency)
        {

            if (node == null)
            {
                node = new TSTNode(word[pos]);
            }

            if (word[pos] < node.Ch)
            {
                node.left = insert(node.left, word, pos, frequency);
            }
            else if (word[pos] > node.Ch)
            {
                node.right = insert(node.right, word, pos, frequency);
            }
            else
            {
                if (pos + 1 < word.Length)
                {
                    node.middle = insert(node.middle, word, pos + 1, frequency);
                }
                else
                {
                    node.wordEnd = true;
                    node.frequency = frequency;
                }
            }
            return node;
        }

        /// <summary>
        /// insert words into the tree
        /// </summary>
        /// <param name="word">A normal word</param>
        /// <param name="frequency">The pre-decided frequency count in the dictionary</param>
        public void insert(string word, int frequency)
        {
            root = insert(root, word, 0, frequency);
        }

        private TSTNode traverse(TSTNode node, string prefix, int pos)
        {
            if (node != null)
            {
                if (prefix[pos] < node.Ch)
                {
                    return traverse(node.left, prefix, pos);
                }
                else if (prefix[pos] > node.Ch)
                {

                    return traverse(node.right, prefix, pos);
                }
                else
                {
                    if (pos + 1 == prefix.Length)
                    {
                        return node;
                    }
                    else
                    {
                        return traverse(node.middle, prefix, pos + 1);

                    }
                }
            }
            return node;
        }

        public TSTNode traverse(string prefix)
        {
            TSTNode n = traverse(root, prefix, 0);
            return n;
        }

        /// <summary>
        /// recursive search funciton. Not to be called explicitly
        /// </summary>
        /// <param name="node">TST node</param>
        /// <param name="str">A simple String</param>
        private void search(TSTNode node, String str)
        {
            if (node == null)
                return;
            else
            {
                search(node.left, str);
                str = str + node.Ch;
                if (node.wordEnd)
                {
                    dict.Add(pre + str, node.frequency);
                }

                search(node.middle, str);
                str = str.Substring(0, str.Length - 1);
                search(node.right, str);
            }
        }

        /// <summary>
        /// A function to search the possible words based on the prefix entry.
        /// </summary>
        /// <param name="prefix">An incomplete or complete word</param>
        /// <returns>A list of words</returns>
        private List<String> search(String prefix)
        {
            pre = prefix;
            List<String> list = new List<string>();
            TSTNode start = traverse(prefix);
            if (start != null)
            {
                if (start.wordEnd == true)
                {
                    dict.Add(prefix, start.frequency);
                }

                search(start.middle, "");

                // Order by values.
                // Use LINQ to specify sorting by value.
                var suggestions = from pair in dict
                                  orderby pair.Value descending
                                  select pair;

                
                // Add results to list
                foreach (KeyValuePair<string, int> pair in suggestions)
                {
                    list.Add(pair.Key);
                }

                return list;
            }
            return list;
        }

        /// <summary>
        /// Returns the specified no. of suggestions
        /// </summary>
        /// <param name="prefix">Initial matching prefix String</param>
        /// <returns>A list with suggestions ranked highest to lowest based on frequency</returns>
        public List<String> getAutoCompleteSuggestions(String prefix)
        {
            dict.Clear();
            List<String> list = new List<String>();
            list = search(prefix);            
            return list;
        }
    }

    /// <summary>
    /// Ternary Search Tree node
    /// </summary>
    class TSTNode
    {
        private char ch;
        public TSTNode left, right, middle;
        public int frequency;
        public bool wordEnd;

        public TSTNode(char c)
        {
            ch = c;
            left = null;
            middle = null;
            right = null;
            wordEnd = false;
        }

        public char Ch
        {
            get { return ch; }
            set { ch = Ch; }
        }
    }

}

