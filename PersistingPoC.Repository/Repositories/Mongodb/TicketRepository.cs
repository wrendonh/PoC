using MongoDB.Driver;
using PersistingPoC.Repository.Interfaces.Mongodb;
using PersistingPoC.Repository.Models.Mongodb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PersistingPoC.Repository.Repositories.Mongodb
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IMongoCollection<Ticket> _tickets;

        public TicketRepository(ITicketStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _tickets = database.GetCollection<Ticket>(settings.TicketsCollectionName);
        }

        public IList<Ticket> GetAll()
        {
            return _tickets.Find(r => true).ToList();
        }

        public List<Ticket> GetAll(int skip, int take, Expression<Func<Ticket, object>> orderby, params Expression<Func<Ticket, bool>>[] filters)
        {
            IQueryable<Ticket> set = _tickets.AsQueryable();
            set = filters.Aggregate(set, (current, filter) => current.Where(filter));

            var ordered = ObjectSort(set, orderby);
            return ordered.Skip(skip).Take(take).ToList();
        }

        public IList<Ticket> GetAll(Expression<Func<Ticket, bool>> expression)
        {
            return _tickets.AsQueryable().Where(expression).ToList();
        }

        public async Task<IList<Ticket>> GetAllAsync()
        {
            return await _tickets.Find(r => true).ToListAsync();
        }
        
        public Ticket GetById(string id)
        {
            return _tickets.Find(r => r.Id == id).FirstOrDefault();            
        }

        public async Task<Ticket> GetByExternalIdAsync(int externalId)
        {
            return await _tickets.Find(x => x.ExternalTicketId == externalId).FirstOrDefaultAsync();
        }

        public void Insert(Ticket entity)
        {
            _tickets.InsertOne(entity);
        }

        public async Task InsertAsync(Ticket entity)
        {
            await _tickets.InsertOneAsync(entity);
        }

        public void Update(Ticket entity)
        {
            _tickets.ReplaceOne(r => r.Id == entity.Id, entity);
        }

        public async Task UpdateAsync(Ticket entity)
        {
            await _tickets.ReplaceOneAsync(r => r.Id == entity.Id, entity);
        }

        public async Task UpdateTicketAndDetailsAsync(Ticket ticket)
        {
            var filter = Builders<Ticket>.Filter.Eq("ExternalTicketId", ticket.ExternalTicketId);
            await _tickets.FindOneAndReplaceAsync<Ticket>(filter, ticket);
        }

        public void Delete(Ticket entity)
        {
            _tickets.DeleteOne(r => r.Id == entity.Id);
        }

        public async Task DeleteAsync(Ticket entity)
        {
            await _tickets.DeleteOneAsync(r => r.Id == entity.Id);
        }

        protected IOrderedQueryable<TT> ObjectSort<TT>(IQueryable<TT> entities, Expression<Func<TT, object>> expression)
        {
            if (!(expression.Body is UnaryExpression unaryExpression))
            {
                return entities.OrderBy(expression);
            }

            var propertyExpression = (MemberExpression)unaryExpression.Operand;
            var parameters = expression.Parameters;

            if (propertyExpression.Type == typeof(DateTime))
            {
                var newExpression = Expression.Lambda<Func<TT, DateTime>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            if (propertyExpression.Type == typeof(DateTime?))
            {
                var newExpression = Expression.Lambda<Func<TT, DateTime?>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            if (propertyExpression.Type == typeof(int))
            {
                var newExpression = Expression.Lambda<Func<TT, int>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            if (propertyExpression.Type == typeof(long))
            {
                var newExpression = Expression.Lambda<Func<TT, long>>(propertyExpression, parameters);
                return entities.OrderBy(newExpression);
            }

            throw new NotSupportedException("Object type resolution not implemented for this type");
        }
    }
}
