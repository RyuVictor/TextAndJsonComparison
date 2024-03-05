namespace TextAndJsonCompare.Models
{
    public class TextToCompare
    {
        public string Alpha { get; set; }
        public string Beta { get; set; }

        // Properties for user's choice of text input method
        public string AlphaOption { get; set; } // Will store "text" or "file" for Alpha
        public string BetaOption { get; set; } // Will store "text" or "file" for Beta

        // Properties for file uploads
        public Stream AlphaFile { get; set; } // Will store uploaded file for Alpha
        public Stream BetaFile { get; set; } // Will store uploaded file for Beta
    }
}
