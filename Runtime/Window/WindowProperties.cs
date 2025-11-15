using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    [System.Serializable]
    public class WindowProperties : IWindowProperties
    {
        [SerializeField] protected bool m_hideOnForegroundLost = true;
        [SerializeField] protected WindowPriority m_windowQueuePriority = WindowPriority.ForceForeground;
        [SerializeField] protected bool m_isPopup = false;

        public WindowProperties()
        {
            m_hideOnForegroundLost = true;
            m_windowQueuePriority = WindowPriority.ForceForeground;
            m_isPopup = false;
        }

        public WindowPriority WindowQueuePriority
        {
            get => m_windowQueuePriority;
            set => m_windowQueuePriority = value;
        }

        public bool HideOnForegroundLost
        {
            get => m_hideOnForegroundLost;
            set => m_hideOnForegroundLost = value;
        }

        public bool SuppressPrefabProperties { get; set; }

        public bool IsPopup
        {
            get => m_isPopup;
            set => m_isPopup = value;
        }

        public WindowProperties(bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = WindowPriority.ForceForeground;
            HideOnForegroundLost = true;
            SuppressPrefabProperties = suppressPrefabProperties;
        }

        public WindowProperties(WindowPriority priority, bool hideOnForegroundLost = false, bool suppressPrefabProperties = false)
        {
            WindowQueuePriority = priority;
            HideOnForegroundLost = hideOnForegroundLost;
            SuppressPrefabProperties = suppressPrefabProperties;
        }
    }
}