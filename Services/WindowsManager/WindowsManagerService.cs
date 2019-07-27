﻿using EspIot.Core.Messaging.Enum;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.ReedSwitch.Enums;
using Messages.Events.Outbound;
using Services.Events;
using System.Collections;
using System.Threading;

namespace Services.WindowsManager
{
    public class WindowsManagerService
    {
        private readonly LinearActuatorDriver[] _windowActuators;
        private readonly ReedSwitchDriver[] _windowReedSwitches;
        private readonly ReedSwitchDriver _doorReedSwitch;
        private readonly MqttOutboundEventBus _outboundEventBus;
        private readonly Hashtable _actionsOnWindows = new Hashtable();
        private const int ACTUATOR_WORK_TIMEOUT = 120_000; //maximum working time of window actuator in ms  

        public WindowsManagerService(
            LinearActuatorDriver window1Actuator,
            LinearActuatorDriver window2Actuator,
            ReedSwitchDriver window1ReedSwitch,
            ReedSwitchDriver window2ReedSwitch,
            ReedSwitchDriver doorReedSwitch,
            MqttOutboundEventBus outboundEventBus
            )
        {
            _windowActuators = new[] { window1Actuator, window2Actuator };
            _windowReedSwitches = new[] { window1ReedSwitch, window2ReedSwitch };
            _doorReedSwitch = doorReedSwitch;
            _outboundEventBus = outboundEventBus;
            SubscribeToEventHandlers();
        }

        public void OpenWindows(ushort[] windowIds)
        {

        }

        public void CloseWindows(ushort[] windowIds, OnSuccessEventHandler onSuccessEventHandler, OnFailureEventHandler onFailureEventHandler)
        {
            lock (this)
            {
                foreach (var windowId in windowIds)
                {
                    if (_actionsOnWindows.Contains(windowId))
                    {
                        onFailureEventHandler(this, new FailureEvent(StatusCode.Refused, $"Pending operation on window with id {windowId}"));
                        break;
                    }
                    
                    if (windowId >= _windowActuators.Length)
                    {
                        onFailureEventHandler(this, new FailureEvent(StatusCode.Error, $"Window with id  {windowId} not exists"));
                        break;
                    }

                    if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Closed)
                    {
                        continue;
                    }

                    _actionsOnWindows.Add(windowId, Operation.Closing);
                    _windowActuators[windowId].StartMovingExtensionDirection();
                }
            }

            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(ACTUATOR_WORK_TIMEOUT);

                foreach (var windowId in windowIds)
                {
                    if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Opened)
                    {
                        onFailureEventHandler(this, new FailureEvent(StatusCode.Refused, $"Window actuator mechanism critical failure. Window with id {windowId} is still opened."));
                        _actionsOnWindows.Remove(windowId);
                    }
                }
            })).Start();
        }

        public WindowsState GetWindowsState()
        {
            return new WindowsState(_windowReedSwitches[0].GetState().ToBool(), _windowReedSwitches[1].GetState().ToBool(), _doorReedSwitch.GetState().ToBool());
        }

        private void SubscribeToEventHandlers()
        {
            _doorReedSwitch.OnClosed += (sender, e) =>
            {
                _outboundEventBus.Send(new DoorClosedEvent());
            };

            _doorReedSwitch.OnOpened += (sender, e) =>
            {
                _outboundEventBus.Send(new DoorOpenedEvent());
            };

            for (ushort i = 0; i < _windowReedSwitches.Length; i++)
            {
                _windowReedSwitches[i].OnClosed += (sender, e) =>
                {
                    _outboundEventBus.Send(new WindowClosedEvent(i));
                    if (_actionsOnWindows.Contains(i))
                    {
                        _windowActuators[i].StopMoving();
                    }

                };

                _windowReedSwitches[i].OnOpened += (sender, e) =>
                {
                    _outboundEventBus.Send(new WindowOpenedEvent(i));
                };
            }
        }
    }
}
