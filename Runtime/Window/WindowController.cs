using UIFramework.Core;

namespace UIFramework.Window
{
    public abstract class WindowController : AWindowController<WindowProperties> { }
    public abstract class AWindowController<TProps> : UIScreenController<TProps>, IWindowController
        where TProps : IWindowProperties
    {
        public bool HideOnForegroundLost => Properties.HideOnForegroundLost;

        public bool IsPopup => Properties.IsPopup;

        public WindowPriority Priority => Properties.WindowQueuePriority;

        public virtual void UI_Close()
        {
            CloseRequest?.Invoke(this);
        }

        protected sealed override void SetProperties(TProps props)
        {
            if (props == null) return;
            if (!props.SuppressPrefabProperties)
            {
                props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                props.WindowQueuePriority = Properties.WindowQueuePriority;
                props.IsPopup = Properties.IsPopup;
            }

            Properties = props;
        }

        protected override void HierachyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}