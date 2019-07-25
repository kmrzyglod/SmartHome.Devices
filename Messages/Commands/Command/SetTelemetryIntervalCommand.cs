﻿using EspIot.Core.Messaging.Interfaces;

namespace Messages.Commands.Command
{
    class SetTelemetryIntervalCommand : ICommand
    {
        public string CorrelationId { get; }
        //Telemetry send interval in seconds
        public int Interval { get;}
    }
}
