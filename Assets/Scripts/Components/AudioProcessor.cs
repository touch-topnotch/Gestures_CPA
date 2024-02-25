using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Gesture_Editor_SDK.EditorAttributes;
using Scripts.Tests;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Components
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioProcessor : SerializedMonoBehaviour
    {
        [BoxGroup("Audio Clips Library")][SerializeField]
        private Dictionary<string, AudioClip> _audioClips = new();

        [BoxGroup("Audio Clips Library")][SerializeField]
        private Dictionary<string, List<AudioClip>> _audioClipLists = new();

        [SerializeField] private AudioSource _audioSource;
        
        private void OnValidate()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.loop = false;
            }

            if (string.IsNullOrEmpty(_folderPath))
            {
                _folderPath = Directory.Exists("Assets/Sounds" + this.name) ? "Assets/Sounds" + this.name : "Assets/Sounds";
            }
            if (string.IsNullOrEmpty(_telegramSoundsFolderPath))
            {
                _telegramSoundsFolderPath = Directory.Exists("Assets/Sounds/TelegramSounds") ? "Assets/Sounds/TelegramSounds" : "Assets/Sounds";
            }

            _showSorting = Directory.Exists(_telegramSoundsFolderPath) &&
                           Directory.GetFiles(_telegramSoundsFolderPath).Length > 0;

        }

        public void PlayRandomSound(string listName)
        {
            if (_audioClipLists.ContainsKey(listName))
                _audioSource.PlayOneShot(_audioClipLists[listName][Random.Range(0, _audioClipLists[listName].Count)]);
            else
                Debug.LogWarning("List " + listName + " not found in AudioProcessor");
        }

        public void PlaySequencedSound(string listName, int id)
        {
            if (!_audioClipLists.ContainsKey(listName))
            {
                Debug.LogWarning("List " + listName + " not found in AudioProcessor");
                return;
            }

            if (_audioClipLists[listName].Count <= id)
            {
                Debug.LogWarning("Index out of range");
                return;
            }
            
            _audioSource.PlayOneShot(_audioClipLists[listName][id]);
        }
        public void PlaySound(string soundName)
        {
            if (_audioClips.ContainsKey(soundName))
                _audioSource.PlayOneShot(_audioClips[soundName]);
            else
                Debug.LogWarning("Sound " + soundName + " not found in AudioProcessor");
        }

        #region Authomatization
        #if UNITY_EDITOR
        [BoxGroup("Add missing clips")]
        [FolderPath] [SerializeField] private string _folderPath;

        [BoxGroup("Add missing clips")]
        [Button("Add missing audio clips")]
        private void AddMissingAudioClips()
        {
            // go by folder path and
            // add all audio clips in folder to tempClips, if it contains S_ prefix
            // if clip contains S_R_ prefix, add it to _audioClipLists with name S_name_of_clip_without_prefix
            // else add it to _audioClips with name S_name_of_clip_without_prefix
            if(Directory.Exists(_folderPath))
            {
                var files = Directory.GetFiles(_folderPath);
                foreach (var file in files)
                {
                    if (file.Contains(".meta")) continue;
                    var clip = (AudioClip)AssetDatabase.LoadAssetAtPath(file, typeof(AudioClip));
                    
                    if (clip.name.Contains("S_"))
                    {
                        if (clip.name.Contains("S_R_"))
                        {
                            var name = clip.name.Split("_")[2];
                            if (_audioClipLists.ContainsKey(name))
                            {
                                _audioClipLists[name].Add(clip);
                            }
                            else
                            {
                                _audioClipLists.Add(name, new List<AudioClip> { clip });
                            }
                        }
                        else
                        {
                            if (clip.name.Contains("Frame"))
                            {
                                var name = "Frame";
                                if (int.TryParse(clip.name.Split("_")[3], out int index))
                                {
                                    if (!_audioClipLists.ContainsKey(name))
                                    {
                                        _audioClipLists.Add(name,  new List<AudioClip>());
                                    }

                                    if (_audioClipLists[name].Count > index)
                                    {
                                        _audioClipLists[name][index] = clip;
                                    }
                                    else
                                    {
                                        for(int i = _audioClipLists[name].Count; i < index; i++)
                                        {
                                            _audioClipLists[name].Add(clip);
                                        }
                                        
                                        //_audioClipLists[name]
                                        _audioClipLists[name].Add(clip);
                                    }
                                }
                                else
                                {
                                    Debug.Log("Index of frame is not exist! File: " + clip.name);
                                }
                            }
                            else
                            {
                                var name = clip.name.Split("_")[2];
                                if(_audioClips.ContainsKey(name))
                                    _audioClips[name] = clip;
                                else
                                    _audioClips.Add(name, clip);
                                
                            }
                        }
                    }
                }
            }
        }

        private bool _showSorting = true;
        [BoxGroup("Sorting")] [SerializeField]
        [FolderPath]
        private string _telegramSoundsFolderPath;
        
        [BoxGroup("Sorting")]
        [Button("Sort Telegram audio clips to folders")]
      //  [ShowIf("_showSorting")]
        private void SortAudioClips()
        {   
            Debug.Log("Move audio clips to folders ...");
            // search all files with S_ in folder , find name = fileName.Split("_")[1] and move them to folder Assets/Sounds/name
            if(Directory.Exists(_telegramSoundsFolderPath))
            {
                var files = Directory.GetFiles(_telegramSoundsFolderPath);
                foreach (var file in files)
                {
                    if (file.Contains(".meta"))
                        AssetDatabase.DeleteAsset(file);

                    var clip = (AudioClip)AssetDatabase.LoadAssetAtPath(file, typeof(AudioClip));
                    if (clip.name.Contains("S_"))
                    {
                        var name = clip.name.Split("_")[1];
                        var folder = "Assets/Sounds/" + name;
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        AssetDatabase.MoveAsset(file, folder + "/" + clip.name + ".wav");
                    }
                }
            }
            Debug.Log("Move audio clips to folders finished");
        }
        #endif
        #endregion
    }
}