using System.Collections.Generic;
using UnityEngine;

namespace UIFramework.Core
{
    /// <summary>
    /// 基础的UI Layer层
    /// </summary>
    public abstract class UILayer<TScreen> : MonoBehaviour where TScreen : IScreenController
    {
        protected Dictionary<string, TScreen> m_RegisteredScreens;

        public abstract void ShowScreen(TScreen screen);
        public abstract void ShowScreen<TProps>(TScreen screen, TProps props) where TProps : IScreenProperties;
        public abstract void HideScreen(TScreen screen);

        protected virtual void Awake()
        {
            m_RegisteredScreens = new Dictionary<string, TScreen>();
        }

        public virtual void Initialize()
        {
            m_RegisteredScreens = new Dictionary<string, TScreen>();
        }

        /// <summary>
        /// 重新设置UI界面的父物体
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="screenTransform"></param>
        public virtual void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            screenTransform.SetParent(this.transform, false);
        }

        public void RegisterScreen(string screenID, TScreen controller)
        {
            if (!m_RegisteredScreens.ContainsKey(screenID))
            {
                ProcessScreenRegister(screenID, controller);
            }
            else
            {
                Debug.LogWarning($"[AUILayerController] Screen with ID {screenID} is already registered in this layer.");
            }
        }

        public void UnregisterScreen(string screenID, TScreen controller)
        {
            if (m_RegisteredScreens.ContainsKey(screenID))
            {
                ProcessScreenUnregister(screenID, controller);
            }
            else
            {
                Debug.LogWarning($"[AUILayerController] Screen with ID {screenID} is not registered in this layer.");
            }
        }

        public void ShowScreenByID(string screenID)
        {
            if (m_RegisteredScreens.TryGetValue(screenID, out TScreen screen))
            {
                ShowScreen(screen);
            }
            else
            {
                Debug.LogWarning($"[AUILayerController] No screen found with ID {screenID} to show.");
            }
        }

        public void ShowScreenByID<TProps>(string screenID, TProps props) where TProps : IScreenProperties
        {
            if (m_RegisteredScreens.TryGetValue(screenID, out TScreen screen))
            {
                ShowScreen(screen, props);
            }
            else
            {
                Debug.LogWarning($"[AUILayerController] No screen found with ID {screenID} to show.");
            }
        }

        public void HideScreenByID(string screenID)
        {
            if (m_RegisteredScreens.TryGetValue(screenID, out TScreen screen))
            {
                HideScreen(screen);
            }
            else
            {
                Debug.LogWarning($"[AUILayerController] No screen found with ID {screenID} to hide.");
            }
        }

        public bool IsScreenRegistered(string screenID)
        {
            return m_RegisteredScreens.ContainsKey(screenID);
        }

        public virtual void HideAll(bool shouldAnimateWhenHiding = true)
        {
            foreach (var screen in m_RegisteredScreens.Values)
            {
                HideScreen(screen);
            }
        }

        protected virtual void ProcessScreenRegister(string screenID, TScreen controller)
        {
            controller.ScreenID = screenID;
            m_RegisteredScreens.Add(screenID, controller);
            controller.ScreenDestroyed += OnScreenDestroyed;
        }

        protected virtual void ProcessScreenUnregister(string screenID, TScreen controller)
        {
            controller.ScreenDestroyed -= OnScreenDestroyed;
            m_RegisteredScreens.Remove(screenID);
            controller.ScreenID = null;
        }

        void OnScreenDestroyed(IScreenController controller)
        {
            if (!string.IsNullOrEmpty(controller.ScreenID)
                && m_RegisteredScreens.ContainsKey(controller.ScreenID))
            {
                UnregisterScreen(controller.ScreenID, (TScreen)controller);
            }
        }
    }
}