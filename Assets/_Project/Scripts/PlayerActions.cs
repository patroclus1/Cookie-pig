using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(CharacterController))]
public class PlayerActions : MonoBehaviour
{
    private CharacterController _controller;
    private AudioSource _audioSource;
    private Transform _transform;
    private Vector3 _spawnPosition;
    private Vector3 _velocity;
    private Vector3 _input;
    private Vector3 _moveDirection;
    private Vector3 _lineEnd;
    [SerializeField] private float _groundDistance = 0.1f;
    private bool _wasGroundedLastFrame = false;
    private bool _isPlayerDead = false;
    private bool _isCheckpointSet = false;
    private bool _isDoublejumpAvailable = false;
    private bool _isPlatformInHand = false;
    private bool _isPlatformPlaced = false;
    private float _currentMoveSpeed;
    private float _stepCycle;
    private float _nextStep;
    [SerializeField] private float footstepSoundInterval;
    [SerializeField] private float defaultMoveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityForce = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private SpawnablePlatform platform;
    [SerializeField] private LayerMask groundMask;


    [SerializeField] private List<AudioClip> footstepSounds = new List<AudioClip>();
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landsound;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _transform = GetComponent<Transform>();

        _spawnPosition = _transform.position;
        _currentMoveSpeed = defaultMoveSpeed;

        _stepCycle = 0f;
        _nextStep = 0f;
    }

    private void Update()
    {
        if (_isPlayerDead) return;
        ApplyGravity();
        MovePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            StartCoroutine(DeathAndReset());
        }
    }

    private IEnumerator DeathAndReset()
    {
        _controller.enabled = false;
        _isPlayerDead = true;
        yield return new WaitForSeconds(2f);
        _transform.position = _spawnPosition;
        _controller.enabled = true;
        _isPlayerDead = false;
    }

    private bool IsGrounded()
    {
        _lineEnd = new Vector3(groundCheck.position.x, groundCheck.position.y - _groundDistance, groundCheck.position.z);
        return Physics.CheckCapsule(groundCheck.position, _lineEnd, _groundDistance/2, groundMask);
    }

    public void ReceiveMoveInput(Vector3 input)
    {
        _input = input;
    }

    private void MovePlayer()
    {
        _moveDirection = _transform.right * _input.x + transform.forward * _input.y;
        _controller.Move(_currentMoveSpeed * Time.deltaTime * _moveDirection.normalized);

        ProgressStepCycle();
    }

    private void ApplyGravity()
    {
        _velocity.y += gravityForce * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        if (!_wasGroundedLastFrame && IsGrounded())
        {
            PlayLandingSound();
        }

        if (IsGrounded() && _velocity.y < 0)
        {
            _isDoublejumpAvailable = GameManager.Instance.isDoublejumpPurchased;
            _velocity.y = -2f;
        }

        _wasGroundedLastFrame = IsGrounded();
    }

    private void ProgressStepCycle()
    {
        if (_controller.velocity.sqrMagnitude > 0 && _input.x != 0  || _input.y != 0)
        {
            _stepCycle += (_controller.velocity.magnitude + _currentMoveSpeed) * Time.deltaTime;
        }

        if (!(_stepCycle > _nextStep)) return;

        _nextStep = _stepCycle + footstepSoundInterval;

        PlayFootstepsAudio();
    }

    private void PlayFootstepsAudio()
    {
        if (!IsGrounded()) return;
        AudioManager.Instance.CheckLayers(_transform.position, groundMask);
        int n = Random.Range(1, footstepSounds.Count);
        _audioSource.clip = footstepSounds[n];
        _audioSource.PlayOneShot(_audioSource.clip);
        // move picked sound to start so it`s not picked next time
        footstepSounds[n] = footstepSounds[0];
        footstepSounds[0] = _audioSource.clip;
    }

    private void PlayJumpSound()
    {
        AudioManager.Instance.CheckLayers(_transform.position, groundMask);
        _audioSource.clip = jumpSound;
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    private void PlayLandingSound()
    {
        AudioManager.Instance.CheckLayers(_transform.position, groundMask);

        _audioSource.clip = landsound;
        _audioSource.PlayOneShot(_audioSource.clip);
        _nextStep = _stepCycle + 0.5f;
    }

    public void SwapStepSound(FootstepCollection collection)
    {
        footstepSounds.Clear();
        for (int i = 0; i < collection.footstepSounds.Count; i++)
        {
            footstepSounds.Add(collection.footstepSounds[i]);
        }
        jumpSound = collection.jumpSound;
        landsound = collection.landsound;
    }

    public void PerformJump()
    {
        if (!IsGrounded() && !GameManager.Instance.isDoublejumpPurchased) return;
        if (IsGrounded())
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravityForce);
            PlayJumpSound();
        }
        else if (!IsGrounded() && _isDoublejumpAvailable)
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravityForce);
            _isDoublejumpAvailable = false;
        }
    }

    public void ProbeCheckpoint(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.isCheckpointPurchased) return;

        if (_isCheckpointSet && ctx.interaction is PressInteraction)
        {
            _controller.enabled = false;
            _transform.position = checkPoint.position;
            _controller.enabled = true;
        }
        else if (!_isCheckpointSet && ctx.interaction is PressInteraction)
        {
            _isCheckpointSet = true;
            checkPoint.SetParent(_transform.parent);
            checkPoint.position = _transform.position;
            checkPoint.gameObject.SetActive(true);
        }
        else if (_isCheckpointSet && ctx.interaction is HoldInteraction)
        {
            _isCheckpointSet = false;
            checkPoint.SetParent(_transform);
            checkPoint.position = _transform.position;
            checkPoint.gameObject.SetActive(false);
        }
    }

    public void EnableSprint()
    {
        if (GameManager.Instance.isSprintPurchased)
        {
            _currentMoveSpeed = sprintSpeed;
        }
    }

    public void DisableSprint()
    {
        _currentMoveSpeed = defaultMoveSpeed;
    }

    public void ProbePlatform(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Instance.isPlatformPurchased) return;

        if (!_isPlatformPlaced && ctx.interaction is PressInteraction)
        {
            _isPlatformInHand = !_isPlatformInHand;
            platform.gameObject.SetActive(_isPlatformInHand);
        }
        else if (_isPlatformPlaced && ctx.interaction is HoldInteraction)
        {
            platform.Return();
            _isPlatformPlaced = false;
            _isPlatformInHand = true;
        }
    }

    public void ShootPlatform()
    {
        if (_isPlatformInHand)
        {
            _isPlatformInHand = false;
            platform.Launch();
            _isPlatformPlaced = true;
        }
    }

    private void OnDrawGizmos()
    {
        var lineEnd = new Vector3(groundCheck.position.x, groundCheck.position.y - _groundDistance, groundCheck.position.z);
        Gizmos.DrawLine(groundCheck.position, lineEnd);
    }
}
