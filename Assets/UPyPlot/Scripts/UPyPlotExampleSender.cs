using UnityEngine;
using ClapDetector;
using Audio;
using System.Collections;
using System.Collections.Generic;

public class UPyPlotExampleSender : MonoBehaviour {

	private AudioClip m_acRecording = null;

	public MicListener micListener;

	//public ClapDetector.ClapDetector clapDetector;

	[UPyPlot.UPyPlotController.UPyProbe] // Add probe so this value will be plotted.
	private float xVar;

	// [UPyPlot.UPyPlotController.UPyProbe] // Add probe so this value will be plotted.
	// private float zVar; 

	// [UPyPlot.UPyPlotController.UPyProbe] // Add probe so this value will be plotted.
	// [Range(-10,10)]                      // Add a manual control slider so its easy to change in real time.
	// [SerializeField] public float yVar;  // A public variable that user can adjust manually and see in plot.

	private float lastRndX = 0;
	private float lastRndZ = 0;

	void Start() {
		micListener = new MicListener();
		micListener.StartRecording();
		// Runnable.Run(getData());
		//clapDetector = new ClapDetector.ClapDetector();
		//clapDetector.Listen();
	}

	private IEnumerator getData() {
		while (true) {
			if (micListener.data.Count != 0) {
				xVar = micListener.data.Dequeue();
				yield return new WaitForSeconds(0.01f);
			}
			yield return new WaitForSeconds(0.15f);
		}
	}

	void Update () { // Some example code that makes the values change in the plot.
		// xVar = micListener.xVar;
		// if (micListener.data.Count != 0) {
		// 	Debug.Log(micListener.data.Count);
		// 	xVar = micListener.data.Dequeue();
		// }
		//xVar = Mathf.Lerp(lastRndX, Random.Range (0.0f, 10.0f), Time.deltaTime * 0.5f );
		//xVar = clapDetector.streamingMic.current;
		// Debug.Log(xVar);
		// lastRndX = xVar;

		// zVar = Mathf.Lerp(lastRndZ, Random.Range(-10.0f, 10.0f), Time.deltaTime * 0.5f );
		// lastRndZ = zVar;

		// transform.position = new Vector3 (xVar, yVar, zVar); // Move the example sphere.
	}
}
