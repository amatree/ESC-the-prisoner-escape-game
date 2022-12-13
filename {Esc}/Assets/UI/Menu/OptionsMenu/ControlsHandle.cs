using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControlsHandle : MonoBehaviour
{
    [SerializeField] public GameConfiguration gameConfiguration;
    [SerializeField] [ReadOnly] public GameSettingsManager gameDataManager;
	public PlayerController playerController;
	public TMP_InputField m_slowWalkKeyInput;
	public TMP_InputField m_sprintKeyInput;
	public TMP_InputField m_jumpKeyInput;
	
	[Header("Current Data")]
	[ReadOnly] public KeyCode c_slowWalkKeyInput;
	[ReadOnly] public KeyCode c_sprintKeyInput;
	[ReadOnly] public KeyCode c_jumpKeyInput;

	[Header("Previous Data")]
	[ReadOnly] public bool onOptionsGUI = false;
	[ReadOnly] public KeyCode p_slowWalkKeyInput;
	[ReadOnly] public KeyCode p_sprintKeyInput;
	[ReadOnly] public KeyCode p_jumpKeyInput;


	Event @event;
	
    // Start is called before the first frame update
    void Start()
    {
		if (gameConfiguration is null)
			gameConfiguration = GameObject.FindGameObjectWithTag("GameConfiguration").GetComponent<GameConfiguration>();
		if (playerController is null)
			playerController = GameObject.FindObjectOfType<PlayerController>();

        gameDataManager = gameConfiguration.gameDataManager;
		playerController.slowWalkKey = gameDataManager.slowWalkKey;
		playerController.sprintKey = gameDataManager.sprintKey;
		playerController.jumpKey = gameDataManager.jumpKey;

		m_slowWalkKeyInput.text = gameDataManager.slowWalkKey.ToString();
		m_sprintKeyInput.text = gameDataManager.sprintKey.ToString();
		m_jumpKeyInput.text = gameDataManager.jumpKey.ToString();
    }

    // Update is called once per frame
    void Update()
    {
		if (m_slowWalkKeyInput.isFocused || m_sprintKeyInput.isFocused || m_jumpKeyInput.isFocused)
			WaitForKeyInput();
    }

	void OnGUI()
	{
		@event = Event.current;
		if (!onOptionsGUI)
		{
			onOptionsGUI = true;
			p_slowWalkKeyInput = gameDataManager.slowWalkKey;
			p_sprintKeyInput = gameDataManager.sprintKey;
			p_jumpKeyInput = gameDataManager.jumpKey;
		}
	}

	public void Revert()
	{
		gameDataManager.slowWalkKey = p_slowWalkKeyInput;
		gameDataManager.sprintKey = p_sprintKeyInput;
		gameDataManager.jumpKey = p_jumpKeyInput;
		onOptionsGUI = false;
	}

	public void Save()
	{
		gameDataManager.slowWalkKey = c_slowWalkKeyInput;
		gameDataManager.sprintKey = c_sprintKeyInput;
		gameDataManager.jumpKey = c_jumpKeyInput;
        gameDataManager.Save();
		onOptionsGUI = false;
	}

	void WaitForKeyInput()
	{
		if (@event.isKey)
		{
			if (@event.type == EventType.KeyUp && @event.keyCode is not KeyCode.None)
				if (m_slowWalkKeyInput.isFocused)
				{
					c_slowWalkKeyInput = @event.keyCode;
					m_slowWalkKeyInput.text = "";
					m_slowWalkKeyInput.text = @event.keyCode.ToString();
				}
				else if (m_sprintKeyInput.isFocused)
				{
					c_sprintKeyInput = @event.keyCode;
					m_sprintKeyInput.text = "";
					m_sprintKeyInput.text = @event.keyCode.ToString();
				}
				else if (m_jumpKeyInput.isFocused)
				{
					c_jumpKeyInput = @event.keyCode;
					m_jumpKeyInput.text = "";
					m_jumpKeyInput.text = @event.keyCode.ToString();
				}
		}
	}
}
