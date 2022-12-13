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

	List<KeyCode> modifierKeys;
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

		c_slowWalkKeyInput = gameDataManager.slowWalkKey;
		c_sprintKeyInput = gameDataManager.sprintKey;
		c_jumpKeyInput = gameDataManager.jumpKey;

		modifierKeys = new List<KeyCode>(){
			KeyCode.LeftAlt, KeyCode.RightAlt,
			KeyCode.LeftControl, KeyCode.RightControl,
			KeyCode.LeftShift, KeyCode.RightShift,
		};
    }

    // Update is called once per frame
    void Update()
    {
		if (m_slowWalkKeyInput.isFocused || m_sprintKeyInput.isFocused || m_jumpKeyInput.isFocused)
		{
			WaitForKeyInput();
		}
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

	void GetModifierKeys()
	{
		foreach (KeyCode __k in modifierKeys)
		{
			if (Input.GetKeyUp(__k))
			{
				RegisterKey(__k);
			}
		}
	}

	void WaitForKeyInput()
	{
		if (@event.isKey)
		{
			if (@event.type == EventType.KeyUp && @event.keyCode is not KeyCode.None)
			{
				RegisterKey(@event.keyCode);
			}
		} else if (@event.alt || @event.shift || @event.control)
		{
			GetModifierKeys();
		}
	}

	void RegisterKey(KeyCode keyCode)
	{
		if (m_slowWalkKeyInput.isFocused)
		{
			c_slowWalkKeyInput = keyCode;
			m_slowWalkKeyInput.text = "";
			m_slowWalkKeyInput.text = keyCode.ToString();
		}
		else if (m_sprintKeyInput.isFocused)
		{
			c_sprintKeyInput = keyCode;
			m_sprintKeyInput.text = "";
			m_sprintKeyInput.text = keyCode.ToString();
		}
		else if (m_jumpKeyInput.isFocused)
		{
			c_jumpKeyInput = keyCode;
			m_jumpKeyInput.text = "";
			m_jumpKeyInput.text = keyCode.ToString();
		}
	}
}
