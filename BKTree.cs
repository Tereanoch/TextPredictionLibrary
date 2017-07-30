using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

namespace Text_Prediction
{
    /// <summary>
    /// A tree for autocorrect
    /// </summary>
    class BKTree
    {
        private Node _Root;
        private int treesize = 0;

        /// <summary>
        /// Returns the tree size
        /// </summary>
        public int TreeSize
        {
            get { return treesize; }
        }
        Dictionary<String, int> dict = new Dictionary<String, int>();

        /// <summary>
        /// Add a word to the BK-Tree.
        /// </summary>
        /// <param name="word"></param>
        public void Add(string word)
        {
            word = word.ToLower();
            if (_Root == null)
            {
                _Root = new Node(word);
                return;
            }

            var curNode = _Root;

            var dist = VarChiDamLevDistance(curNode.Word, word);
            while (curNode.ContainsKey(dist))
            {
                if (dist == 0) return;

                curNode = curNode[dist];
                dist = VarChiDamLevDistance(curNode.Word, word);
            }
            curNode.AddChild(dist, word);
            treesize++;
        }

        /// <summary>
        /// Search an alternative word in the BK tree
        /// </summary>
        /// <param name="word"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public List<string> Search(string word, int d)
        {
            dict = new Dictionary<string, int>();
            var rtn = new List<string>();
            word = word.ToLower();

            RecursiveSearch(_Root, rtn, word, d);

            // Order by values.
            // Use LINQ to specify sorting by value.
            var suggestions = from pair in dict
                              orderby pair.Value ascending
                              select pair;

            // Add results to list
            foreach (KeyValuePair<string, int> pair in suggestions)
            {
                rtn.Add(pair.Key);
            }
            //for (int i = 0; i < rtn.Count ; i++ )
            //{
            //    rtn[i]=rtn[i]+" "+ed[i].ToString();
            //}
            //return rtn;
            return rtn;
        }

        /// <summary>
        /// Recursive function to search for closest word. Cannot to be called explicity
        /// </summary>
        /// <param name="node">A node variable of the BK-Tree</param>
        /// <param name="rtn">A list containing possible alternatives to "word"</param>
        /// <param name="word">A word whose alternative is being searched</param>
        /// <param name="d">maximum allowed edit distance</param>
        private void RecursiveSearch(Node node, List<string> rtn, string word, int d)
        {
            if (node == null)
                return;
            var curDist = VarChiDamLevDistance(node.Word, word);
            var minDist = curDist - d;
            var maxDist = curDist + d;

            if (curDist <= d)
            {
                dict.Add(node.Word, curDist);
                //rtn.Add(node.Word);
            }

            foreach (var key in node.Keys.Cast<int>().Where(key => minDist <= key && key <= maxDist))
            {
                RecursiveSearch(node[key], rtn, word, d);
            }
        }

        /// <summary>
        /// calculate Levensthein distance
        /// </summary>
        /// <param name="first">First String</param>
        /// <param name="second">Second String</param>
        /// <returns>edit distance</returns>
        public static int LevenshteinDistance(string first, string second)
        {
            if (first.Length == 0) return second.Length;
            if (second.Length == 0) return first.Length;

            var lenFirst = first.Length;
            var lenSecond = second.Length;

            var d = new int[lenFirst + 1, lenSecond + 1];

            for (var i = 0; i <= lenFirst; i++)
                d[i, 0] = i;

            for (var i = 0; i <= lenSecond; i++)
                d[0, i] = i;

            for (var i = 1; i <= lenFirst; i++)
            {
                for (var j = 1; j <= lenSecond; j++)
                {
                    var match = (first[i - 1] == second[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + match);
                }
            }

            return d[lenFirst, lenSecond];
        }

        /// <summary>
        /// calculate Damerau Levenshtein distance
        /// </summary>
        /// <param name="first">First String</param>
        /// <param name="second">Second String</param>
        /// <returns>edit distance</returns>
        public static int DamLevDistance(string first, string second)
        {
            int lenFirst = first.Length;
            int lenSecond = second.Length;

            var matrix = new int[lenFirst + 1, lenSecond + 1];

            for (int i = 0; i <= lenFirst; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= lenSecond; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= lenFirst; i++)
            {
                for (int j = 1; j <= lenSecond; j++)
                {
                    int cost = second[j - 1] == first[i - 1] ? 0 : 1;
                    var vals = new int[] 
                    {
                        matrix[i - 1, j] + 1,
                        matrix[i, j - 1] + 1,
                        matrix[i - 1, j - 1] + cost
			        };
                    matrix[i, j] = vals.Min();
                    if (i > 1 && j > 1 && first[i - 1] == second[j - 2] && first[i - 2] == second[j - 1])
                    {


                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                    }
                }
            }
            return matrix[lenFirst, lenSecond];
        }

        /// <summary>
        /// edit distance for Hindi
        /// </summary>
        /// <param name="first">First String</param>
        /// <param name="second">Second String</param>
        /// <returns>edit distance</returns>
        public static int VarChiDamLevDistance(string first, string second)
        {
            if (first.Length == 0)
                return second.Length;
            if (second.Length == 0)
                return first.Length;
            int lenFirst = first.Length;
            int lenSecond = second.Length;

            var matrix = new int[lenFirst + 1, lenSecond + 1];

            for (int i = 0; i <= lenFirst; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= lenSecond; j++)
                matrix[0, j] = j;

            int cm = 0;
            for (int i = 1; i <= lenFirst; i++)
            {
                for (int j = 1; j <= lenSecond; j++)
                {
                    int cost = second[j - 1] == first[i - 1] ? 0 : 1;

                    char c1 = first[i - 1];
                    char c2 = second[j - 1];
                    cm = 0;
                    if (cost != 0)
                    {
                        bool b1 = (c1 == 'ू' || c1 == 'ी' || c1 == 'ु' || c1 == 'ू' || c1 == 'े' || c1 == 'ै' || c1 == 'ो' || c1 == 'ौ');
                        bool b2 = (c2 == 'ि' || c2 == 'ी' || c2 == 'ु' || c2 == 'ू' || c2 == 'े' || c2 == 'ै' || c2 == 'ो' || c2 == 'ौ');
                        if (b1 && !b2)
                            cm += 0;
                        else if (b2 && !b1)
                            cm += 0;
                        else if (!b1 && !b2)
                            cm += 1;
                    }
                    var vals = new int[] 
                    {
                        matrix[i - 1, j] + 1  ,
                        matrix[i, j - 1] + 1  ,
                        matrix[i - 1, j - 1] + cost
			        };
                    matrix[i, j] = vals.Min();
                    if (i > 1 && j > 1 && first[i - 1] == second[j - 2] && first[i - 2] == second[j - 1])
                    {
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
                    }
                }
            }
            return matrix[lenFirst, lenSecond];
        }

    }

    /// <summary>
    /// Node structure of the BK tree
    /// </summary>
    class Node
    {
        public string Word { get; set; }
        public HybridDictionary Children { get; set; }

        public Node() { }

        public Node(string word)
        {
            this.Word = word.ToLower();
        }

        public Node this[int key]
        {
            get { return (Node)Children[key]; }
        }

        public ICollection Keys
        {
            get
            {
                if (Children == null)
                    return new ArrayList();
                return Children.Keys;
            }
        }

        public bool ContainsKey(int key)
        {
            return Children != null && Children.Contains(key);
        }

        public void AddChild(int key, string word)
        {
            if (this.Children == null)
                Children = new HybridDictionary();
            this.Children[key] = new Node(word);
        }
    }
}
