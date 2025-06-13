using System;
using ModestTree;
using Math = System.Math;

namespace Scripts.HandsLogic
{
    public static class HandBonesUtility
    {
        public static readonly string[] boneNames = new[] { "Wrist", "Index", "Little", "Middle", "Palm", "Ring",  "Thumb" };
        public static readonly string[] deepNames = new[] { "Metacarpal", "Proximal", "Intermediate", "Distal", "Tip" };
        public static readonly char[] boneCapitals = new[] { 'W', 'I', 'L', 'M', 'P', 'R', 'T' };
        public static readonly char[] deepCapitals = new[] { 'M', 'P', 'I', 'D', 'T' };

        public static string ToName(int index)
        {
            if (index == 0)
                return boneNames[0];
            if (index == 16)
                return boneNames[4];
            if (index < 16)
                return boneNames[(int)Math.Ceiling((decimal)index / 5)] + deepNames[(index - 1) % 5];
            return boneNames[(int)Math.Ceiling((decimal)(index + 1) / 5)] + deepNames[(index - 2) % 5];
        }
        
        public static char[] ToCapitals(int index)
        {
            if (index == 0)
                return new[] { boneCapitals[0] };
            if (index == 16)
                return new[] { boneCapitals[4] };
            return new[] { boneCapitals[GetFingerId(index)], deepCapitals[GetDeepId(index)] };
        }
        public static int ToIndex(string id)
        {
            int nameIndex = boneCapitals.IndexOf(id[0]);
            if (nameIndex == 0)
                return 0;
            if (nameIndex == 4)
                return 16;
            if (nameIndex < 4)
                return 5 * (nameIndex - 1) + 1 + deepCapitals.IndexOf(id[boneNames[nameIndex].Length]);
            return 5 * (nameIndex - 2) + 2 + deepCapitals.IndexOf(id[boneNames[nameIndex].Length]);
        }
        public static int GetDeeperId(int id)
        {
            if (id == 0 || id == 19 || id < 19 ? id % 5 == 0 : id % 5 == 1)
                throw new ArgumentException();
            return id + 1;
        }
        public static bool TryGetDeeperId(int id, out int result)
        {
            try
            {
                result = GetDeeperId(id);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }
        public static int GetShallowerId(int id)
        {
            if(id == 0)
                throw new ArgumentException();
            if (id == 16 || GetDeepId(id) == 0)
                return 0;
            return id - 1;
        }
        public static bool TryGetShallowerId(int id, out int result)
        {
            try
            {
                result = GetShallowerId(id);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }
        
        public static int GetFingerId(int index)
        {
            if (index < 16)
                return (int)Math.Ceiling((decimal)index / 5);
            return (int)Math.Ceiling((decimal)(index + 1) / 5);
        }
        public static int GetDeepId(int index)
        {
            if (index < 16)
                return (index - 1) % 5;
            return (index - 2) % 5;
        }
      
    }
}