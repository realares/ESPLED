using System;
using System.Drawing;
using System.Windows.Forms;

namespace LedItOut
{
    public static class SpotSet
    {

        public static Spot[] Spots { get; set; }
        public static readonly object Lock = new object();

        /// <summary>
        /// returns the number of leds
        /// </summary>
        public static int CountLeds(int spotsX, int spotsY)
        {
            if (spotsX <= 1 || spotsY <= 1)
            {
                //special case because it is not really a rectangle of lights but a single light or a line of lights
                return spotsX * spotsY;
            }

            //normal case
            return 2 * spotsX + 2 * spotsY - 4;
        }

        public static Rectangle ExpectedScreenBound { get; private set; }
        public static void Refresh()
        {
            lock (Lock)
            {
                var usersetting = UserSettings.Instance;
                Spots = new Spot[CountLeds(usersetting.SpotsX, usersetting.SpotsY)];

                var rectangle = ExpectedScreenBound = Screen.PrimaryScreen.Bounds;

                var canvasSizeX = (rectangle.Width - 2 * usersetting.BorderDistanceX);
                var screenHeight = rectangle.Height;
                var canvasSizeY = (screenHeight - 2 * usersetting.BorderDistanceY);

                var xResolution = usersetting.SpotsX > 1 ? (canvasSizeX - usersetting.SpotWidth) / (usersetting.SpotsX - 1) : 0;
                var xRemainingOffset = usersetting.SpotsX > 1 ? ((canvasSizeX - usersetting.SpotWidth) % (usersetting.SpotsX - 1)) / 2 : 0;
                var yResolution = usersetting.SpotsY > 1 ? (canvasSizeY - usersetting.SpotHeight) / (usersetting.SpotsY - 1) : 0;
                var yRemainingOffset = usersetting.SpotsY > 1 ? ((canvasSizeY - usersetting.SpotHeight) % (usersetting.SpotsY - 1)) / 2 : 0;

                var counter = 0;
                var relationIndex = usersetting.SpotsX - usersetting.SpotsY + 1;

                for (var j = 0; j < usersetting.SpotsY; j++)
                {
                    for (var i = 0; i < usersetting.SpotsX; i++)
                    {
                        var isFirstColumn = i == 0;
                        var isLastColumn = i == usersetting.SpotsX - 1;
                        var isFirstRow = j == 0;
                        var isLastRow = j == usersetting.SpotsY - 1;

                        if (isFirstColumn || isLastColumn || isFirstRow || isLastRow) // needing only outer spots
                        {
                            var x = Math.Max(0, Math.Min(rectangle.Width, xRemainingOffset + usersetting.BorderDistanceX + usersetting.OffsetX + i * (xResolution) + usersetting.SpotWidth / 2));
                            var y = Math.Max(0, Math.Min(screenHeight, yRemainingOffset + usersetting.BorderDistanceY + usersetting.OffsetY + j * (yResolution) + usersetting.SpotHeight / 2));

                            var index = counter++; // in first row index is always counter

                            if (usersetting.SpotsX > 1 && usersetting.SpotsY > 1)
                            {
                                if (!isFirstRow && !isLastRow)
                                {
                                    if (isFirstColumn)
                                    {
                                        index += relationIndex + ((usersetting.SpotsY - 1 - j) * 3);
                                    }
                                    else if (isLastColumn)
                                    {
                                        index -= j;
                                    }
                                }

                                if (!isFirstRow && isLastRow)
                                {
                                    index += relationIndex - (i * 2);
                                }
                            }
                            var spotWidth = Math.Min(usersetting.SpotWidth, Math.Min(x, rectangle.Width - x));
                            var spotHeight = Math.Min(usersetting.SpotHeight, Math.Min(y, screenHeight - y));
                            SpotSet.Spots[index] = new Spot(i, j, x, y, spotWidth, spotHeight);
                        }
                    }
                }


                if (usersetting.OffsetLed != 0) Offset(usersetting.OffsetLed);
                if (usersetting.SpotsY > 1 && usersetting.MirrorX) MirrorX();
                if (usersetting.SpotsX > 1 && usersetting.MirrorY) MirrorY();
            }
        }

        private static void Mirror(int startIndex, int length)
        {
            var halfLength = (length / 2);
            var endIndex = startIndex + length - 1;

            for (var i = 0; i < halfLength; i++)
            {
                Swap(startIndex + i, endIndex - i);
            }
        }

        private static void Swap(int index1, int index2)
        {
            var temp = Spots[index1];
            Spots[index1] = Spots[index2];
            Spots[index2] = temp;
        }

        private static void MirrorX()
        {
            var usersetting = UserSettings.Instance;
            // copy swap last row to first row inverse
            for (var i = 0; i < usersetting.SpotsX; i++)
            {
                var index1 = i;
                var index2 = (Spots.Length - 1) - (usersetting.SpotsY - 2) - i;
                Swap(index1, index2);
            }

            // mirror first column
            Mirror(usersetting.SpotsX, usersetting.SpotsY - 2);

            // mirror last column
            if (usersetting.SpotsX > 1)
                Mirror(2 * usersetting.SpotsX + usersetting.SpotsY - 2, usersetting.SpotsY - 2);
        }

        private static void MirrorY()
        {
            var usersetting = UserSettings.Instance;
            // copy swap last row to first row inverse
            for (var i = 0; i < usersetting.SpotsY - 2; i++)
            {
                var index1 = usersetting.SpotsX + i;
                var index2 = (Spots.Length - 1) - i;
                Swap(index1, index2);
            }

            // mirror first row
            Mirror(0, usersetting.SpotsX);

            // mirror last row
            if (usersetting.SpotsY > 1)
                Mirror(usersetting.SpotsX + usersetting.SpotsY - 2, usersetting.SpotsX);
        }

        private static void Offset(int offset)
        {
            var temp = new Spot[Spots.Length];
            for (var i = 0; i < Spots.Length; i++)
            {
                temp[(i + temp.Length + offset) % temp.Length] = Spots[i];
            }
            Spots = temp;
        }

        public static void IndicateMissingValues()
        {
            foreach (var spot in Spots)
            {
                spot.IndicateMissingValue();
            }
        }
    }

}
