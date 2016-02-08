using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour 
{
    public GameParameters.Tiles tile;
    public int subIndex = 0;
    public MeshRenderer tileRenderer;
    public MeshRenderer blockRenderer;

    public AnimationCurve popOutCurve;

    [HideInInspector] public List<Tile> leftTiles = new List<Tile>();
    [HideInInspector] public List<Tile> rightTiles = new List<Tile>();
    [HideInInspector] public List<Tile> topTiles = new List<Tile>();
    [HideInInspector] public List<Tile> bottomTiles = new List<Tile>();

    AudioSource _audioSource;
    public AudioSource AudioSource
    {
        get
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = GameParameters.Instance.tick;
            return _audioSource;
        }
    }

    public bool canSelect = true;
    bool selectable = false;
    public enum TransitionType
    {
        None,
        Drop,
        Pop,
    };
    public TransitionType transitionType = TransitionType.Drop;

	void Start()
    {
        tileRenderer.material.SetTexture("_MainTex", GameParameters.Instance.GetGroupTexture(tile, subIndex));
	    switch (transitionType)
        {
            case TransitionType.Drop:
                StartCoroutine(MoveIn());
                break;
            case TransitionType.Pop:
                StartCoroutine(MoveInPop());
                break;
	    }
	}

    IEnumerator MoveIn()
    {
        selectable = false;

        var initPos = transform.position;
        var speed = 10.0f + Random.value;
        transform.position += new Vector3(0, ((transform.position.y + 1.0f) * 2.0f) + 10 + Random.value, 0);

        while(transform.position.y > initPos.y)
        {
            transform.position -= new Vector3(0, Time.deltaTime * speed, 0);
            yield return null;
        }
        transform.position = initPos;

        selectable = true;
        var obj = Instantiate(GameParameters.Instance.dustPoof);
        obj.transform.position = transform.position + new Vector3(0,0.5f,0);
    }

    IEnumerator MoveInPop()
    {
        selectable = false;
        var speed = 2 + Random.value * 5;
        float time = 0;
        transform.localScale = Vector3.zero;
        while (transform.localScale.x < 1.0f)
        {
            time += Time.deltaTime * speed;
            transform.localScale = Vector3.one * (1.0f - popOutCurve.Evaluate(time));
            yield return null;
        }
        transform.localScale = Vector3.one;
        selectable = true;
    }

    public void Destroy(bool thisOne = false)
    {
        StartCoroutine(MoveOut(thisOne));
    }

    public IEnumerator MoveOut(bool thisOne)
    {
        Level.Instance.RemoveTile(this);

        var speed = 5.0f - (thisOne ? 0.1f : 0.0f);
        float time = 0;
        while (transform.localScale.x > 0.01f)
        {
            time += Time.deltaTime * speed;
            transform.localScale = Vector3.one * popOutCurve.Evaluate(time);
            yield return null;
        }

        GameObject.Destroy(gameObject);

        if (thisOne)
        {
            int pair = Level.Instance.PairCount();
            UIManager.Instance.inGameMenu.UpdateText(pair);
            if (pair == 0)
            {
                if (Level.Instance.TileCount() == 0)
                {
                    UIManager.Instance.DisplayWin();
                }
                else
                {
                    UIManager.Instance.DisplayLoose();
                }
            }
        }
    }

    void OnMouseUpAsButton()
    {
        if (Level.Instance.CanSelect(this) == false || selectable == false || canSelect == false)
        {
            return;
        }

        if (Level.Instance.selectedTile == null)
        {
            Level.Instance.selectedTile = this;
            blockRenderer.material.color = GameParameters.Instance.selectionColour;
            AudioSource.Play();
        }
        else
        {
            Level.Instance.selectedTile.blockRenderer.material.color = Color.white;
            if (Level.Instance.selectedTile.tile == tile)
            {
                if (Level.Instance.selectedTile != this)
                {
                    LevelManager.Instance.Record(Level.Instance.selectedTile,this);
                    Level.Instance.selectedTile.Destroy();
                    this.Destroy(true);
                    AudioSource.pitch += 0.1f;
                    AudioSource.Play();
                }
                Level.Instance.selectedTile = null;
            }
            else
            {
                Level.Instance.selectedTile = this;
                blockRenderer.material.color = GameParameters.Instance.selectionColour;
                AudioSource.Play();
            }
        }
    }

    public void Pulse()
    {
        StartCoroutine(PulseRoutine());
    }

    IEnumerator PulseRoutine()
    {
        var colour = blockRenderer.material.color;
        float t = 0.0f;
        float speed = 2.0f;
        while (t < 2.0f)
        {
            blockRenderer.material.color = Color.Lerp(colour, Color.Lerp(GameParameters.Instance.selectionColour, colour, Mathf.Max(0, t - 1)), t);
            t += Time.deltaTime * speed;
            yield return null;
        }
        blockRenderer.material.color = colour;
    }

}
