using System;
using UIFramework.Panel;
using UIFramework.Window;

namespace UIFramework.Core
{
    /// <summary>
    /// 所有UI界面必须实现的接口
    /// </summary>
    public interface IScreenController
    {
        string ScreenID { get; set; }
        bool IsVisible { get; }
        void Show(IScreenProperties props = null);
        void Hide(bool animate = true);

        Action<IScreenController> InTransitionFinished { get; set; }
        Action<IScreenController> OutTransitionFinished { get; set; }
        Action<IScreenController> CloseRequest { get; set; }
        Action<IScreenController> ScreenDestroyed { get; set; }
    }

    public interface IWindowController : IScreenController
    {
        bool HideOnForegroundLost { get; }
        bool IsPopup { get; }
        WindowPriority Priority { get; }
    }

    public interface IPanelController : IScreenController
    {
        PanelPriority Priority { get; }
    }
}