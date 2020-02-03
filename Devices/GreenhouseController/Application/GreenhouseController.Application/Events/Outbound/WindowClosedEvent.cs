﻿using System;
using EspIot.Core.Messaging.Interfaces;

namespace GreenhouseController.Application.Events.Outbound
{
    public class WindowClosedEvent : IEvent
    {
        public WindowClosedEvent(ushort windowNum)
        {
            CorrelationId = new Guid().ToString();
            WindowNum = windowNum;
        }
        
        public string CorrelationId { get; }
        public ushort WindowNum { get; }
    }
}