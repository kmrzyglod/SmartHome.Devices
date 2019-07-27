using EspIot.Core.Messaging.Enum;
using EspIot.Core.Messaging.Events;
using EspIot.Drivers.LinearActuator;
using EspIot.Drivers.ReedSwitch;
using EspIot.Drivers.ReedSwitch.Enums;
using Messages.Commands.Command;
using System.Threading;

namespace Messages.Commands.Handler
{
    class CloseWindowCommandHandler
    {
        private readonly CommandResultEventHandler _commandResultEventHandler;
        private readonly LinearActuatorDriver[] _windowActuators;
        private readonly ReedSwitchDriver[] _windowReedSwitches;
        private const int ACTUATOR_WORK_TIMEOUT = 120_000; //maximum working time of window actuator in ms  

        public CloseWindowCommandHandler(CommandResultEventHandler commandResultEventHandler,
            LinearActuatorDriver window1Actuator,
            LinearActuatorDriver window2Actuator,
            ReedSwitchDriver window1ReedSwitch,
            ReedSwitchDriver window2ReedSwitch)
        {
            _commandResultEventHandler = commandResultEventHandler;
            _windowActuators = new[] { window1Actuator, window2Actuator };
            _windowReedSwitches = new[] { window1ReedSwitch, window2ReedSwitch };
        }

        public Thread Handle(CloseWindowCommand command)
        {
            var commandWorker = new Thread(new ThreadStart(() =>
            {
                foreach (var windowId in command.WindowIds)
                {
                    if (windowId >= _windowActuators.Length)
                    {
                        _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, StatusCode.Error, $"Window with id  {windowId} not exists"));
                        continue;
                    }

                    if (_windowReedSwitches[windowId].GetState() == ReedShiftState.Closed)
                    {
                        _commandResultEventHandler(this, new CommandResultEvent(command.CorrelationId, StatusCode.Refused, $"Window {windowId} already closed"));
                        continue;
                    }

                    _windowActuators[windowId].StartMovingExtensionDirection();
                }

                Thread.Sleep(ACTUATOR_WORK_TIMEOUT);
                
                foreach (var windowId in command.WindowIds)
                {
                    if(_windowReedSwitches[windowId].GetState() == ReedShiftState.Opened)
                    {
                        new CommandResultEvent(command.CorrelationId, StatusCode.Error, $"Window actuator mechanism critical failure. Window with id {windowId} is still opened.");
                    }
                }
            }));
            
            commandWorker.Start();
            
            return commandWorker;
        }

        private void OnWindowClosed()
    }
}
