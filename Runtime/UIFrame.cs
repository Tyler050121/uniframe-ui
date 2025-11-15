using System;
using UIFramework.Core;
using UIFramework.Panel;
using UIFramework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UIFrame : MonoBehaviour
    {
        [SerializeField] bool m_InitializeOnAwake = true;
        PanelUILayer m_PanelLayer;
        WindowUILayer m_WindowLayer;

        Canvas m_Canvas;
        GraphicRaycaster m_GraphicRaycaster;

        public Canvas MainCanvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = this.GetComponent<Canvas>();
                }
                return m_Canvas;
            }
        }

        public Camera UICamera
        {
            get
            {
                return MainCanvas.worldCamera;
            }
        }

        void Awake()
        {
            if (m_InitializeOnAwake)
            {
                Initialize();
            }
        }

        public virtual void Initialize()
        {
            m_PanelLayer = this.GetComponentInChildren<PanelUILayer>(true);
            if (m_PanelLayer == null)
            {
                Debug.LogError("[UIFrame] PanelUILayer component is missing in children.");
            }
            else
            {
                m_PanelLayer.Initialize();
            }

            m_WindowLayer = this.GetComponentInChildren<WindowUILayer>(true);
            if (m_WindowLayer == null)
            {
                Debug.LogError("[UIFrame] WindowUILayer component is missing in children.");
            }
            else
            {
                m_WindowLayer.Initialize();
                m_WindowLayer.RequestScreenBlock += OnRequestScreenBlock;
                m_WindowLayer.RequestScreenUnblock += OnRequestScreenUnblock;
            }

            m_GraphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }

        public void ShowPanel(string screenID)
        {
            m_PanelLayer.ShowScreenByID(screenID);
        }

        public void ShowPanel<T>(string screenID, T props) where T : IPanelProperties
        {
            m_PanelLayer.ShowScreenByID<T>(screenID, props);
        }

        public void HidePanel(string screenID)
        {
            m_PanelLayer.HideScreenByID(screenID);
        }

        public void OpenWindow(string screenID)
        {
            m_WindowLayer.ShowScreenByID(screenID);
        }

        public void OpenWindow<T>(string screenID, T props) where T : IWindowProperties
        {
            m_WindowLayer.ShowScreenByID<T>(screenID, props);
        }

        public void CloseWindow(string screenID)
        {
            m_WindowLayer.HideScreenByID(screenID);
        }

        public void CloseCurrentWindow()
        {
            if (m_WindowLayer.CurrentWindow != null)
            {
                CloseWindow(m_WindowLayer.CurrentWindow.ScreenID);
            }
        }

        public void ShowScreen(string screenID)
        {
            if (IsScreenRegistered(screenID, out Type type))
            {
                if (type == typeof(IPanelController))
                {
                    ShowPanel(screenID);
                }
                else if (type == typeof(IWindowController))
                {
                    OpenWindow(screenID);
                }
            }
            else
            {
                Debug.LogWarning($"[UIFrame] No screen found with ID {screenID} to show.");
            }
        }

        public void RegisterScreen(string screenID, IScreenController controller, Transform screenTransform)
        {
            if (controller is IPanelController panelController)
            {
                m_PanelLayer.RegisterScreen(screenID, panelController);
                if (screenTransform != null)
                {
                    m_PanelLayer.ReparentScreen(panelController, screenTransform);
                }
            }
            else if (controller is IWindowController windowController)
            {
                m_WindowLayer.RegisterScreen(screenID, windowController);
                if (screenTransform != null)
                {
                    m_WindowLayer.ReparentScreen(windowController, screenTransform);
                }
            }
            else
            {
                Debug.LogError($"[UIFrame] Attempting to register a screen with unsupported controller type: {controller.GetType().Name}");
            }
        }

        public void RegisterPanel<TPanel>(string screenID, IPanelController controller) where TPanel : IPanelController
        {
            m_PanelLayer.RegisterScreen(screenID, controller);
        }

        public void UnRegisterPanel<TPanel>(string screenID, IPanelController controller) where TPanel : IPanelController
        {
            m_PanelLayer.UnregisterScreen(screenID, controller);
        }

        public void RegisterWindow<TWindow>(string screenID, IWindowController controller) where TWindow : IWindowController
        {
            m_WindowLayer.RegisterScreen(screenID, controller);
        }

        public void UnRegisterWindow<TWindow>(string screenID, IWindowController controller) where TWindow : IWindowController
        {
            m_WindowLayer.UnregisterScreen(screenID, controller);
        }

        public bool IsPanelOpen(string panelID)
        {
            return m_PanelLayer.IsPanelVisible(panelID);
        }

        public void HideAll(bool animate = true)
        {
            m_PanelLayer.HideAll(animate);
            m_WindowLayer.HideAll(animate);
        }

        public void CloseAllPanels(bool animate = true)
        {
            m_PanelLayer.HideAll(animate);
        }

        public void CloseAllWindows(bool animate = true)
        {
            m_WindowLayer.HideAll(animate);
        }

        public bool IsScreenRegistered(string screenID)
        {
            return m_PanelLayer.IsScreenRegistered(screenID) || m_WindowLayer.IsScreenRegistered(screenID);
        }

        public bool IsScreenRegistered(string screenID, out Type controllerType)
        {
            if (m_PanelLayer.IsScreenRegistered(screenID))
            {
                controllerType = typeof(IPanelController);
                return true;
            }
            else if (m_WindowLayer.IsScreenRegistered(screenID))
            {
                controllerType = typeof(IWindowController);
                return true;
            }
            controllerType = null;
            return false;
        }

        void OnRequestScreenBlock()
        {
            if (m_GraphicRaycaster != null)
            {
                m_GraphicRaycaster.enabled = false;
            }
        }

        void OnRequestScreenUnblock()
        {
            if (m_GraphicRaycaster != null)
            {
                m_GraphicRaycaster.enabled = true;
            }
        }
    }
}