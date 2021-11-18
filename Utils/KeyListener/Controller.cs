using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuler.Utils.KeyListener
{
    internal class Controller : IDisposable
    {
        private GlobalKeyboardHook _globalKeyboardHook;
        private IKeybindActions _actions;

        public void SetupKeyboardHooks(IKeybindActions actions)
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
            this._actions = actions;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (_actions == null)
            {
                return;
            }
            //Debug.WriteLine(e.KeyboardData.VirtualCode);

            // seems, not needed in the life.
            //54 - "5"
            // 40 strzałka w dół
            // 38 - strzałka w góre
            Debug.WriteLine("flags = " + e.KeyboardData.Flags);
            Debug.WriteLine("VirtualCode = " + e.KeyboardData.VirtualCode);
            Debug.WriteLine("SysKeyDown = " + e.KeyboardState);
            Debug.WriteLine("");
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
                e.KeyboardData.Flags == GlobalKeyboardHook.LlkhfAltdown)
            {
                if (e.KeyboardData.VirtualCode == 53)
                {
                    _actions.ScreenshotStash();
                    e.Handled = true;
                }
            }
            else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
                     e.KeyboardData.Flags == 33)
            {
                if (e.KeyboardData.VirtualCode == 40)
                {
                    _actions.SelectionDown();
                    e.Handled = true;
                }

                if (e.KeyboardData.VirtualCode == 38)
                {
                    _actions.SelectionUp();
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}
