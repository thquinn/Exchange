using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Util {
    public static float Hypot(float a, float b) {
        return Mathf.Sqrt(a * a + b * b);
    }

    public static string StripLineType(string line, out LineType lineType) {
        int spaceIndex = line.IndexOf(' ');
        if (spaceIndex == -1) {
            spaceIndex = line.Length + 1;
        }
        string typeString = line.Substring(0, spaceIndex - 1);
        switch (typeString) {
            case "clear":
                lineType = LineType.Clear;
                break;
            case "get":
                lineType = LineType.Get;
                break;
            case "go":
                lineType = LineType.Go;
                break;
            case "lose":
                lineType = LineType.Lose;
                break;
            case "reveal":
                lineType = LineType.Reveal;
                break;
            case "sacrifice":
                lineType = LineType.Sacrifice;
                break;
            case "score":
                lineType = LineType.Score;
                break;
            case "tbc":
                lineType = LineType.TBC;
                break;
            case "text":
                lineType = LineType.Text;
                break;
            case "time":
                lineType = LineType.Time;
                break;
            default:
                throw new Exception("Failed to parse line: " + line);
        }
        return spaceIndex < line.Length - 1 ? line.Substring(spaceIndex + 1) : "";
    }

    // written by Mosè Bottacini: https://stackoverflow.com/questions/7040289/converting-integers-to-roman-numerals
    public static string ToRoman(int number) {
        if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        throw new ArgumentOutOfRangeException("something bad happened");
    }
}
