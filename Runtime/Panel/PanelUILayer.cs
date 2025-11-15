using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Panel
{
    /// <summary>
    /// 这个Layer层是控制面板的
    /// 面板是界面的一种,没有历史记录,没有队列,
    /// 就是简单的显示在界面中
    /// 比如说体力槽,小地图这种常驻的
    /// </summary>
    public class PanelUILayer : UILayer<IPanelController>
    {
        [SerializeField, Tooltip("面板优先级与对应父物体的映射列表")]
        PanelPriorityLayerList m_PriorityLayers = null;

        public override void ShowScreen(IPanelController screen)
        {
            screen.Show();
        }

        public override void ShowScreen<TProps>(IPanelController screen, TProps props)
        {
            screen.Show(props);
        }

        public override void HideScreen(IPanelController screen)
        {
            screen.Hide();
        }
        
        public bool IsPanelVisible(string panelID)
        {
            if (m_RegisteredScreens.TryGetValue(panelID, out IPanelController panel))
            {
                return panel.IsVisible;
            }
            return false;
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            var ctl = controller as IPanelController;
            if (ctl != null)
            {
                ReparentToParaLayer(ctl.Priority, screenTransform);
            }
            else
            {
                base.ReparentScreen(controller, screenTransform);
            }
        }

        void ReparentToParaLayer(PanelPriority priority, Transform screenTransform)
        {
            Transform trans;
            if (!m_PriorityLayers.ParaLayerLookup.TryGetValue(priority, out trans))
            {
                trans = this.transform;
            }
            screenTransform.SetParent(trans, false);
        }
    }
}