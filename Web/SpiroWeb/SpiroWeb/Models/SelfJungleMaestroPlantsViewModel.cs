using System.Collections.Generic;

namespace SpiroWeb.Models
{
    public class SelfJungleMaestroPlantsViewModel
    {
        public List<ClassLibrary1.SelfJungleMaestro_PlantStatus> Plants { get; set; }
        public ClassLibrary1.SelfJungleMaestro_RoomStatus RoomStatus { get; set; }
    }
}