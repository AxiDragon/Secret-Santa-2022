﻿using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace ParadoxNotion
{
    public static class ColorUtils
    {
        ///<summary>Convert Color to Hex.</summary>
        private static readonly Dictionary<Color32, string> colorHexCache = new();

        ///<summary>Convert Hex to Color.</summary>
        private static readonly Dictionary<string, Color> hexColorCache = new(StringComparer.OrdinalIgnoreCase);

        ///<summary>The color with alpha</summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        ///<summary>A greyscale color</summary>
        public static Color Grey(float value)
        {
            return new Color(value, value, value, 1);
        }

        public static string ColorToHex(Color32 color)
        {
#if UNITY_EDITOR
            {
                if (!EditorGUIUtility.isProSkin)
                    if (color == Color.white)
                        return "#000000";
            }
#endif

            string result;
            if (colorHexCache.TryGetValue(color, out result)) return result;
            result = ("#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2")).ToUpper();
            return colorHexCache[color] = result;
        }

        public static Color HexToColor(string hex)
        {
            Color result;
            if (hexColorCache.TryGetValue(hex, out result)) return result;
            if (hex.Length != 6) throw new Exception("Invalid length for hex color provided");
            var r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            result = new Color32(r, g, b, 255);
            return hexColorCache[hex] = result;
        }
    }
}