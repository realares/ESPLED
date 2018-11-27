﻿using System;

namespace LedItOut
{
    partial class Program
    {
        public class RgbwColor
        {
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }
            public int White { get; set; }

            public static RgbwColor RGB_to_RGBW(int Ri, int Gi, int Bi)
            {
                float tM = Math.Max(Ri, Math.Max(Gi, Bi));

                //If the maximum value is 0, immediately return pure black.
                if (tM == 0)
                { return new RgbwColor() { Red = 0, Green = 0, Blue = 0, White = 0 }; }

                //This section serves to figure out what the color with 100% hue is
                float multiplier = 255.0f / tM;
                float hR = Ri * multiplier;
                float hG = Gi * multiplier;
                float hB = Bi * multiplier;

                //This calculates the Whiteness (not strictly speaking Luminance) of the color
                float M = Math.Max(hR, Math.Max(hG, hB));
                float m = Math.Min(hR, Math.Min(hG, hB));
                float Luminance = ((M + m) / 2.0f - 127.5f) * (255.0f / 127.5f) / multiplier;

                //Calculate the output values
                int Wo = Convert.ToInt32(Luminance);
                int Bo = Convert.ToInt32(Bi - Luminance);
                int Ro = Convert.ToInt32(Ri - Luminance);
                int Go = Convert.ToInt32(Gi - Luminance);

                //Trim them so that they are all between 0 and 255
                if (Wo < 0) Wo = 0;
                if (Bo < 0) Bo = 0;
                if (Ro < 0) Ro = 0;
                if (Go < 0) Go = 0;
                if (Wo > 255) Wo = 255;
                if (Bo > 255) Bo = 255;
                if (Ro > 255) Ro = 255;
                if (Go > 255) Go = 255;
                return new RgbwColor() { Red = Ro, Green = Go, Blue = Bo, White = Wo };
            }
        }
    }
}
