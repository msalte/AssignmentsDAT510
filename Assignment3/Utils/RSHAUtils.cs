using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Assignment3.Model;

namespace Assignment3.Utils
{
    public static class RSHAUtils
    {
        public static Bits Merge(List<Bits> parts)
        {
            var mergedBits = new Bits(parts.Sum(i => i.Count));

            var index = 0;

            foreach (var list in parts)
            {
                for (var j = 0; j < list.Count; j++)
                {
                    mergedBits.Set(index, list.Get(j));
                    index++;
                }
            }

            return mergedBits;
        }

        /// <summary>
        /// Splits the parameter given Bits into several smaller chunks
        /// </summary>
        /// <param name="toSplit">The Bits to split</param>
        /// <param name="size">The size for each chunk</param>
        /// <returns></returns>
        public static IEnumerable<Bits> Split(Bits toSplit, int size)
        {
            var bitsToSplit = new Bits(toSplit);

            var blocks = new List<Bits>();

            if (bitsToSplit.Count < size)
            {
                var singleBlock = new Bits(bitsToSplit);

                singleBlock.PadLeft(false, size - bitsToSplit.Count);

                blocks.Add(singleBlock);

                return blocks;
            }

            var numBlocks = (bitsToSplit.Count / size);

            for (var i = 0; i < numBlocks; i++)
            {
                var block = new Bits(size);

                for (var j = 0; j < block.Count; j++)
                {
                    if (bitsToSplit.Count == 0) break;

                    block.Set(j, bitsToSplit.Get(j));
                }

                var diff = size - block.Count;

                if (diff != 0)
                {
                    block.PadLeft(false, diff);
                }

                blocks.Add(block);
                bitsToSplit.ShrinkLeft(size);
            }

            return blocks;
        }

        public static Bits ShiftBits(Bits bits, int numShifts)
        {
            var result = new Bits(bits.Count);

            // remove front bits
            for (var i = 0; i < result.Count - numShifts; i++)
            {
                result.Set(i, bits.Get(i + numShifts));
            }

            // append end bits
            var startIndex = result.Count - numShifts;
            var endIndex = result.Count - 1;

            var j = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                result.Set(i, bits.Get(j));
                j++;
            }

            return result;
        }

        public static int CountAvalancheEffect(Bits bits1, Bits bits2)
        {
            var result = 0;

            for (var i = 0; i < bits1.Count; i++)
            {
                if (bits1.Get(i) != bits2.Get(i)) result++;
            }

            return result;
        }

        /// <summary>
        /// Converts a parameter given string into its binary representation,
        /// represented by a Bits object
        /// </summary>
        /// <param name="charSeq">The string to convert</param>
        /// <returns>The resulting Bits object</returns>
        public static Bits StringToBits(string charSeq)
        {
            var sb = new StringBuilder();

            foreach (var b in
                charSeq
                    .Select(character => Encoding.UTF8.GetBytes(character.ToString(CultureInfo.InvariantCulture)))
                    .SelectMany(bytes => bytes))
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(RSHA.ByteSize, '0'));
            }

            var binaryString = sb.ToString();

            var bits = new Bits(binaryString.Length);

            for (var i = 0; i < binaryString.Length; i++)
            {
                bits.Set(i, binaryString[i] == '1');
            }

            return bits;
        }
    }
}