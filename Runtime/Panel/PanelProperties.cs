using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Panel
{
    [System.Serializable]
    public class PanelProperties : IPanelProperties
    {
        [SerializeField] PanelPriority m_Priority = PanelPriority.None;

        public PanelPriority Priority
        {
            get => m_Priority;
            set => m_Priority = value;
        }
    }
}