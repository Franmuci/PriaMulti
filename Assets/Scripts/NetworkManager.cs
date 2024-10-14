using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
using Fusion.Sockets;
using System;

public class NetworkManager : MonoBehaviour,INetworkRunnerCallbacks
{

    [Header ("Textos")]
    public TMP_Text textBarraEstado;
    private NetworkRunner runner;

    [Header ("Paneles")]
    public GameObject panelInicio;
    public GameObject panelBienvenida;
    public GameObject panelCrearSala;
    public GameObject panelSala;
    private GameObject panelAnterior;
    private GameObject panelActual;


    private void Start()
    {
        panelActual = panelInicio;
        CambiarPanel(panelInicio);
    }


    /// <summary>
    /// Ejecuta una conexión al pulsar el botón.
    /// </summary>
    public void OnConectaBtClick()
    {
        ConnectToServer();
    }


    /// <summary>
    /// Intenta conectar al servidor para entrar a una lobby
    /// </summary>
    public async void ConnectToServer()
    {

        textBarraEstado.text = "Conectando con el servidor...";

        //Iniciamos el runner
        if (runner == null)
        {
            runner = GetComponent<NetworkRunner>();
        }

        //Definimos los argumentos de la conexión
        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            

        };

        //Lanzamos la conexión
        var res = await runner.StartGame(startArgs);
         

        if (res.Ok)
        {
            textBarraEstado.text = "Conexión con el servidor establecida";
            CambiarPanel(panelBienvenida);
        }
        else
        {
            textBarraEstado.text = "No se ha podido establecer la conexión con el servidor";
        }

    }


    /// <summary>
    /// Desactiva todos los paneles del medio y activa el panel del parametro {panel}
    /// </summary>
    /// <param name="panel"></param>
    private void CambiarPanel(GameObject panel)
    {

        panelInicio.SetActive(false);
        panelBienvenida.SetActive(false);
        panelSala.SetActive(false);
        panelCrearSala.SetActive(false);


        panelAnterior = panelActual;
        panelActual = panel;
        panel.SetActive(true);
    }


    /// <summary>
    /// Al pulsar el boton cambia el panel a IrCrearSala
    /// </summary>
    public void OnIrCreacionSalaBtClick()
    {
        CambiarPanel(panelCrearSala);
    }


    public void OnVolverBtClick()
    {
        CambiarPanel(panelAnterior);
    }

    public void OnCrearSalaBtClick()
    {
        CrearSala();
    }


    private async void ReRunner()
    {
        //runner = gameObject.AddComponent<NetworkRunner>();

        //Creamos el diccionario para almacenar el mínimo núm de jugagores.
        Dictionary<string, SessionProperty> dictionary = new();
        Dictionary<String, Fusion.SessionProperty> propiedades = dictionary;

        propiedades["minimo"] = 2;

        //Definimos los argumentos de la conexión
        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 3,
            SessionName = "Hola",
            IsVisible = true,
            SessionProperties = propiedades
        };

        //Lanzamos la conexión
        var res = await runner.StartGame(startArgs);


        if (res.Ok)
        {
            textBarraEstado.text = "Conexión con el servidor establecida";
            CambiarPanel(panelBienvenida);
        }
        else
        {
            textBarraEstado.text = "No se ha podido establecer la conexión con el servidor";
        }

    }


    public void  CrearSala()
    {

        textBarraEstado.text = "Creando sala...";

        

        //Iniciamos el runner
        if (runner.IsRunning)
        {
            //await runner.Shutdown(true);
            Destroy(runner);
            Invoke(nameof(ReRunner), 0.5f);

        }

        
         


        
    }



    #region Callbacks

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
       
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }
    
    #endregion
}
