using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicCamera : MonoBehaviour
{
	public float dollySpeed = 1f;
	public float flyInSpeed = 3f;

	public float eventPaddingBefore = 2f;
	public float eventPaddingAfter = 2f;
	public float eventPaddingAfterDeath = .4f;

	public static Action<UnitID> OnActionCameraIn;
	public static Action<UnitID> OnActionCameraOut;

	public static UnitID activeActionActor;


	public Transform focusPoint;
	public new Camera camera;
	public Camera planningCamera;
	public AudioListener microphone;
	public CinemachineSmoothPath circlePath, zoomPath;
	public CinemachineVirtualCamera virtualCamera;
	CinemachineTrackedDolly dolly;

	bool inPlaybackMode = false;

	public Transform focusedUnit;
	public Vector3 smoothFocusVal;

	bool circling = false;
	float circleCameraProgress = 0;

	float unitHeight = 1.7f;
	float flyinHeight = 10f;
	float rightOfCharacterAmount = 8f;
	float flyBehindCharacterAmount = 4f;
	float flyBehindRightOffset = 2.5f;
	float lookAheadFocus = 5f;
	float flyBehindHeight = 2f;
	float flyinProgressStop = 4f;
	Vector3 flyinTarget;

	ActionCamera actionCamera;
	List<MatchEventType> notableEvents = new List<MatchEventType> { MatchEventType.Shoot, MatchEventType.Death };

	public Image[] blinds;
	float blindHeight = 540f;
	float blindOpenSpeed = 1500f;

	public AudioListener globalAudioListener;

	public float playbackStart;


	class UnitFocus
	{
		public float duration;
		public Transform unitModel;
	}

	List<UnitFocus> focusGroup = new List<UnitFocus>();

	class ActionClip
	{
		public float start, end;
		public UnitID unit;
		public bool canRun = true;
		public bool hasStarted = false;
		public bool hasEnded = false;
	}

	List<ActionClip> actionClips = new List<ActionClip>();

	private void Awake()
	{
		MatchManager.OnMatchUpdate += OnMatchUpdate;
		MatchManager.OnMatchUnloaded += () =>
		{
			planningCamera.enabled = false;
			camera.enabled = false;
			circling = false;
		};
	}

	void OnMatchUpdate(Match match)
	{

		if (match.state == Match.GameState.Playback)
		{
			EnableCinematicCamera();
			actionClips.Clear();

			foreach (Unit unit in match.GetAllUnits(match.GetLocalTeam()))
				if (unit.timeline.GetDuration() > 0)
				{
					UnitFocus focus = new UnitFocus();
					focus.unitModel = VisualUnitManager.GetVisualUnitById(unit.id).modelTransform;
					focus.duration = unit.timeline.GetDuration();
					focusGroup.Add(focus);
				}

			foreach (MatchEvent matchEvent in match.events)
			{
				if (notableEvents.Contains(matchEvent.type))
				{
					Unit actor = match.GetUnit(matchEvent.actorUnitId);
					if (actor.ownerID == ClientLogin.user.id)
					{
						ActionClip clip = new ActionClip();
						clip.start = matchEvent.time - eventPaddingBefore;
						clip.end = matchEvent.time + eventPaddingAfter;
						clip.unit = actor.id;

						actionClips.Add(clip);
					}
				}
			}



			// Checks for overlapping actions clips and removes them
			ValidateActionClips();

			// Merges action clips if they are performed by the same unit
			MergeActionClips();

			// Remove unrunnable clips that may have been modified with the merge action
			RemoveUnrunnableActionClips();
			inPlaybackMode = true;
			playbackStart = Time.time;
			StartCircling();
		} else
		{
			if (match.state == Match.GameState.Starting || match.state == Match.GameState.Planning || match.state == Match.GameState.Ending) EnablePlanningCamera();

			inPlaybackMode = false;
		}
	}

	void MergeActionClips()
	{
		if (actionClips.Count < 2) return;
		for (int i = 0; i < actionClips.Count - 1; i++)
		{
			if (actionClips[i].unit == actionClips[i + 1].unit)
			{
				// Make this clip not runnable (it will be replaced by the next clip)
				actionClips[i].canRun = false;
				// Move the start of the next clip to the start of this clip
				actionClips[i + 1].start = actionClips[i].start;
			}
		}
	}

	void ValidateActionClips()
	{
		foreach (ActionClip clip in actionClips)
			foreach (ActionClip otherClip in actionClips)
				if (otherClip.start >= clip.start && otherClip.start <= clip.end && otherClip.unit != clip.unit)
				{
					otherClip.canRun = false;
					clip.canRun = false;
				}
		RemoveUnrunnableActionClips();
	}

	void RemoveUnrunnableActionClips()
	{
		for (int i = 0; i < actionClips.Count; i++)
			if (!actionClips[i].canRun) actionClips.RemoveAt(i);

	}

	float GetTimePassed()
	{
		return Time.time - playbackStart;
	}

	void Update()
	{
		if (inPlaybackMode)
		{
			float time = GetTimePassed();
			foreach (ActionClip clip in actionClips)
			{
				if (time >= clip.start && !clip.hasStarted && clip.canRun)
				{
					clip.hasStarted = true;
					VisualUnit visualUnit = VisualUnitManager.GetVisualUnitById(clip.unit);
					focusedUnit = visualUnit.modelTransform;

					activeActionActor = visualUnit.id;
					OnActionCameraIn?.Invoke(activeActionActor);

					CreateZoomPath(visualUnit.transform);
				}
				if (time >= clip.end && !clip.hasEnded && clip.canRun)
				{

					clip.hasEnded = true;
					StartCircling();
				}
			}

			/*foreach(Unit unit in match)*/
		}
	}

	void SetBlindsVisible(bool show)
	{
		SetBlindHeights(show ? blindHeight : 0);
	}

	void SetBlindHeights(float height)
	{
		foreach (Image blind in blinds)
			blind.rectTransform.sizeDelta = new Vector2(blind.rectTransform.sizeDelta.x, height);
	}

	void Start()
	{
		dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
		SetBlindsVisible(false);
		EnablePlanningCamera();
	}

	void EnablePlanningCamera()
	{
		StartCoroutine(AnimateUpBlinds());
		circling = false;
		DisableActionCamera();
		camera.enabled = false;
		planningCamera.enabled = true;
	}

	void EnableCinematicCamera()
	{
		DisableActionCamera();
		planningCamera.enabled = false;
		camera.enabled = true;
	}

	void DisableActionCamera()
	{
		globalAudioListener.enabled = true;
		if (actionCamera != null)
		{
			actionCamera.microphone.enabled = false;
			actionCamera.camera.enabled = false;
		}
	}

	[ContextMenu("Create zoom path")]
	void CreateZoomPath(Transform unit)
	{

		actionCamera = unit.GetComponentInChildren<ActionCamera>();

		if (actionCamera == null)
		{
			Debug.LogWarning("No action camera found on unit");
			StartCircling();
			return;
		}

		List<CinemachineSmoothPath.Waypoint> path = new List<CinemachineSmoothPath.Waypoint>();
		flyinTarget = GetUnitHeadPosition();

		path.Add(CreateWaypoint(camera.transform.position));

		Vector3 flyToUnit = flyinTarget;

		path.Add(CreateWaypoint(flyToUnit));

		zoomPath.m_Waypoints = path.ToArray();
		virtualCamera.LookAt = focusPoint;

		circling = false;

		circleCameraProgress = dolly.m_PathPosition;
		dolly.m_PathPosition = 0f;
		dolly.m_Path = zoomPath;


		StartCoroutine(StartFlyingIn());
	}

	Vector3 GetUnitHeadPosition()
	{
		return new Vector3(focusedUnit.position.x, unitHeight, focusedUnit.position.z);
	}

	IEnumerator StartFlyingIn()
	{
		while (dolly.m_PathPosition <= flyinProgressStop)
		{
			Vector3 flyinFocus = (focusedUnit.forward * lookAheadFocus) + GetUnitHeadPosition();
			focusPoint.position = flyinFocus;
			dolly.m_PathPosition += Time.deltaTime * flyInSpeed;

			yield return new WaitForEndOfFrame();
		}
		SwitchToActionCamera();
	}

	void SwitchToActionCamera()
	{
		SetBlindsVisible(true);
		camera.enabled = false;
		planningCamera.enabled = false;
		globalAudioListener.enabled = false;
		actionCamera.camera.enabled = true;
		actionCamera.microphone.enabled = true;
		StartCoroutine(AnimateUpBlinds());
	}

	IEnumerator AnimateUpBlinds()
	{
		float height = blindHeight;
		while (height > 0)
		{
			height -= blindOpenSpeed * Time.deltaTime;
			SetBlindHeights(height);
			yield return new WaitForEndOfFrame();
		}
	}


	CinemachineSmoothPath.Waypoint CreateWaypoint(Vector3 position)
	{
		CinemachineSmoothPath.Waypoint waypoint = new CinemachineSmoothPath.Waypoint();
		waypoint.position = position;
		return waypoint;
	}

	[ContextMenu("Start circle")]
	public void StartCircling()
	{
		if (activeActionActor != null)
		{
			OnActionCameraOut?.Invoke(activeActionActor);
			activeActionActor = null;
		}
		if (actionCamera != null) actionCamera.camera.enabled = false;
		camera.enabled = true;

		circling = true;
		dolly.m_Path = circlePath;
		dolly.m_PathPosition = circleCameraProgress;
		virtualCamera.LookAt = focusPoint;

		Vector3 avgFocus = GetAverageFocus();
		if (focusGroup.Count > 0)
			focusPoint.position = avgFocus;


		StartCoroutine(CircleEnumerator());

	}

	Vector3 GetAverageFocus()
	{
		float timePassed = GetTimePassed();

		Vector3 avgFocus = Vector3.zero;
		if (focusGroup.Count > 0)
		{
			for (int i = 0; i < focusGroup.Count; i++)
			{
				UnitFocus unitFocus = focusGroup[i];

				if (unitFocus.duration < timePassed) focusGroup.RemoveAt(i);
				else avgFocus += unitFocus.unitModel.position;
			}
			if (focusGroup.Count > 1) avgFocus /= focusGroup.Count;

		} else
		{
			List<VisualUnit> visualUnits = VisualUnitManager.GetUserVisualUnitsById(ClientLogin.user.id);
			int aliveUnits = 0;
			if (visualUnits != null)
			{
				foreach (VisualUnit visualUnit in visualUnits)
				{
					if (!visualUnit.isDead)
					{
						aliveUnits++;
						avgFocus += visualUnit.modelTransform.position;
					}
				}

				if (aliveUnits == 0) return Vector3.zero;
				avgFocus /= aliveUnits;
			}

		}


		return avgFocus;
	}

	IEnumerator CircleEnumerator()
	{

		while (circling)
		{

			Vector3 avgFocus = GetAverageFocus();

			Vector3 result = Vector3.SmoothDamp(focusPoint.position, avgFocus, ref smoothFocusVal, 1f);
			if (!float.IsNaN(result.x))
				focusPoint.position = result;

			dolly.m_PathPosition += Time.deltaTime * dollySpeed;
			circleCameraProgress = dolly.m_PathPosition;
			yield return new WaitForEndOfFrame();
		}
	}

}
