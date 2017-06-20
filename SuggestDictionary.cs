using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Prediction
{
    /// <summary>
    /// This class handles the bigram pairs
    /// </summary>
    class SuggestDictionary
    {
        public TernarySearchTree st;
        public Dictionary<String, String> ht = new Dictionary<String, String>();

        /// <summary>
        /// Get next word suggestion on the basis of bigram pairs
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<String> getHashSuggestions(String str)
        {
            List<String> sugg = new List<string>();
            String[] data = null;
            st = null;
            st = new TernarySearchTree();
            Dictionary<String, int> dict = new Dictionary<string, int>();
            if (ht.ContainsKey(str))
            {
                if (ht[str] == "")
                    return sugg;
                String[] suggestions = ht[str].Split('&');
                List<String> list = new List<string>(suggestions);
                list = list.GetRange(1, list.Count - 2);

                foreach (String i in list)
                {
                    data = i.Split(';');
                    sugg.Add(data[0]);
                    if(!dict.ContainsKey(data[0]))
                    dict.Add(data[0], int.Parse(data[1]));
                    st.insert(data[0], int.Parse(data[1]));
                }
                list = new List<string>();
                var variable = from pair in dict
                               orderby pair.Value descending
                               select pair;


                // Add results to list
                foreach (KeyValuePair<string, int> pair in variable)
                {
                    list.Add(pair.Key);
                }
                return list;
            }
            return new List<String>();
        }

        /// <summary>
        /// give a reduced autocomplete list based on next words.
        /// </summary>
        /// <param name="prefix">Incomplete word</param>
        /// <returns>A list of possible suggestions</returns>
        public List<string> getAutoSuggestSuggestions(string prefix)
        {
            List<string> sugg = new List<string>();
            sugg = st.getAutoCompleteSuggestions(prefix);
            return sugg;
        }
    }
}
