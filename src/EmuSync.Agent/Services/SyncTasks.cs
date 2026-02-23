using System.Collections.Concurrent;

namespace EmuSync.Agent.Services;

public class SyncTasks(ILogger<SyncTasks> logger) : ISyncTasks
{

    private readonly ILogger<SyncTasks> _logger = logger;
    private readonly ConcurrentDictionary<string, GameEntity> _syncTasks = new();

    public bool HasTasks()
    {
        return !_syncTasks.IsEmpty;
    }

    public GameEntity? GetNext()
    {
        if (_syncTasks.IsEmpty)
        {
            return null;
        }

        GameEntity game = _syncTasks.FirstOrDefault().Value;
        _syncTasks.TryRemove(game.Id, out _);

        return game;
    }

    public void Add(GameEntity game)
    {
        _logger.LogInformation("[{gameName} / {gameId}] sync task added", game.Name, game.Id);

        _syncTasks.AddOrUpdate(
            game.Id,
            game,
            (_, _) => game
        );
    }

    public bool Update(GameEntity game)
    {
        return _syncTasks.TryGetValue(game.Id, out var existing)
            && _syncTasks.TryUpdate(game.Id, game, existing)
            && LogUpdated(game);
    }

    private bool LogUpdated(GameEntity game)
    {
        _logger.LogInformation(
            "[{gameName} / {gameId}] sync task updated",
            game.Name,
            game.Id
        );

        return true;
    }

    public bool Remove(string gameId)
    {
        return _syncTasks.TryRemove(gameId, out _);
    }

    public void Clear()
    {
        _syncTasks.Clear();
    }
}