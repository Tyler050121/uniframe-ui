using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Window
{
    /// <summary>
    /// 这是一个“辅助”层级，以便显示优先级更高的窗口。
    /// 默认情况下，它包含任何标记为弹出窗口的窗口。它由 WindowUILayer 控制
    /// </summary>
    public class WindowParaLayer : MonoBehaviour
    {
        [SerializeField] GameObject m_DarkenBgObject = null;

        List<GameObject> m_ContainedScreens = new List<GameObject>();

        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(this.transform, false);
            m_ContainedScreens.Add(screenRectTransform.gameObject);
        }

        public void RefreshDarkenBg()
        {
            for (int i = 0; i < m_ContainedScreens.Count; i++)
            {
                if (m_ContainedScreens[i] == null) continue;
                if (!m_ContainedScreens[i].activeInHierarchy) continue;
                m_DarkenBgObject.SetActive(true);
                return;
            }
            m_DarkenBgObject.SetActive(false);
        }

        public void DarkenBg()
        {
            m_DarkenBgObject.SetActive(true);
            m_DarkenBgObject.transform.SetAsLastSibling();
        }
    }
}