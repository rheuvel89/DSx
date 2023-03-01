using System;
using System.Threading;
using System.Threading.Tasks;
using CoreDX.vJoy.Wrapper;

namespace DSx.VJoy
{
    public class VJoyController : IVJoyController
    {
        private readonly VJoyControllerManager _manager;
        private readonly uint _id;
        private IVJoyController _controller;
        private IVJoyController _controllerGuard => _controller != null ? _controller : throw new Exception($"Controller not acquired or failed to acquire controller with ID {_id}");

        public VJoyController(VJoyControllerManager manager, uint id)
        {
            _manager = manager;
            _id = id;
        }

        public void Acquire()
        {
            _controller = _manager.AcquireController(_id);
        }


        public void Dispose()
        {
            _controllerGuard.Dispose();
        }

        public bool Reset()
        {
            return _controllerGuard.Reset();
        }

        public bool ResetButtons()
        {
            return _controllerGuard.ResetButtons();
        }

        public bool ResetPovs()
        {
            return _controllerGuard.ResetPovs();
        }

        public bool SetAxisX(int value)
        {
            return _controllerGuard.SetAxisX(value);
        }

        public bool SetAxisY(int value)
        {
            return _controllerGuard.SetAxisY(value);
        }

        public bool SetAxisZ(int value)
        {
            return _controllerGuard.SetAxisZ(value);
        }

        public bool SetAxisRx(int value)
        {
            return _controllerGuard.SetAxisRx(value);
        }

        public bool SetAxisRy(int value)
        {
            return _controllerGuard.SetAxisRy(value);
        }

        public bool SetAxisRz(int value)
        {
            return _controllerGuard.SetAxisRz(value);
        }

        public bool SetSlider0(int value)
        {
            return _controllerGuard.SetSlider0(value);
        }

        public bool SetSlider1(int value)
        {
            return _controllerGuard.SetSlider1(value);
        }

        public bool SetWheel(int value)
        {
            return _controllerGuard.SetWheel(value);
        }

        public bool PressButton(uint btnNo)
        {
            return _controllerGuard.PressButton(btnNo);
        }

        public bool ReleaseButton(uint btnNo)
        {
            return _controllerGuard.ReleaseButton(btnNo);
        }

        public bool ClickButton(uint btnNo, int milliseconds)
        {
            return _controllerGuard.ClickButton(btnNo, milliseconds);
        }

        public Task<bool> ClickButtonAsync(uint btnNo, int milliseconds, CancellationToken token)
        {
            return _controllerGuard.ClickButtonAsync(btnNo, milliseconds, token);
        }

        public bool SetContPov(int value, uint povNo)
        {
            return _controllerGuard.SetContPov(value, povNo);
        }

        public bool SetDiscPov(int value, uint povNo)
        {
            return _controllerGuard.SetDiscPov(value, povNo);
        }

        public uint Id => _controllerGuard.Id;

        public bool HasRelinquished => _controllerGuard.HasRelinquished;

        public bool HasAxisX => _controllerGuard.HasAxisX;

        public bool HasAxisY => _controllerGuard.HasAxisY;

        public bool HasAxisZ => _controllerGuard.HasAxisZ;

        public bool HasAxisRx => _controllerGuard.HasAxisRx;

        public bool HasAxisRy => _controllerGuard.HasAxisRy;

        public bool HasAxisRz => _controllerGuard.HasAxisRz;

        public bool HasSlider0 => _controllerGuard.HasSlider0;

        public bool HasSlider1 => _controllerGuard.HasSlider1;

        public bool HasWheel => _controllerGuard.HasWheel;

        public int ButtonCount => _controllerGuard.ButtonCount;

        public int ContPovCount => _controllerGuard.ContPovCount;

        public int DiscPovCount => _controllerGuard.DiscPovCount;

        public long? AxisMaxValue => _controllerGuard.AxisMaxValue;
    }
}