using GreenhouseController.Drivers;

namespace GreenhouseController.Services
{
    class ServiceFactory
    {
        private readonly DriversFactory _driversFactory;

        public ServiceFactory(DriversFactory driversFactory)
        {
            _driversFactory = driversFactory;
        }

    }
}
