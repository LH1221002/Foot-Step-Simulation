using HapticShoes;

namespace Footsteps
{

    public class VibrationData
    {
        public int Strength { get; set; }
        public int Volume { get; set; }
        public int Layers { get; set; }
        public ShoeController.Material Material { get; set; }
    }
}
