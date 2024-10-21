using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using WebSocketSharp;
using Fusion.Sockets;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [Header("Textos")]
    public TMP_Text textBarraEstado;
    public TMP_Text textJugadores;
    public TMP_Text textInfoJuego;
    private NetworkRunner runner;

    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelBienvenida;
    public GameObject panelCrearSala;
    public GameObject panelUnirseSala;
    public GameObject panelSalaCreada;
    private GameObject panelAnterior;
    private GameObject panelActual;

    [Header("Variables")]
    public TMP_InputField ifNombreSala;
    public TMP_InputField ifNombreSalaUnirse;
    public TMP_InputField ifMinJugadores;
    public TMP_InputField ifMaxJugadores;
    public Toggle boolPrivada;
    

    private int maxJugadores = 10;
    private int minJugadores = 10;
    private List<int> jugadores = new();



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
        //ConnectToServer();
        textBarraEstado.text = "Conexión con el servidor establecida";
        CambiarPanel(panelBienvenida);
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
            runner = gameObject.AddComponent<NetworkRunner>();
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
        panelSalaCreada.SetActive(false);
        panelCrearSala.SetActive(false);
        panelUnirseSala.SetActive(false);


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

    public void OnIrUnirseSalaBtClick()
    {
        CambiarPanel(panelUnirseSala);
    }


    public void OnVolverBtClick()
    {
        CambiarPanel(panelAnterior);
    }


    public void OnCrearSalaBtClick()
    {
        if (FiltroOkDatosCrearSala())
        {
            CrearSala();
        }
        
    }

    public void OnUnirseSalaBtClick()
    {
        
            UnirseSala();
        

    }

    private bool FiltroOkDatosCrearSala()
    {
        if (String.IsNullOrEmpty(ifNombreSala.text) 
            || !int.TryParse(ifMaxJugadores.text, result: out maxJugadores) 
            || !int.TryParse(ifMinJugadores.text, result: out minJugadores) 
            || minJugadores <= 1
            || maxJugadores < minJugadores
            )
        {
            Debug.Log(ifNombreSala.text);
            return false;
        }
        Debug.Log(ifNombreSala.text);
        return true;
    }


    public async void CrearSala()
    {
        textBarraEstado.text = "Creando sala...";

        //Antes de crear la sala, tenemos que destruir
        //el runner que está en ejecución (sólo el componente)
        //y esperamos a que termine 
        await DestroyNetworkRunnerAsync();

        //Iniciamos el nuevo runner
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.AddCallbacks(this);
        }

        //Creamos el diccionario para almacenar el mínimo núm de jugadores
        Dictionary<String, Fusion.SessionProperty> propiedades =
            new()
            {
                ["minimo"] = minJugadores
            };

        //Definimos los argumentos de conexión
        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = ifNombreSala.text,
            PlayerCount = maxJugadores,
            IsVisible = !boolPrivada.isOn,
            SessionProperties = propiedades,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        //Lanzamos el juego 
        var resultado = await runner.StartGame(startArgs);

        if (resultado.Ok)
        {
            textBarraEstado.text = "Sala " + startArgs.SessionName + " creada correctamente";
            CambiarPanel(panelSalaCreada);
        }
        else
        {
            textBarraEstado.text = "No se ha podido crear la sala";
        }
    }




    public async void UnirseSala()
    {

        textBarraEstado.text = "Uniéndose a sala...";
        //Antes de crear o unirnos a la sala, tenemos que destruir
        //el runner que está en ejecución (sólo el componente)
        //y esperamos a que termine 
        await DestroyNetworkRunnerAsync();

        //Iniciamos el nuevo runner
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }


        //Definimos los argumentos de conexión
        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = ifNombreSalaUnirse.text
        };

        //Lanzamos el juego 
        StartGameResult resultado = await runner.StartGame(startArgs);

        if (resultado.Ok)
        {
            textBarraEstado.text = "Entrada a sala " + startArgs.SessionName + " realizada correctamente";
            CambiarPanel(panelSalaCreada);
        }
        else
        {
            textBarraEstado.text = "No se ha podido entrar a la sala" + startArgs.SessionName;
        }

    }


    public async void UnirseSala2(List<SessionInfo> sessions)
    {

        textBarraEstado.text = "Uniéndose a sala...";
        //Antes de crear o unirnos a la sala, tenemos que destruir
        //el runner que está en ejecución (sólo el componente)
        //y esperamos a que termine 
        await DestroyNetworkRunnerAsync();

        //Iniciamos el nuevo runner
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            
        }


        //Definimos los argumentos de conexión
        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessions[0].Name
        };

        //Lanzamos el juego 
        StartGameResult resultado = await runner.StartGame(startArgs);

        if (resultado.Ok)
        {
            textBarraEstado.text = "Entrada a sala " + startArgs.SessionName + " realizada correctamente";
            CambiarPanel(panelSalaCreada);
        }
        else
        {
            textBarraEstado.text = "No se ha podido entrar a la sala" + startArgs.SessionName;
        }

    }


    /// <summary>
    /// Creamos un método para destruir el componente runner
    /// Es necesario esperar a que termine, por lo que usamos tareas
    /// </summary>
    /// <returns></returns>
    private async Task DestroyNetworkRunnerAsync()
    {
        if (runner != null)
        {
            // Destruir el NetworkRunner
            Destroy(runner);

            // Esperar un tiempo para asegurarnos que Unity procese la destrucción
            await Task.Yield(); // Task.Yield asegura que la operación se espera correctamente
        }
    }


    #region Callbacks
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
        //jugadores.Add(player.PlayerId);

        string texto = "";

        foreach (var item in runner.ActivePlayers)
        {
           texto += $"Jugador {item} \n";
        }
        textJugadores.text = texto;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
        string texto = "";

        foreach (var item in runner.ActivePlayers)
        {
            texto += $"Jugador {item} \n";
        }
        textJugadores.text = texto;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //UnirseSala2(sessionList);
        Debug.Log(sessionList);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }
    #endregion
}
