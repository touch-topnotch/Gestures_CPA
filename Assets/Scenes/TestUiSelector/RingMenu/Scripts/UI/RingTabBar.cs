using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RingTabBar : MonoBehaviour
{
    [SerializeField] private TMP_Text TabsCountText;
    [SerializeField] private TMP_Text currentTabText;
    [SerializeField] private TMP_Text nextTabText;

    public void SetTabsCountText(string s)
    {
        TabsCountText.text = s;
    }
    public void SetCurrentTabText(string s)
    {
        currentTabText.text = s;
    }
    public void SetNextTabText(string s)
    {
        nextTabText.text = s;
    }
}
