using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Assignment3.Utils;

namespace Assignment3.Model
{
    public class RSHA
    {
        private Bits _firstIterationBits;
        private const int BlockSize = 2048;
        public const int ByteSize = 8;
        private const string Gibberish = "13tr1dc3g123vn21v3u21yf31vu2y3f12yu3fg12yu3f1yu32giu21h31iu2398";

        public RSHA()
        {
            InitializeFirstIterationBitArray();
        }

        private void InitializeFirstIterationBitArray()
        {
            var jibberish = Gibberish;

            while (jibberish.Length*ByteSize < BlockSize)
            {
                jibberish += Gibberish;
            }

            jibberish = jibberish.Substring(0, BlockSize/ByteSize);

            _firstIterationBits = RSHAUtils.StringToBits(jibberish);
        }

        public Bits Hash(Bits message)
        {
            // 1. Convert message bits to a number
            // 2. Convert number to text
            // 3. Convert text to bits

            // ?. This will cause "a" to be signigicantly different than "aa"
            message = RSHAUtils.StringToBits(message.AsBigInteger.ToString());

            // 4. Concatenate message text and message number text and substitute letters alternating between i+1 and i+2
            //    Do this two times, with the output of the first iteration as the input of the second iteratoin.
            message = Substitute(message);
            message = Substitute(message);

            // 5. While the message is less than block size, repeat its current state

            // ?. This functions as a padding of the message if it is too short and will ensure the fixed output length required by hash functions
            while (message.Count < BlockSize)
            {
                message = RSHAUtils.Merge(new List<Bits>
                    {
                        message,
                        message
                    });
            }

            // 6. Permute the (repeated) message by XOR'ing each single bit with its last bit
            var permutedMessage = PermuteMessage(message);

            // 7. Split the permuted message into blocks
            var blocks = RSHAUtils.Split(RSHAUtils.Merge(permutedMessage), BlockSize);

            Bits hashValue = null;

            // 8. For each block
            foreach (var block in blocks)
            {
                var partSize = block.Count/8;

                var parts = new List<Bits>(RSHAUtils.Split(block, partSize));

                for (var i = 0; i < parts.Count; i++)
                {
                    var number = BigInteger.Pow(parts[i].AsBigInteger, 3);
                    var xorWith = RSHAUtils.StringToBits(number.ToString());

                    if (xorWith.Count > partSize)
                    {
                        xorWith.ShrinkLeft(xorWith.Count - partSize);
                    }
                    else if (xorWith.Count < partSize)
                    {
                        xorWith.PadLeft(i%2 == 0, partSize - xorWith.Count);
                    }

                    parts[i] = parts[i].Xor(xorWith);
                }


                for (var i = 0; i < parts.Count; i += 3)
                {
                    var bitShifts = i%2 == 0 ? 13 : 7;

                    parts[i] = RSHAUtils.ShiftBits(parts[i], bitShifts);
                    parts[i] = parts[i].Xor(parts[i + 1]);
                }

                for (var i = 0; i < parts.Count; i += 2)
                {
                    parts[i] = parts[i].Xor(parts[i + 1]);
                }

                var result = RSHAUtils.Merge(parts);

                if (hashValue == null)
                {
                    hashValue = result.Xor(_firstIterationBits);
                    continue;
                }

                hashValue = result.Xor(hashValue);
            }

            return hashValue;
        }

        private static Bits Substitute(Bits messageBits)
        {
            var plainText = messageBits.AsPlainText;
            var number = messageBits.AsBigInteger;

            var substituted = SubstituteMessage(plainText + number);

            messageBits = RSHAUtils.StringToBits(substituted);
            return messageBits;
        }

        /// <summary>
        /// This method will substitute each message letter, alternating between letter+1 and letter+2
        /// Substitution refers the replacement of certain components with other components, following certain rules.
        /// </summary>
        /// <param name="message">The message to substitute</param>
        /// <returns>The resulting message</returns>
        private static string SubstituteMessage(string message)
        {
            var chars = message.ToCharArray();

            var msg = string.Empty;
            var it = 0;
            foreach (var c in chars)
            {
                var pos = it%2 == 0 ? 1 : 2;

                msg += (char) (c + pos);

                it++;
            }

            message = msg;
            return message;
        }

        /// <summary>
        /// This method will permute the message bits
        /// Permutation refers to manipulation of the order of bits according to some algorithm
        /// </summary>
        /// <param name="messageBits">The message bits to permute</param>
        /// <returns>The resulting bits after the permutation</returns>
        private static List<Bits> PermuteMessage(Bits messageBits)
        {
            var splitMessage = new List<Bits>(RSHAUtils.Split(messageBits, 1));

            var last = splitMessage.LastOrDefault();

            foreach (var bit in splitMessage)
            {
                bit.Xor(last);
            }

            return splitMessage;
        }
    }
}