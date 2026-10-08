using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class CardConnector : Draggable
{
	private enum VisualsState
	{
		None,
		ActiveUnconnected,
		Inactive,
		ActiveConnected
	}

	[HideInInspector]
	public string UniqueId;

	[HideInInspector]
	public GameCard Parent;

	public SpriteRenderer ConnectorIcon;

	[HideInInspector]
	public string ConnectedNodeUniqueId;

	[HideInInspector]
	public CardConnector ConnectedNode;

	public Rectangle ConnectorRect;

	public Rectangle OutlineRect;

	public CardDirection CardDirection;

	public ConnectionType ConnectionType;

	[HideInInspector]
	public Vector3 Middle;

	[HideInInspector]
	public Vector3 MiddleVelo;

	public float ClosedStateScale = 0.2f;

	private bool isActive;

	private Vector3 scaleRef;

	private Vector3 targetScale;

	private Vector3 targetPosition;

	private Vector3 BasePosition;

	private VisualsState currentVisualsState;

	private static List<CardConnector> nodeTracker = new List<CardConnector>();

	public bool IsEnergyConnector
	{
		get
		{
			if (ConnectionType != ConnectionType.LV)
			{
				return ConnectionType == ConnectionType.HV;
			}
			return true;
		}
	}

	public void InitializeEnergyNode(CardConnectorData data, GameCard parent)
	{
		Parent = parent;
		CardDirection = data.EnergyConnectionType;
		ConnectionType = data.EnergyConnectionStrength;
		BasePosition = base.transform.localPosition;
	}

	protected override void Update()
	{
		if (WorldManager.instance.CurrentBoard.Id != "cities")
		{
			bool flag = false;
			if (!WorldManager.instance.CanUseTransport && ConnectionType == ConnectionType.Transport)
			{
				flag = true;
			}
			if (ConnectionType != ConnectionType.Transport)
			{
				flag = true;
			}
			if (flag)
			{
				base.transform.localScale = Vector3.zero;
				return;
			}
		}
		if (WorldManager.instance.CurrentView == ViewType.Default || WorldManager.instance.CurrentView == ViewType.Calamity)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Energy && ConnectionType != ConnectionType.LV && ConnectionType != ConnectionType.HV)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Transport && ConnectionType != ConnectionType.Transport)
		{
			isActive = false;
		}
		else if (WorldManager.instance.CurrentView == ViewType.Sewer && ConnectionType != ConnectionType.Sewer)
		{
			isActive = false;
		}
		else
		{
			isActive = true;
		}
		if (isActive)
		{
			bool flag2 = InputController.instance.StoppedGrabbing();
			if ((InputController.instance.GetInputEnded(0) || flag2) && CitiesManager.instance.DrawingConnector != null)
			{
				CardConnector cardConnector = WorldManager.instance.HoveredDraggable as CardConnector;
				if (cardConnector != null && cardConnector != CitiesManager.instance.DrawingConnector)
				{
					if (cardConnector.ConnectedNode == null)
					{
						AudioManager.me.PlaySound2D(GetConnectSoundForType(ConnectionType), 1f, 0.8f);
						CitiesManager.instance.StopDrawCable(WorldManager.instance.HoveredDraggable as CardConnector);
					}
					else
					{
						CitiesManager.instance.DrawingConnector = null;
					}
				}
				else
				{
					CitiesManager.instance.StopDrawCable(null);
				}
			}
		}
		UpdateConnectorVisuals();
	}

	private Sprite GetSpriteForConnection(ConnectionType connection)
	{
		return connection switch
		{
			ConnectionType.HV => SpriteManager.instance.HighVoltageSprite, 
			ConnectionType.LV => SpriteManager.instance.LowVoltageSprite, 
			ConnectionType.Sewer => SpriteManager.instance.SewerSprite, 
			ConnectionType.Transport => SpriteManager.instance.TransportSprite, 
			_ => null, 
		};
	}

	private Color GetColorForConnection(ConnectionType connection, bool isConnected)
	{
		switch (connection)
		{
		case ConnectionType.HV:
			if (!isConnected)
			{
				return ColorManager.instance.HighVoltageConnector;
			}
			return ColorManager.instance.HighVoltageConnectorActive;
		case ConnectionType.LV:
			if (!isConnected)
			{
				return ColorManager.instance.LowVoltageConnector;
			}
			return ColorManager.instance.LowVoltageConnectorActive;
		case ConnectionType.Sewer:
			if (!isConnected)
			{
				return ColorManager.instance.SewerConnector;
			}
			return ColorManager.instance.SewerConnectorActive;
		case ConnectionType.Transport:
			if (!isConnected)
			{
				return ColorManager.instance.TransportConnector;
			}
			return ColorManager.instance.TransportConnectorActive;
		default:
			return ColorManager.instance.LowVoltageConnector;
		}
	}

	public void UpdateConnectorVisuals()
	{
		if (!Parent.MyBoard.IsCurrent)
		{
			return;
		}
		CardConnector drawingConnector = CitiesManager.instance.DrawingConnector;
		ConnectorIcon.sprite = GetSpriteForConnection(ConnectionType);
		targetScale = Vector3.one;
		targetPosition = BasePosition;
		bool flag = ConnectedNode != null;
		if (isActive)
		{
			if (!flag && currentVisualsState != VisualsState.ActiveUnconnected)
			{
				currentVisualsState = VisualsState.ActiveUnconnected;
				OutlineRect.Color = Color.black;
				ConnectorRect.Color = GetColorForConnection(ConnectionType, ConnectedNode != null);
				SpriteRenderer connectorIcon = ConnectorIcon;
				Rectangle connectorRect = ConnectorRect;
				int num2 = (OutlineRect.SortingLayerID = SortingLayer.NameToID("Above"));
				int sortingLayerID = (connectorRect.SortingLayerID = num2);
				connectorIcon.sortingLayerID = sortingLayerID;
				Rectangle outlineRect = OutlineRect;
				sortingLayerID = (ConnectorRect.RenderQueue = 3500);
				outlineRect.RenderQueue = sortingLayerID;
			}
			if (flag && currentVisualsState != VisualsState.ActiveConnected)
			{
				currentVisualsState = VisualsState.ActiveConnected;
				OutlineRect.Color = Color.black;
				ConnectorRect.Color = GetColorForConnection(ConnectionType, ConnectedNode != null);
				SpriteRenderer connectorIcon2 = ConnectorIcon;
				Rectangle connectorRect2 = ConnectorRect;
				int num2 = (OutlineRect.SortingLayerID = SortingLayer.NameToID("Above"));
				int sortingLayerID = (connectorRect2.SortingLayerID = num2);
				connectorIcon2.sortingLayerID = sortingLayerID;
				Rectangle outlineRect2 = OutlineRect;
				sortingLayerID = (ConnectorRect.RenderQueue = 3500);
				outlineRect2.RenderQueue = sortingLayerID;
			}
			PerformanceHelper.SetActive(ConnectorIcon.gameObject, active: true);
			if (WorldManager.instance.HoveredDraggable == this)
			{
				targetScale = Vector3.one * 1.1f;
			}
			else
			{
				targetScale = Vector3.one;
			}
			if (drawingConnector != null && drawingConnector != this && (drawingConnector.ConnectionType != ConnectionType || drawingConnector.CardDirection == CardDirection))
			{
				targetScale = Vector3.zero;
			}
			if (IsHovered)
			{
				if (ConnectionType == ConnectionType.LV || ConnectionType == ConnectionType.HV)
				{
					string termId = ((CardDirection == CardDirection.input) ? "label_connection_type_input" : "label_connection_type_output");
					string termId2 = ((ConnectionType == ConnectionType.LV) ? "label_connection_low_voltage" : "label_connection_high_voltage");
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate(termId2) + " " + SokLoc.Translate(termId);
				}
				else if (ConnectionType == ConnectionType.Sewer)
				{
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate("label_connection_sewer");
				}
				else if (ConnectionType == ConnectionType.Transport)
				{
					string termId3 = ((CardDirection == CardDirection.input) ? "label_connection_type_input" : "label_connection_type_output");
					GameScreen.InfoBoxText = SokLoc.Translate("label_connector_info");
					GameScreen.InfoBoxTitle = SokLoc.Translate("label_connection_transport") + " " + SokLoc.Translate(termId3);
				}
			}
		}
		else
		{
			SetToBackground();
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, targetScale, 20f * Time.deltaTime);
		base.transform.localPosition = targetPosition;
	}

	private void SetToBackground()
	{
		if (currentVisualsState != VisualsState.Inactive)
		{
			currentVisualsState = VisualsState.Inactive;
			SpriteRenderer connectorIcon = ConnectorIcon;
			Rectangle connectorRect = ConnectorRect;
			int num2 = (OutlineRect.SortingLayerID = SortingLayer.NameToID("Default"));
			int sortingLayerID = (connectorRect.SortingLayerID = num2);
			connectorIcon.sortingLayerID = sortingLayerID;
			Rectangle outlineRect = OutlineRect;
			sortingLayerID = (ConnectorRect.RenderQueue = 3000);
			outlineRect.RenderQueue = sortingLayerID;
			OutlineRect.Color = WorldManager.instance.CurrentBoard.BoardOptions.CardBackgroundPallete.Color2;
			ConnectorRect.Color = WorldManager.instance.CurrentBoard.BoardOptions.CardBackgroundPallete.Color;
		}
		targetScale = Vector3.one * 0.75f;
		targetPosition = BasePosition + Vector3.forward * 0.03f;
		if (Vector3.Distance(base.transform.localScale, targetScale) < 0.1f)
		{
			PerformanceHelper.SetActive(ConnectorIcon.gameObject, active: false);
		}
	}

	public override void Clicked()
	{
		if (!isActive)
		{
			return;
		}
		if (ConnectedNode != null)
		{
			if (CitiesManager.instance.DrawingConnector == null)
			{
				SetConnectedNode(null);
				CitiesManager.instance.StartDrawCable(this);
			}
			var (clip, vol) = GetStartSoundForType(ConnectionType);
			AudioManager.me.PlaySound2D(clip, 1f, vol);
		}
		else if (CitiesManager.instance.DrawingConnector == null)
		{
			CitiesManager.instance.StartDrawCable(this);
			var (clip2, vol2) = GetStartSoundForType(ConnectionType);
			AudioManager.me.PlaySound2D(clip2, 1f, vol2);
		}
	}

	public void SetConnectedNode(CardConnector connector)
	{
		if (connector != null)
		{
			ConnectedNode = connector;
			connector.ConnectedNode = this;
			return;
		}
		if (ConnectedNode != null)
		{
			ConnectedNode.ConnectedNode = null;
		}
		ConnectedNode = null;
	}

	public SavedCardConnector ToSavedEnergyConnector()
	{
		if (ConnectedNode == null)
		{
			return null;
		}
		return new SavedCardConnector
		{
			UniqueId = GetConnectorUniqueId(),
			ConnectedNodeUniqueId = ConnectedNode.GetConnectorUniqueId()
		};
	}

	public string GetConnectorUniqueId()
	{
		string uniqueId = Parent.CardData.UniqueId;
		string text = CardDirection.ToString();
		string text2 = ConnectionType.ToString();
		int myIndex = GetMyIndex();
		return $"{uniqueId}_{text2}_{text}_{myIndex}";
	}

	private int GetMyIndex()
	{
		int num = 0;
		for (int i = 0; i < Parent.CardConnectorChildren.Count; i++)
		{
			CardConnector cardConnector = Parent.CardConnectorChildren[i];
			if (cardConnector == this)
			{
				return num;
			}
			if (cardConnector.ConnectionType == ConnectionType && cardConnector.CardDirection == CardDirection)
			{
				num++;
			}
		}
		throw new Exception();
	}

	public (AudioClip, float) GetStartSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return (AudioManager.me.EnergyStart, 0.6f);
		case ConnectionType.Sewer:
			return (AudioManager.me.SewerStart, 0.7f);
		case ConnectionType.Transport:
			return (AudioManager.me.TransportStart, 0.8f);
		default:
			return (null, 0f);
		}
	}

	public AudioClip GetConnectSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return AudioManager.me.EnergyConnected;
		case ConnectionType.Sewer:
			return AudioManager.me.SewerConnected;
		case ConnectionType.Transport:
			return AudioManager.me.TransportConnected;
		default:
			return null;
		}
	}

	public AudioClip GetStretchSoundForType(ConnectionType connection)
	{
		switch (connection)
		{
		case ConnectionType.LV:
		case ConnectionType.HV:
			return AudioManager.me.EnergyStrech;
		case ConnectionType.Sewer:
			return AudioManager.me.SewerStrech;
		case ConnectionType.Transport:
			return AudioManager.me.TransportStrech;
		default:
			return null;
		}
	}

	public bool HasEnergyOutput()
	{
		nodeTracker.Clear();
		return Parent.CardData.HasEnergyOutput(this, nodeTracker);
	}

	public bool HasEnergyInput()
	{
		return Parent.CardData.HasEnergyInput(this);
	}

	public override bool CanBePushed()
	{
		return false;
	}

	public override bool CanBeDragged()
	{
		return false;
	}

	public override bool CanBePushedBy(Draggable draggable)
	{
		return false;
	}

	protected override void ClampPos()
	{
	}

	public GameCard GetConnectedGameCard()
	{
		return ConnectedNode?.Parent;
	}
}
