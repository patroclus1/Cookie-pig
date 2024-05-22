using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private PlayerActions player;
    [SerializeField] private FootstepCollection[] terrainFootstepCollections;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup playerMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup environmentMixerGroup;
    [SerializeField] private AudioClip cookieCollectedAudio;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource _backgroundMusic;
    private AudioSource _cookieAudio;

    private AudioSource _purchaseSFX;
    private AudioSource _notEnoughCookiesSFX;

    private AudioSource _closingSFX;

    private AudioSource _gameOverSFX;

    private string currentLayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        _backgroundMusic = gameObject.AddComponent<AudioSource>();
        _cookieAudio = gameObject.AddComponent<AudioSource>();
        _purchaseSFX = gameObject.AddComponent<AudioSource>();
        _notEnoughCookiesSFX = gameObject.AddComponent<AudioSource>();
        _closingSFX = gameObject.AddComponent<AudioSource>();
        _gameOverSFX = gameObject.AddComponent<AudioSource>();


        _backgroundMusic.clip = backgroundMusic;
        _cookieAudio.clip = cookieCollectedAudio;
        _purchaseSFX.clip = buySound;
        _notEnoughCookiesSFX.clip = errorSound;
        _closingSFX.clip = closeSound;
        _gameOverSFX.clip = gameOverSound;

        _purchaseSFX.outputAudioMixerGroup = environmentMixerGroup;
        _purchaseSFX.loop = false;
        _purchaseSFX.volume = 0.3f;
        _purchaseSFX.playOnAwake = false;

        _notEnoughCookiesSFX.outputAudioMixerGroup = environmentMixerGroup;
        _notEnoughCookiesSFX.loop = false;
        _notEnoughCookiesSFX.volume = 0.25f;
        _notEnoughCookiesSFX.playOnAwake = false;

        _closingSFX.outputAudioMixerGroup = environmentMixerGroup;
        _closingSFX.loop = false;
        _closingSFX.volume = 0.25f;
        _closingSFX.playOnAwake = false;

        _backgroundMusic.outputAudioMixerGroup = musicMixerGroup;
        _backgroundMusic.loop = true;
        _backgroundMusic.volume = 1.0f;
        _backgroundMusic.Play();

        _cookieAudio.outputAudioMixerGroup = environmentMixerGroup;
        _cookieAudio.playOnAwake = false;
        _cookieAudio.volume = 0.1f;
        _cookieAudio.loop = false;

        _gameOverSFX.outputAudioMixerGroup = musicMixerGroup;
        _gameOverSFX.loop = false;
        _gameOverSFX.playOnAwake = false;
        _gameOverSFX.volume = 0.4f;
    }

    private void OnEnable()
    {
        Cookie.OnCookieCollect += _ => PlayCookieSound();
    }

    private void OnDisable()
    {
        Cookie.OnCookieCollect -= _ => PlayCookieSound();
    }

    public void CheckLayers(Vector3 playerPosition, LayerMask groundLayer)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, 3, groundLayer))
        {
            if (hit.transform.TryGetComponent(out Terrain terrain))
            {
                if (currentLayer != GetLayerName(playerPosition, terrain))
                {
                    currentLayer = GetLayerName(playerPosition, terrain);

                    foreach (FootstepCollection collection in terrainFootstepCollections)
                    {
                        if (currentLayer == collection.name)
                        {
                            player.SwapStepSound(collection);
                        }
                    }
                }
            }
            if (hit.transform.TryGetComponent(out SurfaceType surfaceType))
            {
                FootstepCollection collection = surfaceType.FootstepCollection;
                currentLayer = collection.name;
                player.SwapStepSound(collection);
            }
        }
    }

    private float[] GetTextureMix(Vector3 position, Terrain terrain)
    {
        Vector3 terrainPos = terrain.transform.position;
        TerrainData terrainData = terrain.terrainData;
        int mapX = Mathf.RoundToInt((position.x - terrainPos.x) / terrainData.size.x * terrainData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((position.z - terrainPos.z) / terrainData.size.z * terrainData.alphamapHeight);

        float[,,] splatMapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        float[] cellMix = new float[splatMapData.GetUpperBound(2) + 1];

        for (int i = 0; i < cellMix.Length; i++)
        {
            cellMix[i] = splatMapData[0, 0, i];
        }
        return cellMix;
    }

    private string GetLayerName(Vector3 position, Terrain terrain)
    {
        float[] cellMix = GetTextureMix(position, terrain);
        float strongest = 0;
        int maxIndex = 0;

        for (int i = 0; i < cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }
        return terrain.terrainData.terrainLayers[maxIndex].name;
    }

    private void PlayCookieSound()
    {
        _cookieAudio.PlayOneShot(_cookieAudio.clip);
    }

    public void PlayErrorSound()
    {
        _notEnoughCookiesSFX.PlayOneShot(_notEnoughCookiesSFX.clip);
    }

    public void PlaySuccessSound()
    {
        _purchaseSFX.PlayOneShot(_purchaseSFX.clip);
    }

    public void PlayClickSound()
    {
        _closingSFX.PlayOneShot(_closingSFX.clip);
    }

    public void PlayVictorySound()
    {
        _backgroundMusic.Stop();
        _gameOverSFX.Play();
    }
}
