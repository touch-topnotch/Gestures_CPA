using System;
using UnityEngine;

namespace Scripts.Static
{
    public class GameException: Exception
    {
        public GameException(string message) : base(message)
        {
            Debug.LogError(message);
        }
    }
    public class CharToFloatException: GameException 
    {
        //unsupported type of char
        public CharToFloatException(char c) : base("Unsupported type of char " + c +" when converting to float")
        {
        }
    }
    public class FloatToCharException: GameException 
    {
        //unsupported type of char
        public FloatToCharException(float f) : base("Float out of range " + f + " when converting to char")
        {
        }
    }
    public class StringToVec3Exception: GameException 
    {
        //unsupported type of char
        public StringToVec3Exception(string c) : base("Unsupported type of string " + c +" when converting to Vector3")
        {
        }
    }
    public class CharToVec3Exception: GameException 
    {
        //unsupported type of char
        public CharToVec3Exception(char c) : base("Unsupported type of char " + c +" when converting to Vector3")
        {
        }
    }
}