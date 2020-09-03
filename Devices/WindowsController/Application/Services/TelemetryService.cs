using System;
using System.Threading;
using WindowsController.Application.Events.Outbound;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using EspIot.Drivers.Switch;
using EspIot.Drivers.Switch.Enums;

namespace WindowsController.Application.Services
{
    public class TelemetryService: IService
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly SwitchDriver _window1ReedSwitch;
        private readonly SwitchDriver _window2ReedSwitch;
        private bool _isServiceRunning;
        private bool _runWorkingThread;
        private int _sentInterval = 1_800_000; //in [ms] 30 min
        private Thread _workingThread = new Thread(() => { });

        public TelemetryService(IOutboundEventBus outboundEventBus, SwitchDriver window1ReedSwitch,
            SwitchDriver window2ReedSwitch)
        {
            _outboundEventBus = outboundEventBus;
            _window1ReedSwitch = window1ReedSwitch;
            _window2ReedSwitch = window2ReedSwitch;
        }

        public void Start()
        {
            if (_isServiceRunning || _workingThread.IsAlive)
            {
                return;
            }

            _isServiceRunning = true;
            _runWorkingThread = true;

            _workingThread = new Thread(() =>
            {
                _outboundEventBus.Send(new DeviceStatusUpdatedEvent("Telemetry service was started",
                    DeviceStatusCode.ServiceStarted));
                while (_runWorkingThread)
                {
                    _outboundEventBus.Send(new WindowsControllerTelemetryEvent(new []{_window1ReedSwitch.GetState().ToBool(), _window2ReedSwitch.GetState().ToBool()}));
                    Thread.Sleep(_sentInterval);
                    
                }

                _outboundEventBus.Send(new DeviceStatusUpdatedEvent("Telemetry service was stopped",
                    DeviceStatusCode.ServiceStopped));
                _isServiceRunning = false;
            });

            _workingThread.Start();
        }

        public void Stop()
        {
            _runWorkingThread = false;
        }

        public bool IsRunning() => _isServiceRunning;
    }
}
