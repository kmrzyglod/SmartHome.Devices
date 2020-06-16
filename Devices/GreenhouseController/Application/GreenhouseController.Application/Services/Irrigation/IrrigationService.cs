using EspIot.Application.Interfaces;
using EspIot.Drivers.SoildStateRelay;

namespace GreenhouseController.Application.Services.Irrigation
{
    public class IrrigationService: IService
    {
        public SolidStateRelaysDriver SolidStateRelays { get; }
        public short RelaySwitchPumpChannel { get; }

        public IrrigationService(SolidStateRelaysDriver solidStateRelays, short relaySwitchPumpChannel)
        {
            SolidStateRelays = solidStateRelays;
            RelaySwitchPumpChannel = relaySwitchPumpChannel;
        }

        public void Start()
        {
            
        }

        public void Stop()
        {
           
        }

        public bool IsRunning()
        {
            return true;
        }
    }
}
