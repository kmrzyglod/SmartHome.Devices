﻿using System;
using EspIot.Core.Messaging.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class DoorOpenedEvent : IEvent
    {
        public DoorOpenedEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        public string CorrelationId { get; }
    }
}
