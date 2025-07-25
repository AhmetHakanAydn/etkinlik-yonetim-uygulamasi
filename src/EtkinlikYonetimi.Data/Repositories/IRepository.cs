using System.Linq.Expressions;

namespace EtkinlikYonetimi.Data.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entities that match the specified expression
        /// </summary>
        /// <param name="expression">The search expression</param>
        /// <returns>Collection of matching entities</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Gets the first entity that matches the specified expression
        /// </summary>
        /// <param name="expression">The search expression</param>
        /// <returns>The first matching entity or null</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Adds a new entity to the repository
        /// </summary>
        /// <param name="entity">The entity to add</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the repository
        /// </summary>
        /// <param name="entities">The entities to add</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity from the repository
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        void Remove(T entity);

        /// <summary>
        /// Removes multiple entities from the repository
        /// </summary>
        /// <param name="entities">The entities to remove</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Gets the total count of entities
        /// </summary>
        /// <returns>The total count</returns>
        Task<int> CountAsync();

        /// <summary>
        /// Gets the count of entities that match the specified expression
        /// </summary>
        /// <param name="expression">The filter expression</param>
        /// <returns>The count of matching entities</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Checks if any entity matches the specified expression
        /// </summary>
        /// <param name="expression">The search expression</param>
        /// <returns>True if any entity matches, false otherwise</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
}