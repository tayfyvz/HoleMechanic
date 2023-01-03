using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoleMovement : MonoBehaviour
{

    [Header("Hole mesh")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;


    [Header("Hole vertices radius")]
    public Vector2 moveLimits;
    public float radius;
    public Transform holeCenter;

    Mesh mesh;
    List<int> holeVertices;
    List<Vector3> offSets;
    int holeVerticesCount;

    float x, y;
    Vector3 touch, targetPos;
    public float moveSpeed;

    public float scaleMultiplier;

    public static HoleMovement Instance;

    public GameObject upgradePanel;
    public GameObject fullHole;

    public Text moneyTxt;
    int money = 0;
    public Text radiusCostTxt;
    public Text capacityCostTxt;
    public Text speedCostTxt;

    int radiusCost = 40;
    int capacityCost = 40;
    int speedCost = 40;

    int silverCountt, crystalCountt, goldCountt = 0;

    public GameObject winPanel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        GameManager.Instance.isGameOver = false;
        GameManager.Instance.isMoving = false;

        holeVertices = new List<int>();
        offSets = new List<Vector3>();

        mesh = meshFilter.mesh;

        //find hole vertices in mesh
        FindHoleVertices();

        SetHoleScale(0.5f);

        radiusCostTxt.text = "$ " + radiusCost;
        capacityCostTxt.text = "$ " + capacityCost;
        speedCostTxt.text = "$ " + speedCost;
        moneyTxt.text = "$ 0";
    }

    void FindHoleVertices()
    {
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            float distance = Vector3.Distance(holeCenter.position, mesh.vertices[i]);

            if(distance < radius)
            {
                holeVertices.Add(i);
                offSets.Add(mesh.vertices[i] - holeCenter.position);
            }

        }

        holeVerticesCount = holeVertices.Count;
    }

    private void Update()
    {
        
        GameManager.Instance.isMoving = Input.GetMouseButton(0);

        if(!GameManager.Instance.isGameOver && GameManager.Instance.isMoving)
        {
            MoveHole();

            UpdateHoleVerticesPosition();

        }

        if(FallCollider.Instance.collectiblesCount == FallCollider.Instance.collectiblesMaxCount)
        {
            HoleIsFull(FallCollider.Instance.silverCount, FallCollider.Instance.goldCount, FallCollider.Instance.crystalCount);
        }


        if(FallCollider.Instance.progress.value == 25)
        {
            winPanel.SetActive(true);
        }

    }


    void MoveHole()
   {
        x = Input.GetAxis("Mouse X");
        y = Input.GetAxis("Mouse Y");

        //lerp (smooth) movement
        touch = Vector3.Lerp(holeCenter.position, holeCenter.position + new Vector3(-y, 0f, x), moveSpeed * Time.deltaTime);

        targetPos = new Vector3(Mathf.Clamp(touch.x, -moveLimits.x, moveLimits.x),touch.y,Mathf.Clamp(touch.z, -moveLimits.y, moveLimits.y));

        holeCenter.position = targetPos;
    }

    public void SetHoleScale(float multiplier) 
    {
        /*
         * 
         *FIRST VERSION OF SET HOLE SCALE FUNCTION
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < holeVerticesCount; i++)
        {
            vertices[holeVertices[i]] = holeCenter.position + offSets[i] * multiplier;

        }

        //update mesh
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        holeCenter.localScale *= multiplier;

        for (int i = 0; i < offSets.Count; i++)
        {
            offSets[i] *= multiplier;
        }

        moveLimits /= 1.3f; // set up correctly
        
        */
        IEnumerator c = SetHoleScaleCoroutine(multiplier);
        StartCoroutine(c);

    }

    IEnumerator SetHoleScaleCoroutine(float multiplier)
    {
        //if > offsets, then dont scale

        Vector3[] vertices = mesh.vertices;
        Vector3 holeCenterLocalScaleEnd = holeCenter.localScale * multiplier;

        List<Vector3> offSetsEnd = new List<Vector3>();

        for (int i = 0; i < offSets.Count; i++)
        {
            offSetsEnd.Add(offSets[i] * multiplier);
        }

        while (vertices[holeVertices[0]] != holeCenter.position + offSets[0] * multiplier)
        {
            for (int i = 0; i < holeVerticesCount; i++)
            {
                vertices[holeVertices[i]] = Vector3.Lerp(vertices[holeVertices[i]], holeCenter.position + offSetsEnd[i], 0.5f);


            }

            //update mesh
            mesh.vertices = vertices;
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            //****************

            holeCenter.localScale = Vector3.Lerp(holeCenter.localScale, holeCenterLocalScaleEnd, 0.4f);


            yield return null;
        }

        for (int i = 0; i < offSets.Count; i++)
        {
            offSets[i] *= multiplier;
        }
        

        //moveLimits /= 1.3f; // set up correctly
    }

    void UpdateHoleVerticesPosition()
    {
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < holeVerticesCount; i++)
        {
            vertices[holeVertices[i]] = holeCenter.position + offSets[i];

        }

        //update mesh
        mesh.vertices = vertices;
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(holeCenter.position, radius);
    }


    public void UpdateRadius()
    {
        if(money >= radiusCost)
        {
            SetHoleScale(1.5f);
            money -= radiusCost;

            radiusCost *= 2;
            radiusCostTxt.text = "$ " + radiusCost;
            moneyTxt.text = "$ " + money;

        }

    }

    public void UpdateSpeed()
    {
        if(money >= speedCost)
        {
            moveSpeed += 1;
            money -= speedCost;

            speedCost *= 2;
            speedCostTxt.text = "$ " + speedCost;
            moneyTxt.text = "$ " + money;
        }

    }


    public void UpdateCapacity()
    {
        if(money >= capacityCost)
        {
            FallCollider.Instance.collectiblesMaxCount += 3;
            money -= capacityCost;

            capacityCost *= 2;
            capacityCostTxt.text = "$ " + capacityCost;
            moneyTxt.text = "$ " + money;

        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "upgrade")
        {
            upgradePanel.SetActive(true);

        }else if(other.tag == "empty")
        {
            MakeHoleEmpty();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "upgrade")
        {
            upgradePanel.SetActive(false);

        }
    }

    void HoleIsFull(int silverCount, int crystalCount, int goldCount)
    {
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("gold"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = true;
        }
        
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("silver"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = true;
        }
        
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("crystal"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = true;
        }

        silverCountt = silverCount;
        goldCountt = goldCount;
        crystalCountt = crystalCount;



        fullHole.SetActive(true);
    }

    void MakeHoleEmpty()
    {
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("gold"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = false;
        }
        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("silver"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = false;
        }

        foreach (GameObject collectible in GameObject.FindGameObjectsWithTag("crystal"))
        {
            collectible.GetComponent<Rigidbody>().isKinematic = false;
        }

        silverCountt = FallCollider.Instance.silverCount;
        goldCountt = FallCollider.Instance.goldCount;
        crystalCountt = FallCollider.Instance.crystalCount;

        MakeMoney();

        silverCountt = 0;
        goldCountt = 0;
        crystalCountt = 0;

        FallCollider.Instance.silverCount = 0;
        FallCollider.Instance.goldCount = 0;
        FallCollider.Instance.crystalCount = 0;
        FallCollider.Instance.collectiblesCount = 0;

        fullHole.SetActive(false);

    }

    void MakeMoney()
    {
        int totalMoney = money + silverCountt * 20 + goldCountt * 70 + crystalCountt * 60;
        money = totalMoney;
        moneyTxt.text = "$ " + totalMoney;
    }
}
