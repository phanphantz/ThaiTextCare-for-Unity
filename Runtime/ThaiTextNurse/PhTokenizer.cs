using System.Collections.Generic;

namespace PhEngine.ThaiTextCare
{
    public class PhTokenizer
    {
        class TrieNode
        {
            public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
            public bool IsEndOfWord;
        }

        TrieNode m_Root;

        public PhTokenizer(IEnumerable<string> dictionary)
        {
            m_Root = new TrieNode();
            foreach (var word in dictionary)
            {
                AddWord(word);
            }
        }
        
        void AddWord(string word)
        {
            var currentNode = m_Root;
            foreach (var letter in word)
            {
                if (!currentNode.Children.ContainsKey(letter))
                {
                    currentNode.Children[letter] = new TrieNode();
                }
                currentNode = currentNode.Children[letter];
            }
            currentNode.IsEndOfWord = true;
        }

        /// <summary>
        ///  Tokenize the input text by finding the longest match in the dictionary
        /// </summary>
        /// <param name="input">original text</param>
        /// <param name="isSupportRichTextTags"></param>
        /// <returns></returns>
        public List<string> Tokenize(string input, bool isSupportRichTextTags)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < input.Length)
            {
                string longestMatch = null;
                bool wasOpenBracket = false;
                bool wasThaiCharacter = false;
                TrieNode currentNode = m_Root;
                int matchLength = 0;
                var length = input.Length;
                for (int j = i; j < length; j++)
                {
                    char c = input[j];
                    if (IsOpenBracket(c))
                    {
                        wasOpenBracket = true;
                        continue;
                    }

                    var isThaiEndCharacter = IsThaiEndCharacter(c);
                    if (IsCloseBracket(c) || isThaiEndCharacter)
                    {
                        //Check if the end character is followed by a close bracket
                        if (isThaiEndCharacter && j < length - 1 && IsCloseBracket(input[j + 1]))
                            j++;
                        
                        longestMatch = input.Substring(i, j - i + 1);
                        break;
                    }
                    
                    if (IsShouldNotTokenize(c))
                    {
                        //If last character was Thai, end it first
                        if (wasThaiCharacter)
                        {
                            longestMatch = input.Substring(i, j - i);
                            break;
                        }
                        j++;
                        while (j < length)
                        {
                            c = input[j];
                            if (IsShouldNotTokenize(c))
                            {
                                //Keep going with non-tokenized characters
                                j++;
                            }
                            else
                            {
                                //We've found something recognizable again
                                break;
                            }
                        }
                        longestMatch = input.Substring(i, j - i);
                        break;
                    }
                    
                    //Try check for Rich Text Tags
                    if (isSupportRichTextTags && c == '<')
                    {
                        var k = j;
                        j++;
                        while (j < length)
                        {
                            //Keep going until the tag is closed or invalid
                            c = input[j];
                            if (c == '>')
                            {
                                j++;
                                break;
                            }
                            if (c == '<' || j >= length)
                            {
                                //Invalid tag, revert to character after open bracket
                                j = k+1;
                                break;
                            }
                            j++;
                        }
                        longestMatch = input.Substring(i, j - i);
                        break;
                    }

                    if (currentNode.Children.TryGetValue(c, out var child))
                    {
                        wasThaiCharacter = true;
                        currentNode = child;
                        matchLength++;
                        if (currentNode.IsEndOfWord)
                        {
                            //If the next character is a follower character, we definitely want to wait for it
                            if (HasNoFollower(input, j))
                            {
                                if (wasOpenBracket)
                                {
                                    matchLength++;
                                    wasOpenBracket = false;
                                }
                                longestMatch = input.Substring(i, matchLength);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //Increment main index
                if (longestMatch != null)
                {
                    tokens.Add(longestMatch);
                    i += longestMatch.Length;
                }
                else
                {
                    // No match
                    if (HasNoFollower(input, i))
                    {
                        if (wasOpenBracket && i < length-1)
                        {
                            tokens.Add(input.Substring(i , 2)); 
                            i+=2;
                        }
                        else
                        {
                            tokens.Add(input[i].ToString()); 
                            i++;
                        }
                    }
                    else
                    {
                        //Make sure to get all follower characters 
                        var substringLength = 2;
                        var k = i + 1;
                        while (!HasNoFollower(input, k))
                        {
                            substringLength++;
                            k++;
                        }
                        if (wasOpenBracket)
                            substringLength++;
                        tokens.Add(input.Substring(i , substringLength)); 
                        i+=substringLength;
                    }
                }
            }

            return tokens;
        }

        static bool HasNoFollower(string input, int currentIndex)
        {
            return currentIndex >= input.Length - 1 || !IsFollowingThaiGlyph(input[currentIndex + 1]);
        }

        static bool IsShouldNotTokenize(char c)
        {
            // Avoid IsDigit() and IsWhiteSpace() to gain more performance
            return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) ||
                   (c >= '๐' && c <= '๙') || (c >= '0' && c <= '9')||
                   (c == '~' || c == 'ๆ' || c == 'ฯ' || c == '“' || c == '”' || c == ',' || c =='.')
                   || c == ' ' || c == '\n' || c == '\r' || c == '\t' || c == '\\'
                   || IsCloseBracket(c);
        }

        static bool IsOpenBracket(char c)
        {
            return c == '(' || c == '{' || c == '[';
        }
        
        static bool IsCloseBracket(char c)
        {
            return c == ')' || c == '}' || c == ']';
        }
        
        static bool IsFollowingThaiGlyph(char c)
        {
            return (c >= '\u0E30' && c <= '\u0E39') || //Thai Following Vowels
                   c == '\u0E4C' || // ์
                   c == '\u0E47' || // ็
                   c == '\u0E48' || // ่
                   c == '\u0E49' || // ้
                   c == '\u0E4A' || // ๊
                   c == '\u0E4B' || // ๋
                   c == 'ๆ' || c == 'ฯ' || c == 'ๅ';
        }

        static bool IsThaiEndCharacter(char c)
        {
            return c == 'ๆ' || c == 'ฯ';
        }
    }
}