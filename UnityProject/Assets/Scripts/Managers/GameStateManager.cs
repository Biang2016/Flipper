using BiangStudio.Singleton;

public class GameStateManager : TSingletonBaseManager<GameStateManager>
{
    private GameState state = GameState.Waiting;

    public void SetState(GameState newState)
    {
        if (state != newState)
        {
            switch (state)
            {
                case GameState.Playing:
                {
                    break;
                }
                case GameState.Win:
                {
                    break;
                }
                case GameState.ESC:
                {
                    break;
                }
            }

            state = newState;
            switch (state)
            {
                case GameState.Playing:
                {
                    Resume();
                    break;
                }
                case GameState.Win:
                {
                    Pause();
                    break;
                }
                case GameState.ESC:
                {
                    Pause();
                    break;
                }
            }
        }
    }

    public GameState GetState()
    {
        return state;
    }

    private void Pause()
    {
    }

    private void Resume()
    {
    }
}

public enum GameState
{
    Waiting,
    Playing,
    Win,
    ESC,
}