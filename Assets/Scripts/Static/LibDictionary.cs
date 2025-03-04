using System;
using System.Collections.Generic;
using Scripts.Gestures;

namespace Scripts.Static
{
    public class Restrictive<K, T>
    {
        public List<T> GetList() => _limitedList;
        private readonly List<T> _limitedList = new();

        public Dictionary<K, T> GetOpenDict() => _dict;
        public Dictionary<K, T> GetLimitedDict() => _limitedDictionary;
        private readonly Dictionary<K, T> _limitedDictionary = new();
        
        private Dictionary<K, T> _dict = new();
        private readonly List<K> _activeKeys = new List<K>();
        private readonly List<K> _notExistedKeys = new List<K>();

        public void AddDictionary(Dictionary<K, T> dictionary)
        {
            if (dictionary == null)
                return;
            _dict = dictionary;
            ChangeActiveKeys(_activeKeys);
        }
        public void AddItem(K key, T value)
        {
            _dict.Add(key, value);
            for (int i = 0; i < _activeKeys.Count; i++)
            {
                if (Equals(_activeKeys[i], key))
                {
                    AddItemToLists(key);
                }
            }
            for (int i = 0; i < _notExistedKeys.Count; i++)
            {
                if (Equals(_notExistedKeys[i], key))
                {
                    _notExistedKeys.Remove(key);
                    _activeKeys.Add(key);
                    AddItemToLists(key);
                }
            }
        }
        
        public void ChangeActiveKeys(List<K> keys)
        {
            _activeKeys.Clear();
            _notExistedKeys.Clear();
            
            _limitedList.Clear();
            _limitedDictionary.Clear();
            
            for (int i = 0; i < keys.Count; i++)
            {
                AddActiveKey(keys[i]);
            }
        }

     
        public void AddActiveKey(K key)
        {
            if (_limitedDictionary.ContainsKey(key))
                return;
   
            if (_dict.ContainsKey(key))
            {
                _activeKeys.Add(key);
                AddItemToLists(key);
            }
            else
            {
                bool hasInList = false;
                for (int i = 0; i < _notExistedKeys.Count; i++)
                {
                    if (Equals(_notExistedKeys[i],key))
                    {
                        hasInList = true;
                    }
                }
                if(!hasInList)
                    _notExistedKeys.Add(key);
            }
        }

        private void AddItemToLists(K key)
        {
            _limitedDictionary.Add(key, _dict[key]);
            _limitedList.Add(_dict[key]);
        }
        
        public void RemoveActiveKey(K key)
        {
            for (int i = 0; i < _limitedList.Count; i++)
            {
                if(Equals(_limitedList, key))
                {
                    _activeKeys.Remove(key);
                    RemoveItemFromList(key);
                    return;
                }
            }

            for (int i = 0; i < _notExistedKeys.Count; i++)
            {
                if (Equals(_notExistedKeys[i], key))
                {
                    _notExistedKeys.Remove(key);
                    return;
                }
            }
        }
        private void RemoveItemFromList(K key)
        {
            _limitedDictionary.Remove(key);
            _limitedList.Remove(_dict[key]);
        }
    }

    public class test
    {
        void testt()
        {
            Restrictive<string, int> restrictive =
                new Restrictive<string, int>();
            restrictive.AddItem("R", 1);
            restrictive.AddItem("K", 2);
            restrictive.AddActiveKey("K");
            restrictive.RemoveActiveKey("K");
            restrictive.ChangeActiveKeys(new List<string>(){"1", "2", "3"});
        }
    }
}