﻿using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using Towers;
using UnityEngine;

public class TowerConfigManager : MonoBehaviour
{
    public static Tower selectedTower;
    [SerializeField]
    private GameObject towerConfigPanel;
    [SerializeField]
    private TowerInventoryItem prefab;
    [SerializeField]
    private RectTransform contentWindow;
    [SerializeField]
    private GameObject scrollView;
    [SerializeField]
    private GameObject EmptyPanel;

    public float YOffsetMultiplier = 0;
    public float Yoffset = -18.2f;
    public float Xoffset = 208.7f;

    List<TowerInventoryItem> itemListings = new List<TowerInventoryItem>();

    // Start is called before the first frame update
    void Start()
    {
        towerConfigPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedTower == null)
            towerConfigPanel.SetActive(false);
        else
            towerConfigPanel.SetActive(true);


        if (selectedTower != null)
        {
            if (selectedTower.inv.items.Count > 0)
            {
                scrollView.SetActive(true);
                EmptyPanel.SetActive(false);
                if (itemListings.Count != selectedTower.inv.items.Count)
                {
                    foreach (TowerInventoryItem listing in itemListings)
                        Destroy(listing.gameObject);
                    itemListings.Clear();


                    Inventory towerInv = selectedTower.inv;
                    for (int i = 0; i < towerInv.items.Count; i++)
                    {
                        TowerInventoryItem temp = Instantiate(prefab, Vector3.zero, Quaternion.identity, contentWindow);
                        temp.transform.localPosition = new Vector2(Xoffset, Yoffset + YOffsetMultiplier * i);
                        temp.INIT(towerInv.items[i], towerInv.quantity[i]);
                        itemListings.Add(temp);
                    }
                }
                else
                {
                    Inventory towerInv = selectedTower.inv;
                    for (int i = 0; i < towerInv.items.Count; i++)
                    {
                        itemListings[i].INIT(towerInv.items[i], towerInv.quantity[i]);
                    }
                }
            }
            else
            {
                scrollView.SetActive(false);
                EmptyPanel.SetActive(true);
            }
        }
    }

    public void RemoveButtonClicked()
    {
        Destroy(selectedTower.gameObject);
    }
}
