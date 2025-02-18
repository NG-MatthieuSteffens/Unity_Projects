﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private Transform m_pTarget;

	[SerializeField]
	private GameObject tBlueBlip;
	[SerializeField]
	private GameObject tRedBlip;
	[SerializeField]
	private GameObject tOrangeBlip;
	[SerializeField]
	private GameObject tBuildingBlip;
	[SerializeField]
	private Vector2 tBuildingsMeshSize = new Vector2(32.0f, 22.0f);

	[SerializeField]
	public float m_fZoom = 3.0f;
	[SerializeField]
	private Vector2 m_tBorderOffset = new Vector2(0.0f, 0.0f);
	
	#endregion
	
	#region Variables (private)

	private Rect m_tBoundaries;

	private GameObject[] pLivingBeings;
	private List<GameObject> pBlips = new List<GameObject>();

	private Vector2 m_tRotationX = Vector2.right;
	private Vector2 m_tRotationY = Vector2.up;
	
	#endregion


	void Start()
	{
		m_tBoundaries = GetComponent<RectTransform>().rect;

		m_tRotationX = new Vector2(Camera.main.transform.right.x, -Camera.main.transform.right.z);
		m_tRotationY = new Vector2(-CameraControl.HorizontalForward.x, CameraControl.HorizontalForward.z);


		/* Create buildings blips */

		Transform pBlipsParent = transform.FindChild("RenderedBack");

		GameObject[] pBuildings = GameObject.FindGameObjectsWithTag("Building");

		int i = 0;
		int iBlipsCount = 0;

		for (i = 0; i < pBuildings.Length; i++)
		{
			pBlips.Add(Instantiate<GameObject>(tBuildingBlip));
			pBlips[iBlipsCount].name = tBuildingBlip.name;
			pBlips[iBlipsCount].GetComponent<Blip>().Target = pBuildings[i].transform;

			RectTransform tBlipTransform = pBlips[iBlipsCount].GetComponent<RectTransform>();
			tBlipTransform.sizeDelta = tBuildingsMeshSize;
			pBlips[iBlipsCount].transform.SetParent(pBlipsParent, false);

			iBlipsCount++;
		}

		/* Create walls blips */

		GameObject[] pWalls = GameObject.FindGameObjectsWithTag("Wall");

		for (i = 0; i < pWalls.Length; i++)
		{
			pBlips.Add(Instantiate<GameObject>(tBuildingBlip));
			pBlips[iBlipsCount].name = tBuildingBlip.name;
			pBlips[iBlipsCount].GetComponent<Blip>().Target = pWalls[i].transform;

			RectTransform tBlipTransform = pBlips[iBlipsCount].GetComponent<RectTransform>();
			tBlipTransform.sizeDelta = new Vector2(pWalls[i].transform.localScale.x, pWalls[i].transform.localScale.z);
			pBlips[iBlipsCount].transform.SetParent(pBlipsParent, false);

			iBlipsCount++;
		}

		pBlipsParent = transform.FindChild("RenderedFront");

		/* Create living beings blips */

		LivingBeing[] pLivingBeingsComponents = GameObject.FindObjectsOfType<LivingBeing>();
		pLivingBeings = new GameObject[pLivingBeingsComponents.Length];

		for (i = 0; i < pLivingBeingsComponents.Length; i++)
		{
			pLivingBeings[i] = pLivingBeingsComponents[i].gameObject.transform.parent.gameObject;

			switch(pLivingBeings[i].transform.parent.name)
			{
			case "GoodGuys":

				pBlips.Add(Instantiate<GameObject>(tBlueBlip));
				pBlips[iBlipsCount].name = tBlueBlip.name;
				break;

			case "BadGuys":

				pBlips.Add(Instantiate<GameObject>(tRedBlip));
				pBlips[iBlipsCount].name = tRedBlip.name;
				break;

			case "Civilians":

				pBlips.Add(Instantiate<GameObject>(tOrangeBlip));
				pBlips[iBlipsCount].name = tOrangeBlip.name;
				break;
			}

			if (pLivingBeings[i].gameObject.tag == "Target")
				pBlips[iBlipsCount].GetComponent<Blip>().ForwardUpdateFrequency = Blip.ForwardUpdate.IfOutBorders;

			pBlips[iBlipsCount].transform.SetParent(pBlipsParent, false);
			pBlips[iBlipsCount].transform.localPosition = Vector2.zero;
			pBlips[iBlipsCount].GetComponent<Blip>().Target = pLivingBeings[i].transform;

			iBlipsCount++;
		}
	}


	#region Methods

	public Vector2 GetBlipLocalPosition(Vector3 tWorldPos)
	{
		Vector3 tWorldLocalPos = tWorldPos - m_pTarget.position;
		Vector2 tLocalPos = tWorldLocalPos.x * m_tRotationX;
		tLocalPos += tWorldLocalPos.z * m_tRotationY;
		tLocalPos *= m_fZoom;

		return tLocalPos;
	}

	public Vector3 TransformRotation(Vector3 tRotation)
	{
		return new Vector3(0.0f, 0.0f, Camera.main.transform.eulerAngles.y - tRotation.y);
	}

	public Vector2 KeepBlipInBounds(Vector2 tBlipPos, Vector2 tScaledSize)
	{
		tBlipPos = Vector2.Max(tBlipPos, m_tBoundaries.min + (m_tBorderOffset * m_fZoom));
		tBlipPos = Vector2.Min(tBlipPos, m_tBoundaries.max - (m_tBorderOffset * m_fZoom));

		return tBlipPos;
	}

	#endregion Methods
}
