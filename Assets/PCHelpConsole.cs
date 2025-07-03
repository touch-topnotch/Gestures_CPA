using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts
{
    public class PCHelpConsole : MonoBehaviour
    {
        public Button reloadButton;

        private void Start()
        {
            var sceneId = SceneManager.GetActiveScene().name;
            reloadButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(sceneId);
            });
        }
    }
}
