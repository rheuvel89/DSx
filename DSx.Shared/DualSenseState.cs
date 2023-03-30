using DualSenseAPI;

namespace DSx.Mapping
{
    public struct DualSenseState
    {
        public DualSenseState()
        {
            IoMode = IoMode.Bluetooth;
            IsCharging = false;
            IsFullyCharged = false;
            BatteryLevel = 0;
        }
        
        public IoMode IoMode { get; set; }
        public bool IsCharging { get; set; }
        public bool IsFullyCharged { get; set; }
        public float BatteryLevel { get; set; }
    }
}