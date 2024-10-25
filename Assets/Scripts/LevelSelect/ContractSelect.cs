using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ContractManager.Instance.SetContract(FrogContracts.PlaceholderFrogContract, new ContractValues(true, true));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
