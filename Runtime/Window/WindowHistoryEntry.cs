using UIFramework.Core;

namespace UIFramework.Window
{
    public struct WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IWindowProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IWindowProperties props)
        {
            Screen = screen;
            Properties = props;
        }
    
        public void Show()
        {
            Screen.Show(Properties);
        }
    }
}