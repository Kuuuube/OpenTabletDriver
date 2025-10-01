using System;
using System.Numerics;
using OpenTabletDriver.Native.Windows.PenPointer;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace OpenTabletDriver.Desktop.Interop.Input.Absolute
{
    public class WindowsPenPointer : IPenActionHandler, IAbsolutePointer, IPressureHandler, ISynchronousPointer, ITiltHandler, IEraserHandler
    {
        private readonly PointerDevice _pointerDevice;
        private bool _inContact;
        private bool _lastContact;
        private bool _dirty;
        private bool _eraser = false;
        private static Vector2 ScreenToVirtualDesktop = new Vector2(DesktopInterop.VirtualScreen.Width, DesktopInterop.VirtualScreen.Height) / 65535;

        public WindowsPenPointer()
        {
            _pointerDevice = new PointerDevice();
            _inContact = false;
            _lastContact = false;
        }

        public void Flush()
        {
            if (_dirty)
            {
                _pointerDevice.Inject();
                _dirty = false;
            }
        }

        public void Reset()
        {
            _pointerDevice.UnsetPointerFlags(POINTER_FLAGS.INRANGE);
        }

        public void SetPosition(Vector2 pos)
        {
            _dirty = true;
            _pointerDevice.SetPointerFlags(POINTER_FLAGS.INRANGE);
            var pixelPoint = new POINT((int)Math.Round(pos.X), (int)Math.Round(pos.Y));
            var virtualDesktopCoords = pos / ScreenToVirtualDesktop;
            var subpixelPoint = new POINT((int)Math.Round(virtualDesktopCoords.X), (int)Math.Round(virtualDesktopCoords.Y));
            _pointerDevice.SetPosition(pixelPoint, subpixelPoint);
            if (_inContact != _lastContact)
            {
                if (_inContact)
                {
                    _pointerDevice.UnsetPointerFlags(POINTER_FLAGS.UP | POINTER_FLAGS.UPDATE);
                    _pointerDevice.SetPointerFlags(POINTER_FLAGS.DOWN);
                    _lastContact = _inContact;
                }
                else
                {
                    _pointerDevice.UnsetPointerFlags(POINTER_FLAGS.DOWN | POINTER_FLAGS.UPDATE);
                    _pointerDevice.SetPointerFlags(POINTER_FLAGS.UP);
                    _lastContact = _inContact;
                }
            }
            else
            {
                _pointerDevice.SetPointerFlags(POINTER_FLAGS.UPDATE);
            }
            _pointerDevice.SetTarget();
        }

        public void SetPressure(float percentage)
        {
            var pressure = (uint)(percentage * 1024);
            if (pressure > 0)
            {
                _pointerDevice.SetPressure(pressure);
                _pointerDevice.SetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = true;
            }
            else
            {
                _pointerDevice.SetPressure(1);
                _pointerDevice.UnsetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = false;
            }
        }

        public void SetTilt(Vector2 tilt)
        {
            _pointerDevice.SetTilt(tilt);
        }

        private static PEN_FLAGS GetCode(PenAction button) => button switch
        {
            PenAction.Tip => PEN_FLAGS.NONE, // tip is handled via pressure
            PenAction.Eraser => PEN_FLAGS.NONE, // eraser is handled via pressure
            PenAction.BarrelButton1 => PEN_FLAGS.BARREL,
            PenAction.BarrelButton2 => PEN_FLAGS.BARREL,
            PenAction.BarrelButton3 => PEN_FLAGS.BARREL,
            _ => PEN_FLAGS.NONE,
        };

        public void Activate(PenAction action)
        {
            _pointerDevice.SetPenFlags(GetCode(action));
        }

        public void Deactivate(PenAction action)
        {
            _pointerDevice.UnsetPenFlags(GetCode(action));
        }

        public void SetEraser(bool isEraser)
        {
            _eraser = isEraser;
            if (isEraser)
            {
                _pointerDevice.SetPenFlags(PEN_FLAGS.INVERTED);
            }
            else
            {
                _pointerDevice.UnsetPenFlags(PEN_FLAGS.INVERTED);
            }
        }
    }
}
