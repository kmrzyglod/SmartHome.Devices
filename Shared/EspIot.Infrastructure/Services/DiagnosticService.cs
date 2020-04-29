using System;
using System.Threading;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Application.Services;
using EspIot.Infrastructure.Wifi;
using nanoFramework.Runtime.Native;

namespace EspIot.Infrastructure.Services
{
    public class DiagnosticService : IDiagnosticService
    {
        private const ushort MIN_INTERVAL = 30000;
        private readonly IOutboundEventBus _outboundEventBus;
        private int _interval = 3_600_000; //One hour default
        private bool _isRunning;
        private bool _runWorkingThread;
        private Thread _workingThread = new Thread(() => { });

        public DiagnosticService(IOutboundEventBus outboundEventBus)
        {
            _outboundEventBus = outboundEventBus;
        }

        public void Start()
        {
            if (_isRunning || _workingThread.IsAlive)
            {
                return;
            }

            _workingThread = new Thread(() =>
            {
                _runWorkingThread = _isRunning = true;

                while (_runWorkingThread)
                {
                    _outboundEventBus.Send(GetDiagnosticDataEvent());
                    Thread.Sleep(_interval);
                }
            });

            _workingThread.Start();
        }

        public void Stop()
        {
            _runWorkingThread = false;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public DiagnosticService WithInterval(int interval)
        {
            if (_interval < MIN_INTERVAL)
            {
                throw new ArgumentOutOfRangeException(nameof(interval),
                    $"Interval must be greater or equal to {MIN_INTERVAL} ms");
            }

            _interval = interval;
            return this;
        }

        public DiagnosticEvent GetDiagnosticDataEvent()
        {
            var networkConfig = WifiDriver.GetNetworkConfig();
            return new DiagnosticEvent(networkConfig.Ssid, networkConfig.Rssi, networkConfig.Ip,
                networkConfig.GatewayIp, Debug.GC(false));
        }
    }
}