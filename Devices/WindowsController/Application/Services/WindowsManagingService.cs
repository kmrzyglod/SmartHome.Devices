using System;
using System.Threading;
using WindowsController.Application.Events.Outbound;
using EspIot.Application.Events.Outbound;
using EspIot.Application.Interfaces;
using EspIot.Core.Helpers;
using EspIot.Core.Messaging.Enum;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.Switch;
using EspIot.Drivers.Switch.Enums;
using EspIot.Drivers.Switch.Events;

namespace WindowsController.Application.Services
{
    public class WindowsManagingService : IService
    {
        private const int ACTUATOR_WORK_TIMEOUT = 120_000;
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly LinearActuatorDriver[] _windowActuators;
        private readonly SwitchDriver[] _windowControlSwitches;
        private readonly SwitchDriver[] _windowReedSwitches;

        private readonly SwitchState[] _windowStates;

        //Tasks
        private readonly Thread[] _workerThreads = {new Thread(() => { }), new Thread(() => { })};
        private readonly SwitchClosedEventHandler OnWindow1Closed;
        private readonly SwitchClosedEventHandler OnWindow1ControlSwitchClosed;
        private readonly SwitchOpenedEventHandler OnWindow1ControlSwitchOpened;

        //Event handlers 
        private readonly SwitchOpenedEventHandler OnWindow1Opened;
        private readonly SwitchClosedEventHandler OnWindow2Closed;
        private readonly SwitchClosedEventHandler OnWindow2ControlSwitchClosed;
        private readonly SwitchOpenedEventHandler OnWindow2ControlSwitchOpened;
        private readonly SwitchOpenedEventHandler OnWindow2Opened;

        private bool _isRunning;
        private bool _criticalFailure = false;

        public WindowsManagingService(LinearActuatorDriver window1Actuator,
            LinearActuatorDriver window2Actuator,
            SwitchDriver window1ReedSwitch,
            SwitchDriver window2ReedSwitch,
            SwitchDriver window1ControlSwitch,
            SwitchDriver window2ControlSwitch,
            IOutboundEventBus outboundEventBus)
        {
            _windowActuators = new[] {window1Actuator, window2Actuator};
            _windowReedSwitches = new[] {window1ReedSwitch, window2ReedSwitch};
            _windowControlSwitches = new[] {window1ControlSwitch, window2ControlSwitch};
            _windowStates = new[] {window1ReedSwitch.GetState(), window2ReedSwitch.GetState()};
            _outboundEventBus = outboundEventBus;

            OnWindow1Opened = (sender, e) =>
            {
                _windowStates[0] = SwitchState.Opened;
                _outboundEventBus.Send(new WindowOpenedEvent(0));
            };
            OnWindow1Closed = (sender, e) =>
            {
                _windowStates[0] = SwitchState.Closed;
                _windowActuators[0].StopMoving();
                _outboundEventBus.Send(new WindowClosedEvent(0));
            };

            OnWindow2Opened = (sender, e) =>
            {
                _windowStates[1] = SwitchState.Opened;
                _outboundEventBus.Send(new WindowOpenedEvent(1));
            };

            OnWindow2Closed = (sender, e) =>
            {
                _windowStates[1] = SwitchState.Closed;
                _windowActuators[1].StopMoving();
                _outboundEventBus.Send(new WindowClosedEvent(1));
            };

            //Control switches
            OnWindow1ControlSwitchOpened = (sender, e) => { OpenWindowAsync(0); };
            OnWindow1ControlSwitchClosed = (sender, e) =>
            {
                if (_windowStates[0] == SwitchState.Closed)
                {
                    return;
                }

                CloseWindowAsync(0);
            };

            OnWindow2ControlSwitchOpened = (sender, e) => { OpenWindowAsync(1); };

            OnWindow2ControlSwitchClosed = (sender, e) =>
            {
                if (_windowStates[1] == SwitchState.Closed)
                {
                    return;
                }

                CloseWindowAsync(1);
            };
        }

        public void Start()
        {
            SubscribeToEventHandlers();
            _isRunning = true;
        }

        public void Stop()
        {
            UnsubscribeFromEventHandlers();
            foreach (var workerThread in _workerThreads)
            {
                workerThread.Join();
            }

            _isRunning = false;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public bool IsPendingOperationOnWindow(ushort windowId)
        {
            return _workerThreads[windowId].IsAlive;
        }

        private void OpenWindowAsync(ushort windowId)
        {
            lock (this)
            {
                if (_criticalFailure)
                {
                    return;
                }

                _workerThreads[windowId] = new Thread(() =>
                {
                    _windowActuators[windowId].StartMovingExtensionDirection();
                    Logger.Log($"Start openning window {windowId}");

                    for (int i = 0; i <= ACTUATOR_WORK_TIMEOUT; i += 500)
                    {
                        Thread.Sleep(500);
                        if (_workerThreads[windowId] != Thread.CurrentThread)
                        {
                            Logger.Log($"Openning window {windowId} aborted.");
                            return;
                        }
                    }

                    _windowActuators[windowId].StopMoving();
                    if (_windowStates[windowId] == SwitchState.Closed)
                    {
                        _criticalFailure = true;
                        _outboundEventBus.Send(new ErrorEvent($"Window actuator mechanism critical failure. Window with id {windowId} is still closed.", ErrorLevel.Critical));
                    }
                    Logger.Log($"Openning window {windowId} finished.");
                });

                _workerThreads[windowId].Start();
            }
        }

        public void OpenWindow(ushort windowId)
        {
            CanBeExecuted(windowId);
            OpenWindowAsync(windowId);
            _workerThreads[windowId].Join();

            if (_windowReedSwitches[windowId].GetState() == SwitchState.Closed)
            {
                throw new Exception(
                    $"Window actuator mechanism critical failure. Window with id {windowId} is still closed.");
            }
        }

        private void CloseWindowAsync(ushort windowId)
        {
            lock (this)
            {
                if (_windowStates[windowId] == SwitchState.Closed || _criticalFailure)
                {
                    return;
                }

                _workerThreads[windowId] = new Thread(() =>
                {
                    _windowActuators[windowId].StartMovingReductionDirection();
                    Logger.Log($"Start closing window {windowId}");
    
                    for (int i = 0; i <= ACTUATOR_WORK_TIMEOUT; i += 500)
                    {
                        Thread.Sleep(500);
                        if (_workerThreads[windowId] != Thread.CurrentThread)
                        {
                            Logger.Log($"Closing window {windowId} aborted.");
                            return;
                        }

                        if (_windowStates[windowId] == SwitchState.Closed)
                        {
                            break;
                        }
                    }

                    _windowActuators[windowId].StopMoving();
                    if (_windowStates[windowId] == SwitchState.Opened)
                    {
                        _criticalFailure = true;
                        _outboundEventBus.Send(new ErrorEvent($"Window actuator mechanism critical failure. Window with id {windowId} is still opened.", ErrorLevel.Critical));
                    }
                    Logger.Log($"Closing window {windowId} finished.");
                });

                _workerThreads[windowId].Start();
            }
        }

        public void CloseWindow(ushort windowId)
        {
            CanBeExecuted(windowId);
            CloseWindowAsync(windowId);
            _workerThreads[windowId].Join();
            if (_windowReedSwitches[windowId].GetState() == SwitchState.Opened)
            {
                throw new Exception(
                    $"Window actuator mechanism critical failure. Window with id {windowId} is still opened.");
            }
        }

        private void CanBeExecuted(ushort windowId)
        {
            if (_criticalFailure)
            {
                throw new InvalidOperationException("Critical error was detected and mechanism was blocked. Check windows mechanism and restart device.");
            }
            
            if (!_isRunning)
            {
                throw new InvalidOperationException("Windows managing service is not started");
            }

            if (IsPendingOperationOnWindow(windowId))
            {
                throw new InvalidOperationException($"Pending operation on window with id {windowId}");
            }
        }

        private void SubscribeToEventHandlers()
        {
            _windowReedSwitches[0].OnOpened += OnWindow1Opened;
            _windowReedSwitches[0].OnClosed += OnWindow1Closed;

            _windowReedSwitches[1].OnOpened += OnWindow2Opened;
            _windowReedSwitches[1].OnClosed += OnWindow2Closed;

            _windowControlSwitches[0].OnOpened += OnWindow1ControlSwitchOpened;
            _windowControlSwitches[0].OnClosed += OnWindow1ControlSwitchClosed;

            _windowControlSwitches[1].OnOpened += OnWindow2ControlSwitchOpened;
            _windowControlSwitches[1].OnClosed += OnWindow2ControlSwitchClosed;
        }

        private void UnsubscribeFromEventHandlers()
        {
            _windowReedSwitches[0].OnOpened -= OnWindow1Opened;
            _windowReedSwitches[0].OnClosed -= OnWindow1Closed;

            _windowReedSwitches[1].OnOpened -= OnWindow2Opened;
            _windowReedSwitches[1].OnClosed -= OnWindow2Closed;

            _windowControlSwitches[0].OnOpened -= OnWindow1ControlSwitchOpened;
            _windowControlSwitches[0].OnClosed -= OnWindow1ControlSwitchClosed;

            _windowControlSwitches[1].OnOpened -= OnWindow2ControlSwitchOpened;
            _windowControlSwitches[1].OnClosed -= OnWindow2ControlSwitchClosed;
        }
    }
}