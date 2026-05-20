using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    public int Gold { get; set; } = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameStart() {

        UIManager.Inst.GameStartUI();
    }
    public bool AddGold(int getGold) {

        int newGold = Gold + getGold;
        if (newGold<0)
        {
            return false;
        }
        else {
        Gold= newGold;
            return true;
        
        }
    }
}
