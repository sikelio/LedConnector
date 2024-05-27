namespace LedConnector.Services
{
    class ByteLetters
    {
        private readonly Dictionary<string, string> byteLetters = [];

        public ByteLetters()
        {
            byteLetters.Add("A", "0000000000000000011001001011110100101001000000000000000");
            byteLetters.Add("B", "0000000000000000111001001011100100101110000000000000000");
            byteLetters.Add("C", "0000000000000000011101000010000100000111000000000000000");
            byteLetters.Add("D", "0000000000000000111001001010010100101110000000000000000");
            byteLetters.Add("E", "0000000000000000111101000011100100001111000000000000000");
            byteLetters.Add("F", "0000000000000000111101000011100100001000000000000000000");
            byteLetters.Add("G", "0000000000000000011101000010110100100111000000000000000");
            byteLetters.Add("H", "0000000000000000100101001011110100101001000000000000000");
            byteLetters.Add("I", "0000000000000000100001000010000100001000000000000000000");
            byteLetters.Add("J", "0000000000000000000100001000010100100110000000000000000");
            byteLetters.Add("K", "0000000000000000100101010011000101001001000000000000000");
            byteLetters.Add("L", "0000000000000000100001000010000100001110000000000000000");
            byteLetters.Add("M", "0000000000000001000111011101011000110001000000000000000");
            byteLetters.Add("N", "0000000000000000100101101010110100101001000000000000000");
            byteLetters.Add("O", "0000000000000000011001001010010100100110000000000000000");
            byteLetters.Add("P", "0000000000000000111001001011100100001000000000000000000");
            byteLetters.Add("Q", "0000000000000000011001001010010101100111000000000000000");
            byteLetters.Add("R", "0000000000000000111001001011100101001001000000000000000");
            byteLetters.Add("S", "0000000000000000011101000001100000101110000000000000000");
            byteLetters.Add("T", "0000000000000000111000100001000010000100000000000000000");
            byteLetters.Add("U", "0000000000000000100101001010010100101111000000000000000");
            byteLetters.Add("V", "0000000000000000101001010010100101000100000000000000000");
            byteLetters.Add("W", "0000000000000001000110001100011010101010000000000000000");
            byteLetters.Add("X", "0000000000000000101001010001000101001010000000000000000");
            byteLetters.Add("Y", "0000000000000000101001010011100010000100000000000000000");
            byteLetters.Add("Z", "0000000000000000111100001000100010001111000000000000000");
            byteLetters.Add(" ", "0000000000000000000000000000000000000000000000000000000");
        }

        public string GetByteLetter(string letter)
        {
            if (letter.Length > 1)
            {
                return byteLetters["A"];
            }

            string? byteLetter = byteLetters[letter.ToUpper()];

            if (byteLetter == null)
            {
                return byteLetters["A"];
            }

            return byteLetter;
        }

        public string GetRawLetter(string byteLetter)
        {
            KeyValuePair<string, string> letter = byteLetters
                .Where(l => l.Value == byteLetter)
                .FirstOrDefault();

            return letter.Key;
        }

        public string TranslateToBytes(string message)
        {
            if (message.Length == 0)
            {
                return string.Empty;
            }

            int count = 1;
            int start = 0;
            string binaryMessage = "";

            for (int i = 1; i < 11; i++)
            {
                foreach (char letter in message)
                {
                    string byteLetter = GetByteLetter(letter.ToString());
                    binaryMessage += byteLetter.Substring(start, 5);
                }

                while (binaryMessage.Length < 44 * count)
                {
                    binaryMessage += "0";
                }

                count++;
                start += 5;
            }

            return binaryMessage;
        }
    }
}
