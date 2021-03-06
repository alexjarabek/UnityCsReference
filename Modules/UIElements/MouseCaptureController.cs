// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace UnityEngine.UIElements
{
    public static class MouseCaptureController
    {
        internal static IEventHandler mouseCapture { get; private set; }

        public static bool IsMouseCaptured()
        {
            return mouseCapture != null;
        }

        public static bool HasMouseCapture(this IEventHandler handler)
        {
            return mouseCapture == handler;
        }

        public static void CaptureMouse(this IEventHandler handler)
        {
            if (mouseCapture == handler)
                return;

            if (handler == null)
            {
                ReleaseMouse();
                return;
            }

            if (GUIUtility.hotControl != 0)
            {
                Debug.Log("Should not be capturing when there is a hotcontrol");
                return;
            }

            // TODO: assign a reserved control id to hotControl so that repaint events in OnGUI() have their hotcontrol check behave normally

            IEventHandler currentMouseCapture = mouseCapture;
            mouseCapture = handler;

            if (currentMouseCapture != null)
            {
                using (MouseCaptureOutEvent releaseEvent = MouseCaptureOutEvent.GetPooled(currentMouseCapture, mouseCapture))
                {
                    currentMouseCapture.SendEvent(releaseEvent);
                }
            }

            using (MouseCaptureEvent captureEvent = MouseCaptureEvent.GetPooled(mouseCapture, currentMouseCapture))
            {
                mouseCapture.SendEvent(captureEvent);
            }
        }

        public static void ReleaseMouse(this IEventHandler handler)
        {
            if (handler == mouseCapture)
            {
                ReleaseMouse();
            }
        }

        public static void ReleaseMouse()
        {
            if (mouseCapture != null)
            {
                IEventHandler currentMouseCapture = mouseCapture;
                mouseCapture = null;

                using (MouseCaptureOutEvent e = MouseCaptureOutEvent.GetPooled(currentMouseCapture, null))
                {
                    currentMouseCapture.SendEvent(e);
                }
            }
        }
    }
}
