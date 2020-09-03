using System;
using System.Threading;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Messaging.Enum;
using EspIot.Drivers.SeedstudioWaterFlowSensor;
using EspIot.Drivers.SeedstudioWaterFlowSensor.Enums;
using EspIot.Drivers.SoildStateRelay;
using GreenhouseController.Application.Events.Outbound;

namespace GreenhouseController.Application.Services.Irrigation
{
    public class IrrigationService : IService
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly ushort _relaySwitchValveChannel;
        private readonly WaterFlowSensorDriver _waterFlowSensorDriver;
        private bool _irrigationInProgress;
        private DateTime _irrigationStartTime;

        private bool _isServiceRunning;
        private bool _runWorkingThread;

        private Thread _workingThread = new Thread(() => { });
        private Thread _flowWatcherThread = new Thread(() => { });


        public IrrigationService(SolidStateRelaysDriver solidStateRelays, ushort relaySwitchPumpChannel,
            ushort relaySwitchValveChannel, WaterFlowSensorDriver waterFlowSensorDriver,
            IOutboundEventBus outboundEventBus)
        {
            _relaySwitchValveChannel = relaySwitchValveChannel;
            _waterFlowSensorDriver = waterFlowSensorDriver;
            _outboundEventBus = outboundEventBus;
            _solidStateRelays = solidStateRelays;
            _relaySwitchPumpChannel = relaySwitchPumpChannel;
        }

        private SolidStateRelaysDriver _solidStateRelays { get; }
        private ushort _relaySwitchPumpChannel { get; }

        public void Start()
        {
            _isServiceRunning = true;
        }

        public void Stop()
        {
            if (_irrigationInProgress)
            {
                FinishIrrigation();
            }
            _isServiceRunning = false;
        }

        public bool IsRunning() => _isServiceRunning;

        public void StartIrrigation(int maximumIrrigationTime, int waterVolume)
        {
            if (_irrigationInProgress)
            {
                throw new Exception(
                    $"Irrigation process is running. Cannot start another process.");
            }

            if (!IsRunning())
            {
                throw new Exception(
                    $"Irrigation service is stopped. Cannot start irrigation");
            }

            lock (this)
            {
                _irrigationInProgress = true;
                _runWorkingThread = true;
                _waterFlowSensorDriver.StartMeasurement(WaterFlowSensorMeasurementResolution.FiveSeconds);
                _irrigationStartTime = DateTime.UtcNow;
                _flowWatcherThread = new Thread(() =>
                {
                    float previousTotalFlow = 0;
                    while (_runWorkingThread)
                    {
                        float totalFlow = _waterFlowSensorDriver.GetTotalFlow();
                        if (totalFlow <= previousTotalFlow)
                        {
                            FinishIrrigation();
                            _outboundEventBus.Send(new ErrorEvent(
                                "Irrigation process aborted because no water flow was detected. Irrigation system malfunction or no water in tank.",
                                ErrorLevel.Warning));
                            break;
                        }

                        if (totalFlow >= waterVolume)
                        {
                            FinishIrrigation();
                            break;
                        }

                        Thread.Sleep(10000);
                        if (_flowWatcherThread != Thread.CurrentThread)
                        {
                            break;
                        }
                    }
                });

                _workingThread = new Thread(() =>
                {
                    //Open valve and wait 5 seconds
                    _solidStateRelays.On(_relaySwitchValveChannel);
                    Thread.Sleep(5000);

                    //Start water pump and run water flow watcher thread after waiting 10 seconds
                    _solidStateRelays.On(_relaySwitchPumpChannel);
                    Thread.Sleep(10000);
                    _flowWatcherThread.Start();
                    Thread.Sleep(maximumIrrigationTime * 1000);
                    if (_flowWatcherThread != Thread.CurrentThread)
                    {
                        return;
                    }
                    if (!_runWorkingThread)
                    {
                        return;
                    }

                    FinishIrrigation();
                });

                _workingThread.Start();
            }
        }

        public void FinishIrrigation()
        {
            lock (this)
            {
                if (!_irrigationInProgress)
                {
                    throw new Exception(
                        $"Irrigation process is not running. Cannot abort.");
                }

                _runWorkingThread = false;
                _solidStateRelays.Off(_relaySwitchPumpChannel);
                Thread.Sleep(5000);
                _solidStateRelays.Off(_relaySwitchValveChannel);
                _outboundEventBus.Send(new IrrigationFinishedEvent(_waterFlowSensorDriver.GetTotalFlow(),
                    _waterFlowSensorDriver.GetAverageFlow(), _waterFlowSensorDriver.GetMinFlow(),
                    _waterFlowSensorDriver.GetMaxFlow(), _irrigationStartTime, DateTime.UtcNow));
                _waterFlowSensorDriver.StopMeasurement();
                _irrigationInProgress = false;
            }
        }

        public void ScheduleIrrigation()
        {
            throw new NotImplementedException();
        }
    }
}