using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MIMA_RadicalCaptureUI : MonoBehaviour
{
    public UIDocument uiDocument;

    public Action<bool> OnToggleSpoutOutput;
    public Action<bool, int, int> OnToggleOSCSend;
    public Action<string, string> OnStartRadical;
    public Action OnStopRadical;

    private Toggle toggle_spoutOutput;
    private Toggle toggle_oscSend;
    private TextField field_oscPortIn;
    private TextField field_oscPortOut;
    private TextField field_radicalRoomName;
    private TextField field_radicalURL;
    private Label label_oscStatus;
    private Label label_radicalStatus;
    private Button button_toggleRadical;

    private bool radicalStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        var root = uiDocument.rootVisualElement;

        toggle_spoutOutput = root.Q<Toggle>("ToggleSpout");
        toggle_oscSend = root.Q<Toggle>("ToggleOSCSend");
        field_oscPortIn = root.Q<TextField>("Field_OSCPortIn");
        field_oscPortOut = root.Q<TextField>("Field_OSCPortOut");
        field_radicalRoomName = root.Q<TextField>("Field_RadicalRoom");
        field_radicalURL = root.Q<TextField>("Field_RadicalSocketURL");
        label_oscStatus = root.Q<Label>("LabelOSCStatus");
        label_radicalStatus = root.Q<Label>("LabelRadicalStatus");
        button_toggleRadical = root.Q<Button>("ButtonToggleRadical");
        

        toggle_oscSend.RegisterValueChangedCallback(evt =>
        {
            if (toggle_oscSend.value)
            {
                if (OnToggleOSCSend != null)
                {
                    int portIn = int.Parse(field_oscPortIn.value);
                    int portOut= int.Parse(field_oscPortOut.value);
                    OnToggleOSCSend.Invoke(true, portIn, portOut);
                }
            }
            else
            {
                if (OnToggleOSCSend != null)
                {
                    OnToggleOSCSend.Invoke(false, -1, -1);
                }
            }
        });

        toggle_spoutOutput.RegisterValueChangedCallback(evt =>
        {
            if (OnToggleSpoutOutput != null)
            {
                OnToggleSpoutOutput.Invoke(toggle_spoutOutput.value);
            }
        });

        button_toggleRadical.clicked += () =>
        {
            if (radicalStarted)
            {
                button_toggleRadical.text = "Connect To Radical";
                if (OnStopRadical != null) OnStopRadical.Invoke();
            }
            else
            {
                button_toggleRadical.text = "Disconnect From Radical";
                string radicalRoom = field_radicalRoomName.value;
                string radicalURL = field_radicalURL.value;
                if (OnStartRadical != null) OnStartRadical.Invoke(radicalRoom, radicalURL);
            }

            radicalStarted = !radicalStarted;


        };

    }


    public void SetOSCStatus(string stat)
    {
        label_oscStatus.text = "OSC Status : " + stat;
    }

    public void SetRadicalStatus(string stat)
    {
        label_radicalStatus.text = "Radical Status : " + stat;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
