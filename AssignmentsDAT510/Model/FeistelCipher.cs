using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssignmentsDAT510.Interface;
using AssignmentsDAT510.Util;

namespace AssignmentsDAT510.Model
{
    public class FeistelCipher
    {
        private readonly IProgressCallback _progressCallback;

        private const int NumRounds = 16;
        public const int KeySize = 64;

        public FeistelCipher(IProgressCallback callback)
        {
            _progressCallback = callback;
        }

        public void Execute(string input, string key, bool isEncrypt)
        {
            var inputBits = IdentifyInputBits(input, isEncrypt);
            var roundKeys = GenerateRoundKeys(key);
            
            if (inputBits == null) return;

            var numBlocks = (int) Math.Ceiling((double) inputBits.Count/Block.Size);
            var result = string.Empty;

            _progressCallback.OnBegin(NumRounds*numBlocks);

            for (var i = 0; i < numBlocks; i++)
            {
                var block = new Block(BitUtils.GetFirstBlock(inputBits));

                inputBits = BitUtils.RemoveFrontMostBlock(inputBits);

                block = PermuteBlock(block, DES.InitialPermutation);

                try
                {
                    if (isEncrypt)
                    {
                        for (var j = 0; j < NumRounds; j++)
                        {
                            block = ExecuteRound(block, roundKeys[j]);
                        }
                    }
                    else
                    {
                        for (var j = NumRounds - 1; j >= 0; j--)
                        {
                            block = ExecuteRound(block, roundKeys[j]);
                        }
                    }
                }
                catch (Exception)
                {
                    _progressCallback.OnError(string.Format("An error occurred during {0}!",
                        isEncrypt ? "encryption" : "decryption"));

                    return;
                }

                block.SwapSides();
                block.Merge();

                block = PermuteBlock(block, DES.InverseInitialPermutation);

                result = AppendResult(result, block, isEncrypt);

                _progressCallback.OnUpdate(1);
            }

            _progressCallback.OnComplete(result);
        }

        private BitSequence IdentifyInputBits(string input, bool isEncrypt)
        {
            var isInputBinary = !input.Any(bit => bit != '1' && bit != '0');

            if (!isEncrypt)
            {
                if (isInputBinary)
                {
                    return BitUtils.BinaryStringToBitSequence(input);
                }

                _progressCallback.OnError("You are trying to decrypt non binary data!");

                return null;
            }

            if (isInputBinary)
            {
                var bitSequence = new BitSequence(input.Length);

                for (var i = 0; i < bitSequence.Count; i++)
                {
                    bitSequence.Set(i, input[i] == '1');
                }

                return bitSequence;
            }

            return new BitSequence(Encoding.UTF8.GetBytes(input));
        }

        private Block ExecuteRound(Block block, BitSequence roundKeyBits)
        {
            var nextLeft = block.RightSequence;

            var roundFunctionResult = ApplyRoundFunction(block.RightSequence, roundKeyBits);

            var nextRight = block.LeftSequence.XOR(roundFunctionResult);

            var newBlock = new Block
            {
                LeftSequence = nextLeft,
                RightSequence = nextRight
            };

            _progressCallback.OnUpdate(1);

            return newBlock;
        }

        private static Block PermuteBlock(Block oldBlock, int[] indexTable)
        {
            var block = new Block
            {
                TotalSequence = new BitSequence(oldBlock.Count)
            };

            for (var i = 0; i < block.Count; i++)
            {
                var index = indexTable[i];

                block.TotalSequence.Set(i, oldBlock.TotalSequence.Get(index - 1)); // BitSequence is zero based
            }

            block.Divide();

            return block;
        }

        private static string AppendResult(string result, Block block, bool encrypt)
        {
            return result + (encrypt ? block.TotalSequence.AsBinaryString : block.TotalSequence.AsTextString);
        }

        private static List<BitSequence> GenerateRoundKeys(string key)
        {
            var roundKeys = new List<BitSequence>();

            var permutedKey = PC1Key(key);

            var left = BitUtils.GetFirstHalf(permutedKey);
            var right = BitUtils.GetSecondHalf(permutedKey);

            for (var i = 0; i < NumRounds; i++)
            {
                // shift bits to the left by DES.KeyShifts[i] places
                left = BitUtils.ShiftBitsLeft(left, DES.KeyShifts[i]);
                right = BitUtils.ShiftBitsLeft(right, DES.KeyShifts[i]);

                // merge left and right sides into one
                var shiftedKey = BitUtils.MergeBitSequences(new List<BitSequence> {left, right});

                var roundKey = PC2Key(shiftedKey);

                roundKeys.Add(roundKey);
            }

            return roundKeys;
        }

        private static BitSequence PC2Key(BitSequence shiftedKey)
        {
            var permutedKey = new BitSequence(DES.PermutedChoices2.Length);

            for (var i = 0; i < DES.PermutedChoices2.Length; i++)
            {
                var position = DES.PermutedChoices2[i];

                permutedKey.Set(i, shiftedKey.Get(position - 1));
            }

            return permutedKey;
        }

        /// <summary>
        /// Permutes the original input key from 64 to 56 bits. 
        /// The remaining 8 bits are discarded.
        /// </summary>
        /// <param name="key">The original key</param>
        /// <returns>The permuted key, represented by a BitArray object</returns>
        private static BitSequence PC1Key(string key)
        {
            var originalKey = new BitSequence(Encoding.UTF8.GetBytes(key));

            var permutedKey = new BitSequence(56);

            for (var i = 0; i < DES.PermutedChoices1.Length; i++)
            {
                var position = DES.PermutedChoices1[i];

                permutedKey.Set(i, originalKey.Get(position - 1));
            }

            return permutedKey;
        }

        private static BitSequence ApplyRoundFunction(BitSequence bits, BitSequence roundKey)
        {
            bits = Expand(bits); // 32 bit => 48 bit
            bits = bits.XOR(roundKey); // 48 bit XOR 48 bit (key) => 48 bit result
            bits = Substitute(bits); // 48 bit => 32 bit
            bits = Permute(bits); // 32 bit => 32 bit

            return bits;
        }

        private static BitSequence Substitute(BitSequence bits)
        {
            var bitSequenceList = new List<BitSequence>();

            var bitSequenceParts = BitUtils.SplitBitSequence(bits, 6);

            // splitBitSequence is a list of 6 bit BitSequences; substitute each 6 bits to 4 bits
            for (var i = 0; i < bitSequenceParts.Count; i++)
            {
                var part = bitSequenceParts[i]; // a single BitSequence of 6 bits, for instance {1,0,1,0,0,1}

                var outerBits = new[] {part.Get(0), part.Get(5)}; // for instance {1,1}
                var innerBits = new[] {part.Get(1), part.Get(2), part.Get(3), part.Get(4)}; // for instance {0,1,0,0}

                var row = new BitSequence(outerBits).AsInteger; // for instance 3
                var col = new BitSequence(innerBits).AsInteger; // for instance 4

                // takes the integer number from the specific position in the S-boxes [i,row,col]
                // and converts to a binary value of length 4
                var newBitSequence = BitUtils.IntegerToBitSequence(DES.SubstitutionBoxes[i, row, col], 4);

                bitSequenceList.Add(newBitSequence);
            }

            return BitUtils.MergeBitSequences(bitSequenceList);
        }

        private static BitSequence Permute(BitSequence bits)
        {
            var permuted = new BitSequence(DES.Permutation.Length);

            for (var i = 0; i < permuted.Count; i++)
            {
                var position = DES.Permutation[i];

                permuted.Set(i, bits.Get(position - 1));
            }

            return permuted;
        }

        private static BitSequence Expand(BitSequence bits)
        {
            var expanded = new BitSequence(DES.Expansion.Length);

            for (var i = 0; i < expanded.Count; i++)
            {
                var position = DES.Expansion[i];

                expanded.Set(i, bits.Get(position - 1)); // BitSequence is zero based
            }

            return expanded;
        }
    }
}