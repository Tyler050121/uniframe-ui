using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Panel
{
    public enum PanelPriority
    {
        None = 0,
        Prioritary = 1,
        Tutorial = 2,
        Blocker = 3
    }

    [System.Serializable]
    public class PanelPriorityLayerEntry
    {
        [SerializeField, Tooltip("面板的优先级")]
        PanelPriority m_Priority;
        [SerializeField, Tooltip("对应的父物体")]
        Transform m_TargetParent;

        public Transform TargetParent
        {
            get { return m_TargetParent; }
            set { m_TargetParent = value; }
        }

        public PanelPriority Priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }

        public PanelPriorityLayerEntry(PanelPriority priority, Transform targetParent)
        {
            m_Priority = priority;
            m_TargetParent = targetParent;
        }
    }

    [System.Serializable]
    public class PanelPriorityLayerList
    {
        [SerializeField, Tooltip("面板优先级与对应父物体的映射列表")]
        List<PanelPriorityLayerEntry> m_ParaLayers = null;

        Dictionary<PanelPriority, Transform> m_Lookup;
        public Dictionary<PanelPriority, Transform> ParaLayerLookup
        {
            get
            {
                if (m_Lookup == null || m_Lookup.Count == 0)
                {
                    CacheLookup();
                }
                return m_Lookup;
            }
        }

        void CacheLookup()
        {
            m_Lookup = new Dictionary<PanelPriority, Transform>();
            for (int i = 0; i < m_ParaLayers.Count; i++)
            {
                m_Lookup.Add(m_ParaLayers[i].Priority, m_ParaLayers[i].TargetParent);
            }
        }

        public PanelPriorityLayerList(List<PanelPriorityLayerEntry> entries)
        {
            m_ParaLayers = entries;
        }
    }
}