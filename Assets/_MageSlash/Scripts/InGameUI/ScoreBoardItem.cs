using TMPro;
using Unity.Collections;
using UnityEngine;

public class ScoreBoardItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameTmp;
    [SerializeField] TextMeshProUGUI scoreTmp;

    public ulong clientId;
    public FixedString128Bytes username;
    public int score;

    public void SetScore(ulong clientId, FixedString128Bytes username, int score)
    {
        this.clientId = clientId;
        this.username = username;
        this.score = score;

        userNameTmp.text = username.ToString();
        scoreTmp.text = score.ToString();
    }
}
