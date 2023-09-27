using Assets.Scripts.DTO;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class ArrayOfClientsDTO
    {
        public ClientDTO[]? clientsDTO { get; set; }
    }
}
