using System.Collections;
using UnityEngine;

public class RoadSegment : MonoBehaviour
{
    public System.Action OnDestroyCallback;

    private GameObject[] _tiles;
    private float _fadeOutDuration = 1f;
    private float _fallSpeed = 6f;

    public void SetupRoadSegment(GameObject tilePrefab, Vector3 spawnPoint, int roadWidth)
    {
        var triggerCollider = gameObject.AddComponent<BoxCollider>();
        var tileWidth = tilePrefab.transform.localScale.x;
        var tilesCount = roadWidth * roadWidth;

        _tiles = new GameObject[tilesCount];
        transform.position = spawnPoint;
        triggerCollider.size = new Vector3(roadWidth * tileWidth, tileWidth, roadWidth * tileWidth);
        triggerCollider.center = new Vector3(0, tileWidth, 0);
        triggerCollider.isTrigger = true;

        for (var i = 0; i < tilesCount; i++)
        {
            _tiles[i] = Instantiate(tilePrefab, transform);
            _tiles[i].transform.localPosition = CalculateNewTilePos(roadWidth, tileWidth, i);
        }
    }

    private Vector3 CalculateNewTilePos(int roadWidth, float tileWidth, int index)
    {
        var x = -roadWidth + (tileWidth / 2 + index % roadWidth * tileWidth);
        var y = 0f;
        var z = -roadWidth + (tileWidth / 2 + index / roadWidth * tileWidth);

        return new Vector3(x, y, z);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartCoroutine(AnimateFading());
    }

    private void OnDestroy()
    {
        OnDestroyCallback?.Invoke();
    }

    private IEnumerator AnimateFading()
    {
        var materials = new Material[_tiles.Length];
        var timePassed = 0f;

        for (var i = 0; i < _tiles.Length; i++)
            materials[i] = _tiles[i].GetComponent<MeshRenderer>().material;

        while (timePassed < _fadeOutDuration)
        {
            var alphaStep = Mathf.Lerp(1f, 0f, timePassed / _fadeOutDuration);

            foreach (var mat in materials)
            {
                var newColor = mat.color;

                newColor.a = alphaStep;
                mat.color = newColor;
            }

            timePassed += Time.deltaTime;
            transform.Translate(-Vector3.up * _fallSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
