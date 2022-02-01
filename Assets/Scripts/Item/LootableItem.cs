using UnityEngine;
using System.Collections;
using TMPro;
using Lowscope.Saving;

/// <summary>
/// As the name of the component states, it is meant for items that you can loot with the player.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class LootableItem : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public class References
    {
        public Rigidbody2D rigidBody2D;
        public SpriteRenderer spriteRenderer;
        public TextMeshPro amountText;
        public BoxCollider2D boxCollider2D;
    }

    [System.Serializable]
    public class Configuration
    {
        public ItemData data;
        public int amount;
        public float moveSpeed = 2;
        public float activationWait = 1;
    }

    [SerializeField]
    private References references;

    [SerializeField]
    private Configuration configuration;

    private Coroutine moveCoroutine;

    private GameObject player;
    private bool isLooted;

    private void OnValidate()
    {
        if (configuration.data != null)
        {
            references.spriteRenderer.sprite = configuration.data.Icon;
        }
    }

    public float PickupDistance()
    {
        return references.boxCollider2D.size.magnitude * 0.5f;
    }

    public void Configure(ItemData data, int amount)
    {
        configuration.data = data;
        configuration.amount = amount;
        isLooted = false;
        Refresh();
    }

    private void OnEnable()
    {
        if (configuration.data != null)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        if (configuration.data != null)
        {
            references.spriteRenderer.sprite = configuration.data.Icon;

            if (configuration.amount > 1 && configuration.data.CanStack)
            {
                references.amountText.text = configuration.amount.ToString();
                references.amountText.gameObject.SetActive(true);
            }
            else
            {
                references.amountText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLooted && collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            Invoke("MoveToPlayer", configuration.activationWait);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isLooted && collision.CompareTag("Player"))
        {
            player = null;
            CancelInvoke("MoveToPlayer");
        }
    }

    private void MoveToPlayer()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveToPlayer(player));
    }

    IEnumerator MoveToPlayer(GameObject gameObject)
    {
        while (Vector3.Distance(this.transform.position, gameObject.transform.position) > 0.05f)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, gameObject.transform.position, Time.deltaTime * configuration.moveSpeed);
            yield return null;
        }

        if (configuration.data != null)
        {
            Inventory getInventory = gameObject.GetComponent<Inventory>();

            getInventory.AddItem(configuration.data, configuration.amount);
        }

        this.gameObject.SetActive(false);
    }

    [System.Serializable]
    private struct SaveData
    {
        public string data;
        public int amount;
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData()
        {
            data = configuration.data.GetGuid(),
            amount = configuration.amount
        });
    }

    public void OnLoad(string data)
    {
        SaveData getData = JsonUtility.FromJson<SaveData>(data);
        Configure(ScriptableAssetDatabase.GetAsset(getData.data) as ItemData, getData.amount);
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}
