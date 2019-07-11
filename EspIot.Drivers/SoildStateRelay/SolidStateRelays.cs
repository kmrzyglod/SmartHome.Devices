using Windows.Devices.Gpio;

namespace EspIot.Drivers.SoildStateRelay
{
    public class SolidStateRelays
    {
        private readonly int _channelsNum;
        private readonly GpioPin[] _channels;
        private readonly GpioController _gpioController;
        private readonly Mode _mode;
        private readonly bool[] _channelsStates;

        public SolidStateRelays(GpioController gpioController, short[] pins, Mode mode = Mode.Low)
        {
            _channelsNum = pins.Length;
            _channels = new GpioPin[_channelsNum];
            _channelsStates = new bool[_channelsNum];
            _gpioController = gpioController;
            _mode = mode;
            InitChannels(pins);
        }

        public void On(params ushort[] channels)
        {
            foreach (var channel in channels)
            {
                CheckIfChannelNumValid(channel);
                _channels[channel].Write(_mode == Mode.Low ? GpioPinValue.Low : GpioPinValue.High);
                _channelsStates[channel] = true;
            }
        }

        public void Off(params ushort[] channels)
        {
            foreach (var channel in channels)
            {
                CheckIfChannelNumValid(channel);
                _channels[channel].Write(_mode == Mode.Low ? GpioPinValue.High : GpioPinValue.Low);
                _channelsStates[channel] = false;
            }
        }

        public bool GetChannelState(ushort channel)
        {
            CheckIfChannelNumValid(channel);
            return _channelsStates[channel];
        }

        private void CheckIfChannelNumValid(ushort channel)
        {
            if (channel >= _channelsNum)
            {
                throw new System.Exception("Solid state relay channel out of range");
            }
        }

        private void InitChannels(short[] pins)
        {
            for (int i = 0; i < _channelsNum; i++)
            {   
                _channels[i] = _gpioController.OpenPin(pins[i]);
                _channels[i].SetDriveMode(GpioPinDriveMode.Output);
                _channels[i].Write(_mode == Mode.Low ? GpioPinValue.High : GpioPinValue.Low);
            }
        }
    }
}
