using Data.Models.SPU;
using IntelliHome_Backend.Features.Home.Handlers.Interfaces;

namespace IntelliHome_Backend.Features.SPU.Handlers.Interfaces
{
    public interface ILampHandler : ISmartDeviceHandler
    {
        void ChangeMode(Lamp lamp, bool isAuto);
        void ChangeBrightnessLimit(Lamp lamp, double brightness);

        void TurnLampOn(Lamp lamp, Boolean isOn);
    }
}
