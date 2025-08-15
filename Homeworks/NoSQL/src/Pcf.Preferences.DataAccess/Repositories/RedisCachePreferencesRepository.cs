using Pcf.Preferences.Core.Abstractions.Repositories;
using Pcf.Preferences.Core.Domain;
using StackExchange.Redis;
using System.Linq.Expressions;
using System.Text.Json;

namespace Pcf.Preferences.DataAccess.Repositories
{
    public class RedisCachePreferencesRepository : IRepository<Preference>
    {
        private readonly IRepository<Preference> _repository;
        private readonly IConnectionMultiplexer _redis;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private const string PREFIX_KEY = "preferences";

        public RedisCachePreferencesRepository(IRepository<Preference> repository, IConnectionMultiplexer redis) 
        {
            _repository = repository;
            _redis = redis;
        }

        public async Task AddAsync(Preference entity)
        {
            await _repository.AddAsync(entity);
            await InvalidateCache(entity.Id);
        }

        public async Task DeleteAsync(Preference entity)
        {
            await _repository.DeleteAsync(entity);
            await InvalidateCache(entity.Id);
        }

        public async Task<IEnumerable<Preference>> GetAllAsync()
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"{PREFIX_KEY}:all";

            var cached = await db.StringGetAsync(cacheKey);
            if (!cached.IsNull)
                return JsonSerializer.Deserialize<IEnumerable<Preference>>(cached);

            var preferences = await _repository.GetAllAsync();

            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(preferences),
                _cacheExpiration);

            return preferences;
        }

        public async Task<Preference> GetByIdAsync(Guid id)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"{PREFIX_KEY}:{id}";

            var cached = await db.StringGetAsync(cacheKey);
            if (!cached.IsNull)
                return JsonSerializer.Deserialize<Preference>(cached);

            Preference preference = await _repository.GetByIdAsync(id);
            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(preference),
                _cacheExpiration
            );

            return preference;
        }

        public async Task<Preference> GetFirstWhere(Expression<Func<Preference, bool>> predicate)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"{PREFIX_KEY}:all";
            var cached = await db.StringGetAsync(cacheKey);
            if (!cached.IsNull) 
            {
                var preferences = JsonSerializer.Deserialize<IEnumerable<Preference>>(cached);
                return preferences.FirstOrDefault(predicate.Compile());
            }

            return await GetFirstWhere(predicate);
        }

        public async Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> ids)
        {
            var db = _redis.GetDatabase();
            var result = new List<Preference>();
            var uncachedIds = new List<Guid>();

            foreach (var id in ids)
            {
                string cacheKey = $"{PREFIX_KEY}:{id}";
                var cached = await db.StringGetAsync(cacheKey);

                if (!cached.IsNull)
                {
                    result.Add(JsonSerializer.Deserialize<Preference>(cached));
                }
                else
                {
                    uncachedIds.Add(id);
                }
            }

            if (uncachedIds.Any())
            {
                var uncachedPreferences = await _repository.GetRangeByIdsAsync(uncachedIds);
                foreach (var preference in uncachedPreferences)
                {
                    string cacheKey = $"{PREFIX_KEY}:{preference.Id}";
                    await db.StringSetAsync(
                        cacheKey,
                        JsonSerializer.Serialize(preference),
                        _cacheExpiration);
                }

                result.AddRange(uncachedPreferences);
            }

            return result.OrderBy(p => ids.IndexOf(p.Id));
        }

        public async Task<IEnumerable<Preference>> GetWhere(Expression<Func<Preference, bool>> predicate)
        {
            var db = _redis.GetDatabase();
            string cacheKey = $"{PREFIX_KEY}:all";

            var cached = await db.StringGetAsync(cacheKey);
            if (!cached.IsNull)
            {
                var preferences = JsonSerializer.Deserialize<IEnumerable<Preference>>(cached);
                return preferences.Where(predicate.Compile());
            }
            var result = await _repository.GetWhere(predicate);
            await InvalidateCache();

            return result;
        }

        public async Task UpdateAsync(Preference entity)
        {
            await _repository.UpdateAsync(entity);
            await InvalidateCache(entity.Id);
        }

        private async Task InvalidateCache(Guid preferenceId) 
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"{PREFIX_KEY}:{preferenceId}");
            await db.KeyDeleteAsync($"{PREFIX_KEY}:all");
        }

        private async Task InvalidateCache()
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync($"{PREFIX_KEY}:all");
        }
    }
}
