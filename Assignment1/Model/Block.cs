using System.Collections.Generic;
using Assignment1.Util;

namespace Assignment1.Model
{
    /// <summary>
    /// Represents a block. For details see chapter 2.2.3.1 in the report
    /// </summary>
    public class Block
    {
        public const int Size = 64;

        public BitSequence TotalSequence { get; set; }
        public BitSequence LeftSequence { get; set; }
        public BitSequence RightSequence { get; set; }

        public int Count
        {
            get { return TotalSequence != null ? TotalSequence.Count : 0; }
        }

        public Block(BitSequence bitSequence)
        {
            TotalSequence = bitSequence;
        }

        public Block()
        {
        }

        public void Divide()
        {
            LeftSequence = BitUtils.GetFirstHalf(TotalSequence);
            RightSequence = BitUtils.GetSecondHalf(TotalSequence);
        }

        public void Merge()
        {
            TotalSequence = BitUtils.MergeBitSequences(new List<BitSequence>
            {
                LeftSequence,
                RightSequence
            });
        }

        public void SwapSides()
        {
            var temp = LeftSequence;

            LeftSequence = RightSequence;
            RightSequence = temp;
        }

        public override string ToString()
        {
            if (LeftSequence != null && RightSequence != null)
            {
                return "LEFT: " + LeftSequence.AsTextString + " | RIGHT: " +
                       RightSequence.AsTextString;
            }

            return "LeftSequence and RightSequence are null";
        }
    }
}