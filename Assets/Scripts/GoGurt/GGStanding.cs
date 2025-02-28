using Unity.Netcode;

public class GGStanding : NetworkBehaviour
{
    public NetworkVariable<float> progress = new NetworkVariable<float>(0f);
    public int currentRank;
}
