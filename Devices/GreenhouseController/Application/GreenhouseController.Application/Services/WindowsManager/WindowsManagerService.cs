using System.Collections;
using System.Threading;
using EspIot.Core.Messaging.Concrete;
using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.ReedSwitch.Enums;
using GreenhouseController.Application.Events.Internal;
using GreenhouseController.Application.Events.Outbound;

namespace GreenhouseController.Application.Services.WindowsManager
{
    public class WindowsManagerService
    {
        private const int ACTUATOR_WORK_TIMEOUT = 120_000; //maximum working time of window actuator in ms  
        private readonly Hashtable _actionsOnWindows = new Hashtable();
        private readonly ReedSwitchDriver _doorReedSwitch;
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly LinearActuatorDriver[] _windowActuators;
        private readonly ReedSwitchDriver[] _windowReedSwitches;

        public WindowsManagerService(
            LinearActuatorDriver window1Actuator,
            LinearActuatorDriver window2Actuator,
            ReedSwitchDriver window1ReedSwitch,
            ReedSwitchDriver window2ReedSwitch,
            ReedSwitchDriver doorReedSwitch,
            IOutboundEventBus outboundEventBus
        )
        {
            _windowActuators = new[] {window1Actuator, window2Actuator};
            _windowReedSwitches = new[] {window1ReedSwitch, window2ReedSwitch};
            _doorReedSwitch = doorReedSwitch;
            _outboundEventBus = outboundEventBus;
            SubscribeToEventHandlers();
        }

        public void CloseWindows(ushort[] windowIds, OnSuccessEventHandler onSuccessEventHandler,
            OnFailureEventHandler onFailureEventHandler)
        {
            foreach (ushort windowId in windowIds)
            {
                if (_actionsOnWindows.Contains(windowId))
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Refused,
                            $"Pending operation on window with id {windowId}"));
                    break;
                }

                if (windowId >= _windowActuators.Length)
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Error, $"Window with id  {windowId} not exists"));
                    break;
                }

                if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Closed)
                {
                    continue;
                }

                _actionsOnWindows.Add(windowId, Operation.Closing);
                _windowActuators[windowId].StartMovingExtensionDirection();
            }

            Thread.Sleep(ACTUATOR_WORK_TIMEOUT);
            foreach (ushort windowId in windowIds)
            {
                _windowActuators[windowId].StopMoving();
                if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Opened)
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Refused,
                            $"Window actuator mechanism critical failure. Window with id {windowId} is still opened."));
                    _actionsOnWindows.Remove(windowId);
                }
            }
        }

        public void OpenWindows(ushort[] windowIds, OnSuccessEventHandler onSuccessEventHandler,
            OnFailureEventHandler onFailureEventHandler)
        {
            foreach (ushort windowId in windowIds)
            {
                if (_actionsOnWindows.Contains(windowId))
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Refused,
                            $"Pending operation on window with id {windowId}"));
                    break;
                }

                if (windowId >= _windowActuators.Length)
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Error, $"Window with id  {windowId} not exists"));
                    break;
                }

                if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Opened)
                {
                    continue;
                }

                _actionsOnWindows.Add(windowId, Operation.Opening);
                _windowActuators[windowId].StartMovingReductionDirection();
            }

            Thread.Sleep(ACTUATOR_WORK_TIMEOUT);

            foreach (ushort windowId in windowIds)
            {
                _windowActuators[windowId].StopMoving();
                if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Closed)
                {
                    onFailureEventHandler(this,
                        new ProcessingFailureEvent(StatusCode.Refused,
                            $"Window actuator mechanism critical failure. Window with id {windowId} is still closed."));
                    _actionsOnWindows.Remove(windowId);
                }
            }
        }

        public WindowsState GetWindowsState()
        {
            return new WindowsState(_windowReedSwitches[0].GetState().ToBool(),
                _windowReedSwitches[1].GetState().ToBool(), _doorReedSwitch.GetState().ToBool());
        }

        private void SubscribeToEventHandlers()
        {
            _doorReedSwitch.OnClosed += (sender, e) => { _outboundEventBus.Send(new DoorClosedEvent()); };

            _doorReedSwitch.OnOpened += (sender, e) => { _outboundEventBus.Send(new DoorOpenedEvent()); };

            _windowReedSwitches[0].OnClosed += (sender, e) =>
            {
                _outboundEventBus.Send(new WindowClosedEvent(0));
                if (_actionsOnWindows.Contains(0))
                {
                    _windowActuators[0].StopMoving();
                }
            };

            _windowReedSwitches[0].OnOpened += (sender, e) => { _outboundEventBus.Send(new WindowOpenedEvent(0)); };

            _windowReedSwitches[1].OnClosed += (sender, e) =>
            {
                _outboundEventBus.Send(new WindowClosedEvent(1));
                if (_actionsOnWindows.Contains(1))
                {
                    _windowActuators[1].StopMoving();
                }
            };

            _windowReedSwitches[1].OnOpened += (sender, e) => { _outboundEventBus.Send(new WindowOpenedEvent(1)); };
        }
    }
}