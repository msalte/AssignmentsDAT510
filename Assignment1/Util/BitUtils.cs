using System;
using System.Collections.Generic;
using System.Linq;
using Assignment1.Model;

namespace Assignment1.Util
{
    /// <summary>
    /// Contains several convenience methods for manipulating bits, often times BitSequence objects.
    /// This class is mentioned in chapter 2.2.3.1 in the report
    /// </summary>
    public static class BitUtils
    {
        public const int ByteSize = 8;

        public static BitSequence MergeBitSequences(List<BitSequence> bitSequences)
        {
            var mergedSequence = new BitSequence(bitSequences.Sum(i => i.Count));

            var index = 0;

            foreach (var array in bitSequences)
            {
                for (var j = 0; j < array.Count; j++)
                {
                    mergedSequence.Set(index, array.Get(j));
                    index++;
                }
            }

            return mergedSequence;
        }

        /// <summary>
        /// Will split the given BitSequence into a list of BitSequences, each with the given size
        /// </summary>
        /// <param name="bitSequence">The original BitSequence</param>
        /// <param name="size">The size of each resulting BitSequence part</param>
        /// <returns>A List of BitArrays</returns>
        public static List<BitSequence> SplitBitSequence(BitSequence bitSequence, int size)
        {
            var sequenceList = new List<BitSequence>();

            var numParts = bitSequence.Count/size; // for instance 48/6 = 8

            for (var i = 0; i < numParts; i++)
            {
                var sequence = new BitSequence(size);

                for (var j = 0; j < size; j++)
                {
                    sequence.Set(j, bitSequence.Get(j));
                }

                sequenceList.Add(sequence);

                bitSequence = RemoveBits(bitSequence, size);
            }

            return sequenceList;
        }

        public static BitSequence IntegerToBitSequence(int value, int padding)
        {
            var s = Convert.ToString(value, 2).PadLeft(padding, '0');

            var result = new BitSequence(s.Length);

            for (var i = 0; i < result.Count; i++)
            {
                result.Set(i, s[i] == '1'); // true if 1, false otherwise
            }

            return result;
        }

        private static BitSequence RemoveBits(BitSequence bitSequence, int size)
        {
            var result = new BitSequence(bitSequence.Count - size);

            for (var i = 0; i < result.Count; i++)
            {
                result.Set(i, bitSequence.Get(i + size));
            }

            return result;
        }

        private static BitSequence ShiftBits(BitSequence bitSequence, int numShifts, BitSequence bitsToAppend)
        {
            var result = new BitSequence(bitSequence.Count);

            // remove front bits
            for (var i = 0; i < result.Count - numShifts; i++)
            {
                result.Set(i, bitSequence.Get(i + numShifts));
            }

            // append end bits
            var startIndex = result.Count - numShifts;
            var endIndex = result.Count - 1;

            var j = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                result.Set(i, bitsToAppend.Get(j));
                j++;
            }

            return result;
        }

        public static BitSequence GetFirstHalf(BitSequence bitSequence)
        {
            var result = new BitSequence(bitSequence.Count/2);

            const int startIndex = 0;
            var endIndex = bitSequence.Count/2 - 1;

            for (var i = startIndex; i <= endIndex; i++)
            {
                result.Set(i, bitSequence.Get(i));
            }

            return result;
        }

        public static BitSequence ShiftBitsLeft(BitSequence bitSequence, int numShifts)
        {
            var frontBits = new BitSequence(numShifts);

            for (var i = 0; i < frontBits.Count; i++)
            {
                frontBits.Set(i, bitSequence.Get(i));
            }

            return ShiftBits(bitSequence, numShifts, frontBits);
        }

        public static BitSequence GetSecondHalf(BitSequence bitSequence)
        {
            var result = new BitSequence(bitSequence.Count/2);

            var startIndex = bitSequence.Count/2;
            var endIndex = bitSequence.Count - 1;

            var j = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                result.Set(j, bitSequence.Get(i));
                j++;
            }

            return result;
        }

        public static BitSequence RemoveFrontMostBlock(BitSequence frontMostBlock)
        {
            var newSize = frontMostBlock.Count - Block.Size;

            var noBlocksLeftover = newSize < 0;

            if (noBlocksLeftover)
            {
                // This means all blocks have been processed
                return new BitSequence(0);
            }

            var result = new BitSequence(newSize);

            var j = 0;

            for (var i = (frontMostBlock.Count - newSize); i < frontMostBlock.Count; i++)
            {
                result.Set(j, frontMostBlock.Get(i));
                j++;
            }

            return result;
        }

        public static BitSequence GetFirstBlock(BitSequence bitSequence)
        {

            var blockBits = new BitSequence(Block.Size);

            var sequenceLength = bitSequence.Count;

            var isPaddingRequired = sequenceLength < Block.Size;

            var j = 0;

            if (isPaddingRequired)
            {
                var startIndex = Block.Size - sequenceLength;

                for (var i = 0; i < Block.Size; i++)
                {
                    if (i < startIndex)
                    {
                        blockBits.Set(i, false);
                    }
                    else
                    {
                        blockBits.Set(i, bitSequence.Get(j));
                        j++;
                    }
                }
            }
            else
            {
                for (var i = 0; i < Block.Size; i++)
                {
                    blockBits.Set(i, bitSequence.Get(i));
                }
            }

            return blockBits;
        }
    }
}