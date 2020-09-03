using System;
using EspIot.Application.Interfaces;
using EspIot.Drivers.Switch;
using EspIot.Drivers.Switch.Events;
using GreenhouseController.Application.Events.Outbound;

namespace GreenhouseController.Application.Services.Door
{
    public class DoorService: IService
    {
        private readonly IOutboundEventBus _outboundEventBus;
        private readonly SwitchDriver _doorReedSwitch;

        private readonly SwitchOpenedEventHandler OnDoorOpened;
        private readonly SwitchClosedEventHandler OnDoorClosed;

        private bool _isRunning;

        public DoorService(IOutboundEventBus outboundEventBus, SwitchDriver doorReedSwitch)
        {
            _outboundEventBus = outboundEventBus;
            _doorReedSwitch = doorReedSwitch;

            OnDoorOpened = (sender, e) =>
            {
                _outboundEventBus.Send(new DoorOpenedEvent());
            };
            OnDoorClosed = (sender, e) =>
            {
                _outboundEventBus.Send(new DoorClosedEvent());
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
            _isRunning = false;
        }

        public bool IsRunning() => _isRunning;

        private void SubscribeToEventHandlers()
        {
            _doorReedSwitch.OnOpened += OnDoorOpened;
            _doorReedSwitch.OnClosed += OnDoorClosed;
        }

        private void UnsubscribeFromEventHandlers()
        {
            _doorReedSwitch.OnOpened -= OnDoorOpened;
            _doorReedSwitch.OnClosed -= OnDoorClosed;
        }
    }
}
