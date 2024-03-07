using TMPro;
using UnityEngine;

public class WeaponAmount: MonoBehaviour
{
  public TMP_Text weaponNum;

  public void Start()
  {
    if (weaponNum == null)
    {
      weaponNum = GetComponent<TMP_Text>();
    }
  }
  public void TextUpdate(string message)
  {
     if (weaponNum == null)
    {
      weaponNum = GetComponent<TMP_Text>();
    }

    weaponNum.SetText(message);
  }
}