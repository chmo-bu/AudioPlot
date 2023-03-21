using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using TCPSocket;

// namespace Audio {
public class MicListener : MonoBehaviour
{//AV 0401 2022: filtering with AudioSource //https://youtu.be/GHc9RF258VA?t=223
    private int m_nRecordingRoutine = 0;
    private string m_sMicrophoneID = null;
    public AudioClip m_acRecording = null;
    private int m_nRecordingBufferSize = 1;
    public int m_nRecordingHZ = 16000;
    private float[] samples;

    public Queue<float> data;

    // [UPyPlot.UPyPlotController.UPyProbe] // Add probe so this value will be plotted.

    private TCPClient client;

    void Start() {
        StartRecording();
        client = new TCPClient();
    }
    // public MicListener() {
    //     data = new Queue<float>();
    //     // client = new TCPClient();
    // }

    public bool isListening() {
        return (m_nRecordingRoutine != 0);
    }

    public void StartRecording()
    {
        if (m_nRecordingRoutine == 0)
        {
            //UnityObjectUtil.StartDestroyQueue();
            m_nRecordingRoutine = Runnable.Run(RecordingHandler());
        }
    }

    public void StopRecording()
    {
        if (m_nRecordingRoutine != 0)
        {
            Microphone.End(m_sMicrophoneID);
            Runnable.Stop(m_nRecordingRoutine);
            m_nRecordingRoutine = 0;
            Debug.Log("StopRecording() ");
        }
    }

    private void OnError(string error)
    {
        Debug.Log("StreamingMic Error! " + error);
    }

    public void ToggleMicrophone()
    {
    }

    private IEnumerator RecordingHandler()
    {
        Debug.Log("****StreamingMic devices: " + Microphone.devices);
        m_acRecording = Microphone.Start(m_sMicrophoneID, true, m_nRecordingBufferSize, m_nRecordingHZ);
        while (!(Microphone.GetPosition(null) > 0))
        {
        }
        yield return null;

        if (m_acRecording == null)
        {
            StopRecording();
            yield break;
        }

        // float[] samples = null;
        samples = null;
        int lastSample = 0;
        
        byte[] data = null;

        while (m_nRecordingRoutine != 0 && m_acRecording != null)
        {//we enter this function asyncronously approximately every 100ms (but buffers are uneven). It does not make any sense to re-calculate the threshold every millisend
            int pos = Microphone.GetPosition(m_sMicrophoneID);
            if (pos > m_acRecording.samples || !Microphone.IsRecording(m_sMicrophoneID))
            {
                //Debug.Log("MicrophoneWidget Microphone disconnected.");
                StopRecording();
                yield break;
            }

            int diff = pos - lastSample;
            //Debug.Log("pos=" + pos + ", lastSample=" + lastSample + ", diff=" + diff);

            if (diff > 0)
            {
                int nsamplesarray = diff * m_acRecording.channels;
                samples = new float[nsamplesarray];
                m_acRecording.GetData(samples, 0);//m_acRecording.GetData(samples, lastSample);

                // for (int i=0; i<nsamplesarray; i++) {
                //     Debug.Log(samples[i]);
                //     // if (data.Count < 30000) {
                //     //     data.Enqueue(samples[i]);
                //     // }
                // }

                data = new byte[nsamplesarray*sizeof(float)];

                // if (data.Length > 10000) {
                //     Debug.Log(data.Length);
                // }

                Buffer.BlockCopy(samples, 0, data, 0, data.Length);

                // Debug.Log("sample: " + samples[nsamplesarray-2]);

                client.SendMessage(data);

                // Debug.Log("data = " + String.Join(",",
                //     new List<float>(samples)
                //     .ConvertAll(i => i.ToString())
                //     .ToArray()));

                // stream.Write(data, 0, data.Length);
            }
            else
            {
                samples = new float[m_acRecording.samples];
                m_acRecording.GetData(samples, 0);
            }
            lastSample = pos;
            yield return new WaitForSeconds(0.1f);
        }

        yield break;
    }       
}
// }