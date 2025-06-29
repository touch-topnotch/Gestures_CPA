

using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.Static;

namespace Scripts.Libraries
{
   
    // Abilities Library - extended arsenal with all possible abilities in game
    public class AbilitiesLibrary
    {
        public SmartDict<string, Arsenal> characterAbilities = new();// babushka { varenik, salfetka, worklusnicheskver}, bravery {spear, shield}
        public Arsenal systemAbilities = new Arsenal();
    }
}