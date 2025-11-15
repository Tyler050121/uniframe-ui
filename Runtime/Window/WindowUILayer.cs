using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework.Core;
using UnityEngine;

namespace UIFramework.Window
{
    public class WindowUILayer : UILayer<IWindowController>
    {
        [SerializeField] WindowParaLayer m_PriorityParaLayer = null;
        public IWindowController CurrentWindow { get; private set; }

        Queue<WindowHistoryEntry> m_WindowQueue;
        Stack<WindowHistoryEntry> m_WindowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;

        HashSet<IScreenController> m_ScreensTransitioning;

        bool IsScreenTransitionInProgress
        {
            get { return m_ScreensTransitioning.Count > 0; }
        }

        public override void Initialize()
        {
            base.Initialize();
            m_WindowQueue = new Queue<WindowHistoryEntry>();
            m_WindowHistory = new Stack<WindowHistoryEntry>();
            m_ScreensTransitioning = new HashSet<IScreenController>();
        }

        protected override void ProcessScreenRegister(string screenID, IWindowController controller)
        {
            base.ProcessScreenRegister(screenID, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestByWindow;
        }

        protected override void ProcessScreenUnregister(string screenID, IWindowController controller)
        {
            base.ProcessScreenUnregister(screenID, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestByWindow;
        }

        public override void ShowScreen(IWindowController screen)
        {
            ShowScreen<IWindowProperties>(screen, null);
        }

        public override void ShowScreen<TProps>(IWindowController screen, TProps props)
        {
            IWindowProperties windowProp = props as IWindowProperties;

            if (ShouldEnqueue(screen, windowProp))
            {
                EnqueueWindow(screen, windowProp);
            }
            else
            {
                DoShow(screen, windowProp);
            }
        }

        public override void HideScreen(IWindowController screen)
        {
            if (screen == CurrentWindow)
            {
                m_WindowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (m_WindowQueue.Count > 0)
                {
                    ShowNextInQueue();
                }
                else if (m_WindowHistory.Count > 0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogWarning($"[WindowUILayer] Attempting to hide a window that is not the current window: {screen.ScreenID}");
            }
        }

        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            m_PriorityParaLayer.RefreshDarkenBg();
            m_WindowQueue.Clear();
            m_WindowHistory.Clear();
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;
            if (window == null)
            {
                Debug.LogWarning("[WindowUILayer] Screen " + screenTransform.name + "is not a window controller.");
            }
            else
            {
                if (window.IsPopup)
                {
                    m_PriorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }
            base.ReparentScreen(controller, screenTransform);
        }

        void EnqueueWindow<TProp>(IWindowController screen, TProp props) where TProp : IScreenProperties
        {
            m_WindowQueue.Enqueue(new WindowHistoryEntry(screen, (IWindowProperties)props));
        }

        bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp)
        {
            if (CurrentWindow == null && m_WindowQueue.Count == 0)
            {
                return false;
            }

            if (windowProp != null && windowProp.SuppressPrefabProperties)
            {
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeground;
            }

            if (controller.Priority != WindowPriority.ForceForeground)
            {
                return true;
            }

            return false;
        }

        void ShowPreviousInHistory()
        {
            if (m_WindowHistory.Count > 0)
            {
                WindowHistoryEntry window = m_WindowHistory.Pop();
                DoShow(window);
            }
        }

        void ShowNextInQueue()
        {
            if (m_WindowQueue.Count > 0)
            {
                WindowHistoryEntry window = m_WindowQueue.Dequeue();
                DoShow(window);
            }
        }

        void DoShow(IWindowController screen, IWindowProperties props)
        {
            DoShow(new WindowHistoryEntry(screen, props));
        }

        void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
                Debug.LogWarning($"[WindowUILayer] Attempting to show the same window that is already current: {windowEntry.Screen.ScreenID}");
            }
            else if (CurrentWindow != null && CurrentWindow.HideOnForegroundLost && !windowEntry.Screen.IsPopup)
            {
                CurrentWindow.Hide();
            }

            m_WindowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            if (windowEntry.Screen.IsPopup)
            {
                m_PriorityParaLayer.DarkenBg();
            }

            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;
        }

        void OnInAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
        }

        void OnOutAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
            var window = screen as IWindowController;
            if (window.IsPopup)
            {
                m_PriorityParaLayer.RefreshDarkenBg();
            }
        }

        void OnCloseRequestByWindow(IScreenController screen)
        {
            HideScreen(screen as IWindowController);
        }

        void AddTransition(IScreenController screen)
        {
            m_ScreensTransitioning.Add(screen);
            RequestScreenBlock?.Invoke();
        }

        void RemoveTransition(IScreenController screen)
        {
            m_ScreensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                RequestScreenUnblock?.Invoke();
            }
        }
    }
}