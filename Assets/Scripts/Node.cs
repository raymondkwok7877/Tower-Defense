﻿using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    private Color startColor;
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    private bool rangeDrawn = false;
    public Vector3 positionOffset;

    private Renderer rend;

    [HideInInspector]
    public GameObject turret;
    [HideInInspector]
    public TurretBlueprint turretBlueprint;
    [HideInInspector]
    public bool isUpgraded = false;

    BuildManager buildManager;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    } 

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
       
        if (turret !=null)
        {
            buildManager.SelectNode(this);
            return;
        }

        if (!buildManager.CanBuild)
            return;

        BuildTurret(buildManager.GetTurretToBuild());
    }

    void BuildTurret(TurretBlueprint blueprint)
    {
        if (PlayerStats.Money < blueprint.cost)
        {
            Debug.Log("You broke boi");
            return;
        }

        PlayerStats.Money -= blueprint.cost;

        GameObject theTurret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
        turret = theTurret;

        turretBlueprint = blueprint;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret build!");
    }

    public void UpgradeTurrent()
    {
        if (PlayerStats.Money < turretBlueprint.upgradeCost)
        {
            Debug.Log("You broke boi. Cant upgrade");
            return;
        }

        PlayerStats.Money -= turretBlueprint.upgradeCost;

        //destroy old turret
        Destroy(turret);

        //create new turret
        GameObject theTurret = (GameObject)Instantiate(turretBlueprint.upgradedPrefab, GetBuildPosition(), Quaternion.identity);
        turret = theTurret;

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        isUpgraded = true;

        Debug.Log("Turret upgraded!");
    }

    public void SellTurret()
    {
        PlayerStats.Money += turretBlueprint.GetSellAmount();

        GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Destroy(turret);
        turretBlueprint = null;
    }

    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (!buildManager.CanBuild)
            return;
        if (!rangeDrawn)
        {
            buildManager.DrawTurretRange(this); rangeDrawn = true;
        }
        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }
    void OnMouseExit()
    {
        rend.material.color = startColor;
        buildManager.DestroyRange();
        rangeDrawn = false;
    }
}