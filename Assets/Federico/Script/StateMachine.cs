using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public string customName;
    private State mainStateType;
    public State CurrentState { get; private set; }
    private State nextState;

    private void Awake()
    {
        // Imposta lo stato principale in base al nome personalizzato
        if (string.IsNullOrEmpty(customName))
        {
            Debug.LogError("customName is null or empty.");
        }
        else
        {
            SetMainState(customName);
        }
        
        // Inizializza lo stato corrente con lo stato principale se non già impostato
        if (CurrentState == null)
        {
            SetState(mainStateType);
        }
    }

    private void OnValidate()
    {
        // Imposta lo stato principale in base al nome personalizzato
        if (string.IsNullOrEmpty(customName))
        {
            Debug.LogError("customName is null or empty.");
        }
        else
        {
            SetMainState(customName);
        }
    }

    private void SetMainState(string stateName)
    {
        switch (stateName)
        {
            case "Combat":
                mainStateType = new IdleCombatState();
                break;
            default:
                Debug.LogError("Unknown state name: " + stateName);
                break;
        }
    }

    private void Start()
    {
        // Inizializza lo stato corrente con lo stato principale se non già impostato
        if (CurrentState == null)
        {
            SetState(mainStateType);
        }
    }

    private void Update()
    {
        // Gestisci la transizione dello stato
        if (nextState != null)
        {
            SetState(nextState);
        }

        // Esegui l'aggiornamento dello stato corrente
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        // Esegui l'aggiornamento tardivo dello stato corrente
        if (CurrentState != null)
        {
            CurrentState.OnLateUpdate();
        }
    }

    private void FixedUpdate()
    {
        // Esegui l'aggiornamento fisso dello stato corrente
        if (CurrentState != null)
        {
            CurrentState.OnFixedUpdate();
        }
    }

    private void SetState(State newState)
    {
        nextState = null;
        if (CurrentState != null)
        {
            CurrentState.OnExit();
        }
        CurrentState = newState;
        if (CurrentState != null)
        {
            CurrentState.OnEnter(this);
        }
    }

    public void SetNextState(State newState)
    {
        if (newState != null)
        {
            nextState = newState;
        }
    }

    public void SetNextStateToMain()
    {
        nextState = mainStateType;
    }
}
