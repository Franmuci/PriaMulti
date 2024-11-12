using Fusion;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked] public string Nickname { get; private set; }

    public void SetNickname(string nickname)
    {
        if (Object.HasStateAuthority)
        {
            Nickname = nickname;
        }
    }
}
