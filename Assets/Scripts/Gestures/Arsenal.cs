using System.Collections.Generic;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.Static;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.Gestures
{
    /// <summary>
    /// What's the difference between Arsenal and Library?
    /// Firstly, Arsenal contains ABILITIES (Recognizable items)
    /// Gesture Library - the dictionary of all possible GESTURES (Dynamic gestures, frames ..)
    /// Secondly, Arsenal should be connected to the SERVER - it is a NetworkBehavior component (Or its implementation),
    /// because only server should set the abilities to Player at some stage of game
    /// Thirdly?? I don't know, why i want to say at least 3 reasons to argument my opinion.. OKEEEY.. hm
    /// In the Arsenal you can incredibly fast manipulate with abilities, change, shuffle it, add to recognizer by
    /// extended processor.
    ///
    /// How it works -
    /// I need - get the list of abilites from server - func SetAbilitiesClientRpc
    /// Initialize them on start
    /// Rapidly remove it from server..
    /// Make characters independent? Just separate all recognition logic from CharacterController
    /// (for this moment Recognizer totaly depends on Character List)
    ///
    /// ДУМАЙ! сейчас ты пытаешься запихнуть все части в одну коробку! Так не нужно делать!
    /// Рассортируй их по разным маленьким коробочкам и поставь маркеры у каждой из них
    /// эта коробка - Arsenal, в ней хранится список доступных способностей
    /// эта коробка - AbilitesLibrary, в ней хранятся все доступные IGestureAbility обьекты
    /// эта коробка - GesturesLibrary, в ней хранятся все доступные жесты
    ///
    /// - вот согл, в арсенале нужно также раскидать
    /// у нас разные Ability подчиняются кардинально разным правилам
    /// frame или gesture - просто указать тип!
    /// </summary>
    public class Arsenal : SmartDict<string, IGestureAbility>
    {
        public Dictionary<string, DynamicGesture> ToGestureDict()
        {
            var dg = new Dictionary<string, DynamicGesture>();
            foreach (var ability in this)
            {
                dg.Add(ability.Key, ability.Value.dynamicGesture);
            }

            return dg;
        }
    }
}