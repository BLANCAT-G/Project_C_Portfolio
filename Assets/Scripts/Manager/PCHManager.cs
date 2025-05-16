using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
public enum ColorType
{
    Red=0,
    Orange,
    Yellow,
    LightGreen,
    Green,
    Turquoise,
    Blue,
    Indigo,
    Purple,
    Violet,
    Gray,
    None,
    PlayerNone,
    Black,
    White
}
public static class PCHManager 
{
    public static Color ToColor(this ColorType type)
    {
        switch (type)
        {
            case ColorType.Red:
                return new Color(240f / 255f, 0f / 255f, 0f / 255f);
            case ColorType.Orange:
                return new Color(255f / 255f, 150f / 255f, 0f / 255f); 
            case ColorType.Yellow:
                return new Color(255f / 255f, 212f / 255f, 0f / 255f); 
            case ColorType.LightGreen:
                return new Color(119f / 255f, 195f / 255f, 0f / 255f); 
            case ColorType.Green:
                return new Color(0f / 255f, 128f / 255f, 0f / 255f); 
            case ColorType.Turquoise:
                return new Color(0f / 255f, 128f / 255f, 128f / 255f); 
            case ColorType.Blue:
                return new Color(0f / 255f, 82f / 255f, 193f / 255f); 
            case ColorType.Indigo:
                return new Color(22 / 255f, 60f / 255f, 112 / 255f); 
            case ColorType.Purple:
                return new Color(115 / 255f, 0f / 255f, 179 / 255f); 
            case ColorType.Violet:
                return new Color(166 / 255f, 9f / 255f, 104f / 255f); 
            case ColorType.Gray:
                return new Color(128f / 255f, 128f / 255f, 128f / 255f); 
            case ColorType.Black:
                return Color.black;
            case ColorType.White:
                return Color.white;
            case ColorType.None:
                return Color.clear;
            case ColorType.PlayerNone:
                return new Color(221f/255f, 248f/255f, 243f/255f);
            default:
                return Color.clear;
        }
    }
    public static ColorType SubstractColor(ColorType curColor, ColorType subColor)
    {
        if (curColor == ColorType.None) return ColorType.None;
        if (subColor == ColorType.None) return curColor;
        if (curColor == ColorType.Gray && subColor!=ColorType.Gray) return (ColorType)(((int)subColor + 5) % 10);
        if (curColor != ColorType.Gray && subColor == ColorType.Gray) return curColor;

        int[] tmp = { 0, 9, 8, 9, 8, 0, 2, 1, 2, 1 };
        int cur = (int)curColor;
        int sub = (int)subColor;
        if (curColor == subColor)
            return ColorType.None;
        if (Mathf.Abs(cur-sub)==5)
            return ColorType.Gray;
        return (ColorType)((cur+tmp[(sub-cur+10) % 10])%10);
    }
    public static ColorType MixColor(ColorType c1, ColorType c2)
    {
        int value = 0;

        if (c1 == ColorType.Gray || c1==ColorType.None) return c2;
        else if (c2 == ColorType.Gray || c2==ColorType.None) return c1;

        
        int minColor=Mathf.Min((int)c1, (int)c2);
        int maxColor=Mathf.Max((int)c1, (int)c2);
        if (maxColor - minColor == 5)
            return ColorType.Gray;
        value = (maxColor - minColor > 5) ? ((maxColor + minColor) / 2 + 5)%10 : (maxColor + minColor) / 2;

        return (ColorType)value;
    }
    
    //테스트
    public static void TestMixColor(ColorType c1, ColorType c2)
    {
        ColorType[] states = (ColorType[])System.Enum.GetValues(typeof(ColorType));


        ColorType testv = PCHManager.MixColor(c1, c2);
        if ((int)testv >= 0 && (int)testv < states.Length)
        {
            ColorType state = states[(int)testv];
            Debug.Log(state.ToString());
            Debug.Log((int)testv);
        }
        Debug.Log((int)testv);
    }
    public static void TestSubColor(ColorType c1, ColorType c2)
    {
        ColorType[] states = (ColorType[])System.Enum.GetValues(typeof(ColorType));


        ColorType testv = PCHManager.SubstractColor(c1, c2);
        if ((int)testv >= 0 && (int)testv < states.Length)
        {
            ColorType state = states[(int)testv];
            Debug.Log(state.ToString());
            Debug.Log((int)testv);
        }
        Debug.Log((int)testv);
    }
}
