using System.Collections.Generic;
using System.Linq;

namespace Assets
{
    public static class MatriceManager
    {
        /// <summary>
        /// Compare two array to check if they're the same
        /// <para>
        /// Source : https://stackoverflow.com/a/12446807
        /// </para> 
        /// </summary>
        /// <returns>Return true if the sources are the same</returns>
        public static bool CheckIfArraysAreEquals(int[,] source1, int[,] source2) {
            bool isEqual =
               source1.Rank == source2.Rank &&
               Enumerable.Range(0, source1.Rank).All(dimension => source1.GetLength(dimension) == source2.GetLength(dimension)) &&
               source1.Cast<int>().SequenceEqual(source2.Cast<int>());

            return isEqual;
        }

        public static int[,] RotateMatrice(int[,] src, bool rotateClockWise = true, int nbRotation = 1) {
            int nbCol = src.GetUpperBound(0) + 1; // + 1 to get the size (and not the index) of the dimension
            int nbRow = src.GetUpperBound(1) + 1;

            // Inverse the col and row if the array don't have the same size
            // This avoid an out of bound exception
            int[,] dst = (nbCol == nbRow) ? new int[nbCol, nbRow]
                                          : new int[nbRow, nbCol];

            for (int col = 0; col < nbCol; col++) {
                for (int row = 0; row < nbRow; row++) {
                    int dstCol, dstRow;

                    if (rotateClockWise) {
                        dstCol = row;
                        dstRow = nbCol - (col + 1);
                    } else {
                        dstCol = nbRow - (row + 1);
                        dstRow = col;
                    }

                    dst[dstCol, dstRow] = src[col, row];
                }
            }

            return (nbRotation > 1) ? RotateMatrice(dst, rotateClockWise, nbRotation - 1) : dst;
        }

        public static int[,] ReverseMatrice(int[,] src) {
            int nbCol = src.GetUpperBound(0) + 1; // + 1 to get the size (and not the index) of the dimension
            int nbRow = src.GetUpperBound(1) + 1;
            int[,] dst = new int[nbCol, nbRow];

            for (int col = 0; col < nbCol; col++) {
                for (int row = 0; row < nbRow; row++) {
                    int dstRow, dstCol;

                    dstRow = (nbRow - 1) - row;
                    dstCol = col;

                    dst[dstCol, dstRow] = src[col, row];
                }
            }

            return dst;
        }

        public static List<int[,]> GeneratesAllMatriceVariants(int[,] originalMap) {
            const int NB_ROTATED_VARIANTS = 3; // We don't count the original as a variant
            List<int[,]> mapsVariants = new List<int[,]> {
                originalMap,
            };
            int[,] reversedOriginalMap = ReverseMatrice(originalMap);

            if (!CheckIfArraysAreEquals(originalMap, reversedOriginalMap)) {
                mapsVariants.Add(reversedOriginalMap);
            }

            // Start at 1 because we have to rotate the piece at least one time
            for (int i = 1; i <= NB_ROTATED_VARIANTS; i++) {
                int[,] mapVariant = RotateMatrice(originalMap, true, i);
                int[,] reversedMapVariant = ReverseMatrice(mapVariant);
                bool duplicateVariant = false;
                bool duplicateReversedVariant = false;

                foreach (int[,] map in mapsVariants) {
                    if (CheckIfArraysAreEquals(map, mapVariant)) {
                        duplicateVariant = true;
                        break;
                    }
                }

                if (!duplicateVariant)
                    mapsVariants.Add(mapVariant);

                foreach (int[,] map in mapsVariants) {
                    if (CheckIfArraysAreEquals(map, reversedMapVariant)) {
                        duplicateReversedVariant = true;
                        break;
                    }
                }

                if (!duplicateReversedVariant)
                    mapsVariants.Add(reversedMapVariant);

            }

            return mapsVariants;
        }
    }
}
