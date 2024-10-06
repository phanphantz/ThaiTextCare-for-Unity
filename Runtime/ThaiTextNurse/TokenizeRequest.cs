namespace PhEngine.ThaiTextCare
{
    public class TokenizeRequest
    {
        public TokenizeRequest(string input, string separator = "", bool isBreakWords = true, bool isSupportRichText = true)
        {
            Input = input;
            Separator = separator;
            IsBreakWords = isBreakWords;
            IsSupportRichText = isSupportRichText;
        }

        public string Input { get; }
        public string Separator { get; }
        public bool IsBreakWords { get; }
        public bool IsSupportRichText { get; }
    }
}